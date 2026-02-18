using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Corelio.BlazorApp.Components;
using Corelio.BlazorApp.Services;
using Corelio.BlazorApp.Services.Authentication;
using Corelio.BlazorApp.Services.Http;
using Corelio.BlazorApp.Services.Customers;
using Corelio.BlazorApp.Services.Pos;
using Corelio.BlazorApp.Services.Pricing;
using Corelio.BlazorApp.Services.Products;
using Corelio.BlazorApp.Services.Theming;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization options for HttpClient
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Convert enums to/from strings (e.g., "PCS" instead of 0)
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

    // Case-insensitive property matching
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Add MudBlazor services
builder.Services.AddMudServices();

// Add Localization with Spanish (Mexico) as default culture
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("es-MX"), new CultureInfo("en-US") };
    options.DefaultRequestCulture = new RequestCulture("es-MX");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Add Authentication & Authorization services
// Blazor Server authentication is handled entirely through AuthenticationStateProvider.
// No ASP.NET Core authentication middleware needed - we use <AuthorizeView> in components.
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Token storage - circuit-scoped (one instance per user connection)
builder.Services.AddScoped<ITokenService, TokenService>();

// Authentication state provider
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

// Add Aspire service discovery
builder.Services.AddServiceDiscovery();

// Helper method to configure API base address
void ConfigureApiClient(IServiceProvider sp, HttpClient client)
{
    // When running with Aspire, "http://api" will be resolved via service discovery
    // When running standalone, fall back to appsettings
    var apiUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";

    // Check if running in Aspire environment
    if (builder.Configuration.GetSection("services:api").Exists())
    {
        client.BaseAddress = new Uri("http://api");
        Console.WriteLine($"Blazor using Aspire service discovery for API");
    }
    else
    {
        client.BaseAddress = new Uri(apiUrl);
        Console.WriteLine($"Blazor connecting to API at: {apiUrl}");
    }
}

// HttpClient for authentication endpoints (no auth needed - used for login/register)
builder.Services.AddHttpClient("api-auth", ConfigureApiClient)
    .AddServiceDiscovery();

// HttpClient for protected endpoints (used by AuthenticatedHttpClient wrapper)
builder.Services.AddHttpClient("api", ConfigureApiClient)
    .AddServiceDiscovery();

// AuthenticatedHttpClient wrapper - handles authorization headers automatically
// This approach works perfectly with Blazor Server's circuit scoping because
// TokenService is injected via constructor (circuit-scoped), not resolved from a handler scope
builder.Services.AddScoped<AuthenticatedHttpClient>(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("api");
    var tokenService = sp.GetRequiredService<ITokenService>();
    return new AuthenticatedHttpClient(httpClient, tokenService);
});

// Add AuthService (depends on HttpClient)
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Product service
builder.Services.AddScoped<IProductService, ProductService>();

// Add Pricing service
builder.Services.AddScoped<IPricingService, PricingService>();

// Add Customer service
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Add POS service
builder.Services.AddScoped<IPosService, PosService>();

// Add Theme services
builder.Services.AddScoped<IDynamicThemeService, DynamicThemeService>();
builder.Services.AddScoped<ITenantThemeHttpService, TenantThemeHttpService>();

// Add Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use Request Localization (Spanish culture)
app.UseRequestLocalization();

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// Only use HTTPS redirection in production (not when running through Aspire)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
