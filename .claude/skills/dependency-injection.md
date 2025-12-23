---
name: dependency-injection
description: Validates DI setup, service lifetimes, and avoids common pitfalls
---

# Dependency Injection Best Practices

## Service Lifetimes

### Transient
```csharp
// ✅ Use for lightweight, stateless services
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();

// Characteristics:
// - Created each time requested
// - No state shared between requests
// - Disposed when scope ends
// - Thread-safe by default
```

### Scoped
```csharp
// ✅ Use for per-request services (most common in ASP.NET Core)
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Characteristics:
// - Created once per HTTP request
// - Same instance within request
// - Disposed at end of request
// - EF Core contexts should always be scoped
```

### Singleton
```csharp
// ✅ Use for thread-safe, stateless services or expensive to create
builder.Services.AddSingleton<ISatCatalogService, SatCatalogService>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// ⚠️ WARNING: Must be thread-safe!
// Characteristics:
// - Created once for application lifetime
// - Shared across all requests
// - Never disposed until application shutdown
// - Must be thread-safe!
```

## Registration Patterns

### Interface-Implementation
```csharp
// ✅ CORRECT - Register interface with implementation
builder.Services.AddScoped<IProductService, ProductService>();

// Usage in constructor
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
}
```

### Primary Constructors (C# 12+)
```csharp
// ✅ CORRECT - Use primary constructors for DI
public class ProductsController(
    IProductService productService,
    ILogger<ProductsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        logger.LogInformation("Getting all products");
        return Ok(await productService.GetAllAsync());
    }
}
```

### Multiple Implementations
```csharp
// Register all implementations
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<INotificationService, SmsNotificationService>();

// Inject IEnumerable<T> to get all
public class NotificationManager(
    IEnumerable<INotificationService> notificationServices)
{
    public async Task SendAllAsync(string message)
    {
        foreach (var service in notificationServices)
        {
            await service.SendAsync(message);
        }
    }
}
```

### Keyed Services (.NET 8+)
```csharp
// Register with keys
builder.Services.AddKeyedScoped<IPacService, FinkelPacService>("finkel");
builder.Services.AddKeyedScoped<IPacService, SwPacService>("sw");

// Inject with key
public class CfdiService(
    [FromKeyedServices("finkel")] IPacService pacService)
{
    public async Task<string> StampAsync(string xml)
    {
        return await pacService.StampCfdiAsync(xml);
    }
}
```

## Common Pitfalls

### ❌ PITFALL 1: Captive Dependency
```csharp
// ❌ WRONG - Singleton holding Scoped dependency
public class SingletonService
{
    private readonly IApplicationDbContext _context; // Scoped!

    public SingletonService(IApplicationDbContext context)
    {
        _context = context; // DON'T! Context will never be disposed
    }
}

// ✅ CORRECT - Use IServiceScopeFactory
public class SingletonService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SingletonService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task DoWorkAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        // Use context...
    }
}
```

### ❌ PITFALL 2: Resolving Scoped Service in Singleton
```csharp
// ❌ WRONG - Resolving scoped from singleton constructor
public class CacheService
{
    public CacheService(IServiceProvider serviceProvider)
    {
        // This will throw an exception!
        var context = serviceProvider.GetRequiredService<IApplicationDbContext>();
    }
}

// ✅ CORRECT - Create scope when needed
public class CacheService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public CacheService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        return await context.Products.ToListAsync();
    }
}
```

### ❌ PITFALL 3: Service Locator Anti-Pattern
```csharp
// ❌ WRONG - Using IServiceProvider directly
public class ProductService
{
    private readonly IServiceProvider _serviceProvider;

    public ProductService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task CreateAsync(Product product)
    {
        // Anti-pattern! Dependencies not explicit
        var validator = _serviceProvider.GetRequiredService<IProductValidator>();
        var logger = _serviceProvider.GetRequiredService<ILogger<ProductService>>();
    }
}

// ✅ CORRECT - Inject dependencies explicitly
public class ProductService
{
    private readonly IProductValidator _validator;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductValidator validator,
        ILogger<ProductService> logger)
    {
        _validator = validator;
        _logger = logger;
    }
}
```

### ❌ PITFALL 4: Disposing Scoped Services
```csharp
// ❌ WRONG - Manually disposing scoped service
public class ProductsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ProductsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        var products = await _context.Products.ToListAsync();

        // DON'T! Framework will dispose it
        if (_context is IDisposable disposable)
        {
            disposable.Dispose();
        }

        return Ok(products);
    }
}

// ✅ CORRECT - Let framework handle disposal
public class ProductsController(IApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        var products = await context.Products.ToListAsync();
        return Ok(products);
    }
    // Framework disposes context at end of request
}
```

## Registration Extensions

