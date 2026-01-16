using System.Globalization;
using Blazored.LocalStorage;
using Corelio.BlazorApp.Components;
using Corelio.BlazorApp.Services.Authentication;
using Corelio.BlazorApp.Services.Products;
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
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

// Add HttpClient for API calls with Authorization header
builder.Services.AddScoped<AuthorizationMessageHandler>();
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001")
    };
    return httpClient;
});

// Add AuthService (depends on HttpClient)
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Product service
builder.Services.AddScoped<IProductService, ProductService>();

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

// Note: Blazor Server uses AuthenticationStateProvider for authentication
// UseAuthentication() middleware is not needed and would cause errors
// Authorization is handled by the custom AuthenticationStateProvider

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
