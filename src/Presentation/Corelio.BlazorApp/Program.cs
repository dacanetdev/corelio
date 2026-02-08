using System.Globalization;
using Blazored.LocalStorage;
using Corelio.BlazorApp.Components;
using Corelio.BlazorApp.Services;
using Corelio.BlazorApp.Services.Authentication;
using Corelio.BlazorApp.Services.Pricing;
using Corelio.BlazorApp.Services.Products;
using Corelio.BlazorApp.Services.Theming;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add Localization with Spanish (Mexico) as default culture
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("es-MX"), new CultureInfo("en-US") };
    options.DefaultRequestCulture = new RequestCulture("es-MX");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Add Authentication services
// Even though Blazor uses AuthenticationStateProvider, we need to register
// an authentication scheme for the authorization middleware to work with [Authorize]
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/access-denied";
    });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ITokenService, TokenService>();
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

// HttpClient for authentication endpoints (no auth handler to avoid circular dependency)
builder.Services.AddHttpClient("api-auth", ConfigureApiClient)
    .AddServiceDiscovery();

// HttpClient for protected endpoints (with authorization handler)
builder.Services.AddTransient<AuthorizationMessageHandler>();
builder.Services.AddHttpClient("api", ConfigureApiClient)
    .AddHttpMessageHandler<AuthorizationMessageHandler>()
    .AddServiceDiscovery();

// Register default HttpClient for general use (with auth handler)
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

// Add AuthService (depends on HttpClient)
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Product service
builder.Services.AddScoped<IProductService, ProductService>();

// Add Pricing service
builder.Services.AddScoped<IPricingService, PricingService>();

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

// Authentication middleware is required for authorization to work with [Authorize] attributes
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