### Layer Registration
```csharp
// Corelio.Application/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}

// Corelio.Infrastructure/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Multi-Tenancy
        services.AddScoped<ITenantService, TenantService>();
        services.AddSingleton<ITenantResolver, JwtTenantResolver>();

        // Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICfdiService, CfdiService>();
        services.AddScoped<IPacService, FinkelPacService>();

        // Caching
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }
}

// Program.cs
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);
```

### Conditional Registration
```csharp
// Development vs Production
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, DevelopmentEmailSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, SendGridEmailSender>();
}

// Feature flags
if (builder.Configuration.GetValue<bool>("Features:EnableAdvancedReporting"))
{
    builder.Services.AddScoped<IReportingService, AdvancedReportingService>();
}
else
{
    builder.Services.AddScoped<IReportingService, BasicReportingService>();
}
```

### TryAdd Methods
```csharp
// Only registers if not already registered
builder.Services.TryAddScoped<IProductService, ProductService>();

// Useful in library code to allow overriding
public static IServiceCollection AddMyLibrary(this IServiceCollection services)
{
    services.TryAddScoped<IMyService, DefaultMyService>();
    return services;
}
```

## Options Pattern

### Configuration
```csharp
// appsettings.json
{
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "https://api.corelio.com",
    "Audience": "https://corelio.com",
    "ExpirationMinutes": 60
  }
}

// Options class
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Secret { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpirationMinutes { get; set; } = 60;
}

// Registration
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

// Validation
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Usage with IOptions<T>
public class JwtTokenGenerator(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _jwtOptions = options.Value;

    public string GenerateToken(User user)
    {
        // Use _jwtOptions.Secret, etc.
    }
}

// Usage with IOptionsSnapshot<T> (scoped, reloads on change)
public class CacheService(IOptionsSnapshot<CacheOptions> options)
{
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory)
    {
        var cacheOptions = options.Value;
        // ...
    }
}

// Usage with IOptionsMonitor<T> (singleton, reloads on change)
public class FeatureFlagService(IOptionsMonitor<FeatureFlags> options)
{
    public FeatureFlagService(IOptionsMonitor<FeatureFlags> options)
    {
        options.OnChange(flags =>
        {
            // React to configuration changes
            _logger.LogInformation("Feature flags updated");
        });
    }
}
```

## Factory Pattern

### Simple Factory
```csharp
public interface IPacServiceFactory
{
    IPacService Create(string providerName);
}

public class PacServiceFactory(IServiceProvider serviceProvider) : IPacServiceFactory
{
    public IPacService Create(string providerName)
    {
        return providerName.ToLowerInvariant() switch
        {
            "finkel" => serviceProvider.GetRequiredKeyedService<IPacService>("finkel"),
            "sw" => serviceProvider.GetRequiredKeyedService<IPacService>("sw"),
            _ => throw new ArgumentException($"Unknown PAC provider: {providerName}")
        };
    }
}

// Registration
builder.Services.AddScoped<IPacServiceFactory, PacServiceFactory>();
```

### Func<T> Factory
```csharp
// Registration
builder.Services.AddScoped<IPacService, FinkelPacService>();
builder.Services.AddScoped<Func<IPacService>>(provider =>
    () => provider.GetRequiredService<IPacService>());

// Usage
public class CfdiService(Func<IPacService> pacServiceFactory)
{
    public async Task<string> StampAsync(string xml)
    {
        var pacService = pacServiceFactory();
        return await pacService.StampCfdiAsync(xml);
    }
}
```

## Testing with DI

### Integration Tests
```csharp
public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove real database
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Replace services with mocks
            services.Remove(services.Single(d => d.ServiceType == typeof(IEmailSender)));
            services.AddScoped<IEmailSender, MockEmailSender>();
        });
    }
}
```

## Dependency Injection Checklist

### Service Registration
- [ ] Use appropriate lifetime (Transient, Scoped, Singleton)
- [ ] Register interfaces, not concrete types
- [ ] Avoid captive dependencies
- [ ] Don't use Service Locator pattern
- [ ] Group registrations by layer

### Service Implementation
- [ ] Use constructor injection (primary constructors)
- [ ] Keep constructors simple (no logic)
- [ ] Don't resolve services in constructors
- [ ] Make dependencies explicit
- [ ] Don't manually dispose scoped services

### Configuration
- [ ] Use Options pattern for configuration
- [ ] Validate options on startup
- [ ] Use IOptionsSnapshot for scoped changes
- [ ] Use IOptionsMonitor for singleton changes

### Thread Safety
- [ ] Ensure singleton services are thread-safe
- [ ] Don't share state in transient services
- [ ] Use scoped services for per-request state

---

**Remember**: Choose the right lifetime and avoid captive dependencies!
