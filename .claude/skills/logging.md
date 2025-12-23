---
name: logging
description: Validates structured logging with Serilog and proper log levels
---

# Logging Best Practices

## Structured Logging with Serilog

### Configuration
```csharp
// Program.cs
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "Corelio")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/corelio-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Seq(
        serverUrl: builder.Configuration["Serilog:SeqUrl"] ?? "http://localhost:5341",
        apiKey: builder.Configuration["Serilog:SeqApiKey"])
    .CreateLogger();

try
{
    Log.Information("Starting Corelio application");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // ... rest of configuration

    var app = builder.Build();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

## Log Levels

### Trace (Most Verbose)
```csharp
// Use for: Low-level tracing, very detailed debugging
_logger.LogTrace(
    "Entering method {MethodName} with parameters {Parameters}",
    nameof(CreateProduct),
    new { name, sku, price });

// Typically disabled in production
```

### Debug
```csharp
// Use for: Detailed diagnostic information useful during development
_logger.LogDebug(
    "Loading product {ProductId} from database",
    productId);

_logger.LogDebug(
    "Query executed in {ElapsedMs}ms: {Query}",
    elapsed,
    query);

// Usually disabled in production
```

### Information
```csharp
// Use for: General informational messages about application flow
_logger.LogInformation(
    "User {UserId} created product {ProductId}",
    userId,
    productId);

_logger.LogInformation(
    "CFDI invoice {InvoiceId} stamped successfully",
    invoiceId);

_logger.LogInformation(
    "Processing batch of {Count} sales for tenant {TenantId}",
    salesCount,
    tenantId);

// Enabled in production
```

### Warning
```csharp
// Use for: Abnormal or unexpected events that don't prevent operation
_logger.LogWarning(
    "Product {ProductId} inventory below minimum level: {Current} (minimum: {Minimum})",
    productId,
    currentStock,
    minimumStock);

_logger.LogWarning(
    "Failed login attempt for {Email} from {IpAddress}",
    email,
    ipAddress);

_logger.LogWarning(
    "CSD certificate for tenant {TenantId} expires in {DaysUntilExpiration} days",
    tenantId,
    daysUntilExpiration);

// Always enabled
```

### Error
```csharp
// Use for: Errors and exceptions that don't crash the application
_logger.LogError(
    exception,
    "Failed to stamp CFDI for sale {SaleId}",
    saleId);

_logger.LogError(
    exception,
    "Email sending failed for {Recipient}",
    recipient);

_logger.LogError(
    "Database query timeout after {TimeoutSeconds}s: {Query}",
    timeoutSeconds,
    query);

// Always enabled, triggers alerts
```

### Critical
```csharp
// Use for: Critical failures that require immediate attention
_logger.LogCritical(
    exception,
    "Database connection lost - application cannot continue");

_logger.LogCritical(
    exception,
    "PAC service unavailable for {Duration} - invoicing suspended",
    duration);

_logger.LogCritical(
    "Potential data leak: User {UserId} in tenant {TenantId} accessed resource from tenant {OtherTenantId}",
    userId,
    currentTenantId,
    otherTenantId);

// Always enabled, triggers immediate alerts
```

## Structured Logging Patterns

### ✅ DO: Use Message Templates
```csharp
// ✅ CORRECT - Structured logging
_logger.LogInformation(
    "Product {ProductId} created by {UserId} at {Timestamp}",
    productId,
    userId,
    DateTime.UtcNow);

// ❌ WRONG - String interpolation (not structured)
_logger.LogInformation($"Product {productId} created by {userId}");

// ❌ WRONG - String concatenation
_logger.LogInformation("Product " + productId + " created by " + userId);
```

### ✅ DO: Use Semantic Property Names
```csharp
// ✅ CORRECT - Clear property names
_logger.LogInformation(
    "Sale completed: {SaleId}, Total: {TotalAmount:C}, Items: {ItemCount}",
    sale.Id,
    sale.TotalAmount,
    sale.Items.Count);

// ❌ WRONG - Generic property names
_logger.LogInformation(
    "Sale completed: {Id}, {Amount}, {Count}",
    sale.Id,
    sale.TotalAmount,
    sale.Items.Count);
```

### ✅ DO: Log Exceptions with Context
```csharp
// ✅ CORRECT - Exception with context
try
{
    await _pacService.StampCfdiAsync(xml);
}
catch (PacServiceException ex)
{
    _logger.LogError(
        ex,
        "Failed to stamp CFDI for sale {SaleId}, Customer {CustomerId}, Tenant {TenantId}",
        saleId,
        customerId,
        tenantId);

    return Result.Failure($"CFDI stamping failed: {ex.Message}");
}

// ❌ WRONG - Exception without context
catch (PacServiceException ex)
{
    _logger.LogError(ex, "CFDI stamping failed");
}
```

### ✅ DO: Use LoggerMessage for High-Performance Logging
```csharp
// For frequently called code paths
public static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Product {ProductId} created by user {UserId}")]
    public static partial void LogProductCreated(
        this ILogger logger,
        Guid productId,
        Guid userId);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Warning,
        Message = "Failed login attempt for {Email} from {IpAddress}")]
    public static partial void LogFailedLogin(
        this ILogger logger,
        string email,
        string ipAddress);
}

// Usage
_logger.LogProductCreated(productId, userId);
```

## What to Log

### ✅ ALWAYS Log

#### Security Events
```csharp
// Authentication
_logger.LogInformation("User {UserId} logged in from {IpAddress}", userId, ip);
_logger.LogWarning("Failed login for {Email} from {IpAddress}", email, ip);
_logger.LogWarning("Account {UserId} locked after {Attempts} failed attempts", userId, attempts);

// Authorization
_logger.LogWarning(
    "User {UserId} denied access to {Resource} - missing permission {Permission}",
    userId,
    resourceName,
    requiredPermission);

// Multi-Tenancy Violations (CRITICAL)
_logger.LogCritical(
    "TENANT ISOLATION VIOLATION: User {UserId} (Tenant {UserTenantId}) attempted access to resource {ResourceId} (Tenant {ResourceTenantId})",
    userId,
    userTenantId,
    resourceId,
    resourceTenantId);
```

#### Business Operations
```csharp
// CFDI Operations
_logger.LogInformation(
    "CFDI invoice {InvoiceId} created for sale {SaleId}, Amount {Amount:C}",
    invoiceId,
    saleId,
    amount);

_logger.LogInformation(
    "CFDI invoice {InvoiceId} cancelled, Reason: {CancellationReason}",
    invoiceId,
    reason);

// Financial Transactions
_logger.LogInformation(
    "Sale {SaleId} completed: Customer {CustomerId}, Total {Total:C}, Payment {PaymentMethod}",
    saleId,
    customerId,
    total,
    paymentMethod);

// Inventory Changes
_logger.LogInformation(
    "Stock adjustment for product {ProductId}: {OldQuantity} → {NewQuantity}, Reason: {Reason}",
    productId,
    oldQuantity,
    newQuantity,
    reason);
```

#### Performance Issues
```csharp
// Slow queries
_logger.LogWarning(
    "Slow query detected: {ElapsedMs}ms - {QueryType}",
    elapsed.TotalMilliseconds,
    queryType);

// Rate limiting
_logger.LogWarning(
    "Rate limit exceeded for {IpAddress} on endpoint {Endpoint}",
    ipAddress,
    endpoint);
```

#### Errors
```csharp
// All exceptions
_logger.LogError(
    exception,
    "Operation failed in {MethodName}: {ErrorMessage}",
    methodName,
    exception.Message);

// Integration failures
_logger.LogError(
    exception,
    "PAC service call failed: {ErrorCode} - {ErrorMessage}",
    errorCode,
    errorMessage);
```

### ❌ NEVER Log

#### Sensitive Data
```csharp
// ❌ WRONG - Logging passwords
_logger.LogDebug("User login with password {Password}", password); // DON'T!

// ❌ WRONG - Logging credit card numbers
_logger.LogInformation("Payment with card {CardNumber}", cardNumber); // DON'T!

// ❌ WRONG - Logging API keys
_logger.LogDebug("Calling PAC with key {ApiKey}", apiKey); // DON'T!

// ❌ WRONG - Logging personal data unnecessarily
_logger.LogInformation("Customer details: {CustomerJson}", JsonSerializer.Serialize(customer)); // DON'T!

// ✅ CORRECT - Log identifiers only
_logger.LogInformation("Customer {CustomerId} payment processed", customerId);
_logger.LogInformation("Calling PAC service for tenant {TenantId}", tenantId);
```

#### PII (Personally Identifiable Information)
```csharp
// ❌ WRONG
_logger.LogInformation("User {Email} {PhoneNumber} registered", email, phone);

// ✅ CORRECT - Hash or omit PII
_logger.LogInformation("User {UserId} registered", userId);
```

## Log Enrichment

### Request Context
```csharp
// Middleware to enrich logs with request context
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
        using (LogContext.PushProperty("UserAgent", context.Request.Headers["User-Agent"].ToString()))
        using (LogContext.PushProperty("IpAddress", context.Connection.RemoteIpAddress?.ToString()))
        {
            logger.LogInformation(
                "HTTP {Method} {Path} started",
                context.Request.Method,
                context.Request.Path);

            var sw = Stopwatch.StartNew();

            await next(context);

            sw.Stop();

            logger.LogInformation(
                "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);
        }
    }
}

// Register middleware
app.UseMiddleware<RequestLoggingMiddleware>();
```

### Tenant Context
```csharp
// Enrich all logs with tenant ID
public class TenantLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        var tenantId = tenantService.GetCurrentTenantId();

        using (LogContext.PushProperty("TenantId", tenantId))
        {
            await next(context);
        }
    }
}
```

### User Context
```csharp
// Enrich with user information
public class UserLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = context.User.FindFirst(ClaimTypes.Name)?.Value;

            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("UserName", userName))
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}
```

## Correlation IDs

```csharp
// Add correlation ID to track requests across services
public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Response.Headers.Add(CorrelationIdHeader, correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
```

## Query Logging (EF Core)

```csharp
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}

// Or configure in code
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseNpgsql(connectionString)
        .LogTo(
            Console.WriteLine,
            new[] { DbLoggerCategory.Database.Command.Name },
            LogLevel.Information)
        .EnableSensitiveDataLogging() // Only in development!
        .EnableDetailedErrors();
}
```

## Application Insights Integration

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// Add telemetry enrichment
builder.Services.AddSingleton<ITelemetryInitializer, TenantTelemetryInitializer>();

public class TenantTelemetryInitializer : ITelemetryInitializer
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Initialize(ITelemetry telemetry)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            var tenantId = context.User.FindFirst("tenant_id")?.Value;
            if (tenantId != null)
            {
                telemetry.Context.GlobalProperties["TenantId"] = tenantId;
            }
        }
    }
}
```

## Logging Checklist

### Configuration
- [ ] Serilog configured with multiple sinks (Console, File, Seq)
- [ ] Log levels appropriate for environment
- [ ] Structured logging enabled
- [ ] Log enrichment configured (machine name, environment, etc.)
- [ ] Log retention policy configured

### What to Log
- [ ] Security events (login, logout, authorization failures)
- [ ] Business operations (sales, invoices, inventory changes)
- [ ] Multi-tenancy violations (CRITICAL)
- [ ] Performance issues (slow queries, timeouts)
- [ ] All exceptions with context
- [ ] Integration failures (PAC, email, etc.)

### What NOT to Log
- [ ] Passwords or credentials
- [ ] Credit card numbers
- [ ] API keys or secrets
- [ ] Unnecessary PII
- [ ] Full entity objects with sensitive data

### Best Practices
- [ ] Use message templates (not string interpolation)
- [ ] Include correlation IDs
- [ ] Enrich with tenant and user context
- [ ] Use appropriate log levels
- [ ] Log exceptions with full context
- [ ] Use LoggerMessage for high-frequency logs

---

**Remember**: Good logging is essential for troubleshooting and security audits!
