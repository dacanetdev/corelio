---
name: error-handling
description: Ensures proper Result<T> pattern usage and exception handling
---

# Error Handling Best Practices

## Result<T> Pattern

### Result Class
```csharp
// Corelio.Domain/Common/Result.cs
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public List<string> Errors { get; }

    private Result(bool isSuccess, T value, List<string> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T value) =>
        new(true, value, new List<string>());

    public static Result<T> Failure(string error) =>
        new(false, default!, new List<string> { error });

    public static Result<T> Failure(List<string> errors) =>
        new(false, default!, errors);

    // Pattern matching support
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<List<string>, TResult> onFailure)
    {
        return IsSuccess
            ? onSuccess(Value)
            : onFailure(Errors);
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> onSuccess,
        Func<List<string>, Task<TResult>> onFailure)
    {
        return IsSuccess
            ? await onSuccess(Value)
            : await onFailure(Errors);
    }
}

// Non-generic version
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public List<string> Errors { get; }

    private Result(bool isSuccess, List<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new List<string>();
    }

    public static Result Success() =>
        new(true, new List<string>());

    public static Result Failure(string error) =>
        new(false, new List<string> { error });

    public static Result Failure(List<string> errors) =>
        new(false, errors);
}
```

### Command Handler with Result
```csharp
// ✅ CORRECT - Return Result<T>
public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IMapper _mapper;

    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Check for duplicate SKU
        var exists = await _context.Products
            .AnyAsync(p => p.Sku == request.Sku, cancellationToken);

        if (exists)
            return Result<ProductDto>.Failure("Product with this SKU already exists");

        // Create product
        var product = new Product
        {
            Name = request.Name,
            Sku = request.Sku,
            Price = request.Price,
            CategoryId = request.CategoryId,
            TenantId = _tenantService.GetCurrentTenantId()
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<ProductDto>(product);
        return Result<ProductDto>.Success(dto);
    }
}

// ❌ WRONG - Throwing exceptions for business logic
public async Task<ProductDto> Handle(CreateProductCommand request)
{
    var exists = await _context.Products.AnyAsync(p => p.Sku == request.Sku);

    if (exists)
        throw new DuplicateSkuException("SKU already exists"); // DON'T!

    // ...
}
```

### Controller Usage
```csharp
[HttpPost]
public async Task<ActionResult<ProductDto>> CreateProduct(
    [FromBody] CreateProductRequest request)
{
    var command = new CreateProductCommand(
        request.Name,
        request.Sku,
        request.Price,
        request.CategoryId);

    var result = await _mediator.Send(command);

    // Pattern matching
    return result.Match<ActionResult<ProductDto>>(
        success => CreatedAtAction(
            nameof(GetProduct),
            new { id = success.Id },
            success),
        errors => BadRequest(new { errors }));

    // Or traditional if/else
    if (result.IsSuccess)
    {
        return CreatedAtAction(
            nameof(GetProduct),
            new { id = result.Value.Id },
            result.Value);
    }

    return BadRequest(new { errors = result.Errors });
}
```

## Exception Handling

### Custom Exceptions
```csharp
// Domain exceptions
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found") { }
}

public class ValidationException : DomainException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation failures occurred")
    {
        Errors = errors;
    }
}

public class ForbiddenAccessException : DomainException
{
    public ForbiddenAccessException()
        : base("You do not have permission to access this resource") { }
}

public class TenantMismatchException : DomainException
{
    public TenantMismatchException()
        : base("Resource does not belong to current tenant") { }
}
```

### Global Exception Handler
```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "One or more validation failures occurred"
            ),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                "Not Found",
                notFoundEx.Message
            ),

            ForbiddenAccessException forbiddenEx => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                forbiddenEx.Message
            ),

            TenantMismatchException tenantEx => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                tenantEx.Message
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "You must be authenticated to access this resource"
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred"
            )
        };

        // Log error
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "An unhandled exception occurred");
        }
        else
        {
            _logger.LogWarning(exception, "A handled exception occurred");
        }

        // Build response
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        // Add validation errors if applicable
        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}

// Register in Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

app.UseExceptionHandler();
```

## FluentValidation

### Validator
```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters")
            .MustAsync(BeUniqueSku).WithMessage("SKU already exists");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required")
            .MustAsync(CategoryExists).WithMessage("Category does not exist");
    }

    private async Task<bool> BeUniqueSku(string sku, CancellationToken cancellationToken)
    {
        return !await _context.Products
            .AnyAsync(p => p.Sku == sku, cancellationToken);
    }

    private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.ProductCategories
            .AnyAsync(c => c.Id == categoryId, cancellationToken);
    }
}
```

### Validation Behavior
```csharp
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var errors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.ErrorMessage).ToArray());

            throw new ValidationException(errors);
        }

        return await next();
    }
}

// Register in Program.cs
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));
```

## Try-Catch Best Practices

### ✅ DO: Catch Specific Exceptions
```csharp
public async Task<Result<InvoiceDto>> GenerateCfdiAsync(Guid saleId)
{
    try
    {
        var sale = await _context.Sales.FindAsync(saleId);
        if (sale == null)
            return Result<InvoiceDto>.Failure("Sale not found");

        var xml = await _cfdiService.GenerateXmlAsync(sale);
        var stamped = await _pacService.StampAsync(xml);

        return Result<InvoiceDto>.Success(stamped);
    }
    catch (PacServiceException ex) when (ex.ErrorCode == "301")
    {
        // Certificate expired
        _logger.LogError(ex, "CSD certificate expired for tenant {TenantId}", sale.TenantId);
        return Result<InvoiceDto>.Failure("Your CFDI certificate has expired");
    }
    catch (PacServiceException ex)
    {
        // Other PAC errors
        _logger.LogError(ex, "PAC service error: {ErrorCode}", ex.ErrorCode);
        return Result<InvoiceDto>.Failure($"CFDI stamping failed: {ex.Message}");
    }
    catch (HttpRequestException ex)
    {
        // Network error
        _logger.LogError(ex, "Network error communicating with PAC");
        return Result<InvoiceDto>.Failure("Unable to connect to CFDI service");
    }
}
```

### ❌ DON'T: Catch and Rethrow Without Value
```csharp
// ❌ WRONG - Loses stack trace
try
{
    await _service.DoSomethingAsync();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error occurred");
    throw ex; // Loses stack trace!
}

// ✅ CORRECT - Preserves stack trace
try
{
    await _service.DoSomethingAsync();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error occurred");
    throw; // Preserves stack trace
}

// ✅ CORRECT - Wrap with additional context
try
{
    await _service.DoSomethingAsync();
}
catch (Exception ex)
{
    throw new ApplicationException("Failed to process operation", ex);
}
```

### ❌ DON'T: Catch Exception for Flow Control
```csharp
// ❌ WRONG - Using exceptions for flow control
public async Task<Product?> GetProductBySkuAsync(string sku)
{
    try
    {
        return await _context.Products.SingleAsync(p => p.Sku == sku);
    }
    catch (InvalidOperationException)
    {
        return null; // Don't use exceptions for expected conditions!
    }
}

// ✅ CORRECT - Use appropriate method
public async Task<Product?> GetProductBySkuAsync(string sku)
{
    return await _context.Products.SingleOrDefaultAsync(p => p.Sku == sku);
}
```

## Logging Best Practices

### Structured Logging
```csharp
// ❌ WRONG - String interpolation
_logger.LogInformation($"User {userId} created product {productId}");

// ✅ CORRECT - Structured logging
_logger.LogInformation(
    "User {UserId} created product {ProductId}",
    userId,
    productId);
```

### Log Levels
```csharp
// Trace - Very detailed, only for development
_logger.LogTrace("Entering method {MethodName}", nameof(CreateProduct));

// Debug - Detailed information for debugging
_logger.LogDebug("Product {ProductId} loaded from database", productId);

// Information - General flow of application
_logger.LogInformation("Product {ProductId} created successfully", productId);

// Warning - Unexpected but handled situations
_logger.LogWarning("Product {ProductId} not found, creating new", productId);

// Error - Error conditions that don't stop execution
_logger.LogError(ex, "Failed to stamp CFDI for sale {SaleId}", saleId);

// Critical - Critical failures requiring immediate attention
_logger.LogCritical(ex, "Database connection lost");
```

### Security Logging
```csharp
// Log security events
_logger.LogWarning(
    "Failed login attempt for {Email} from {IpAddress}",
    request.Email,
    httpContext.Connection.RemoteIpAddress);

_logger.LogWarning(
    "Unauthorized access attempt to {Resource} by user {UserId}",
    resourceName,
    userId);

_logger.LogCritical(
    "Potential tenant data leak detected: User {UserId} in tenant {TenantId} attempted to access resource in tenant {TargetTenantId}",
    userId,
    currentTenantId,
    resourceTenantId);
```

## Async/Await Best Practices

### ✅ DO: Use ConfigureAwait(false) in Libraries
```csharp
// In class library code (not ASP.NET Core)
public async Task<Product> GetProductAsync(Guid id)
{
    var product = await _context.Products
        .FindAsync(id)
        .ConfigureAwait(false);

    return product;
}
```

### ❌ DON'T: Use async void
```csharp
// ❌ WRONG - Can't catch exceptions
private async void LoadProducts()
{
    _products = await _client.GetProductsAsync();
}

// ✅ CORRECT
private async Task LoadProductsAsync()
{
    _products = await _client.GetProductsAsync();
}
```

### ✅ DO: Handle Exceptions in Fire-and-Forget
```csharp
// ❌ WRONG - Unhandled exceptions
_ = Task.Run(async () =>
{
    await SendEmailAsync(); // Exception will be swallowed!
});

// ✅ CORRECT
_ = Task.Run(async () =>
{
    try
    {
        await SendEmailAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to send email");
    }
});
```

## Error Handling Checklist

### Command/Query Handlers
- [ ] Return `Result<T>` for business logic errors
- [ ] Throw exceptions only for exceptional conditions
- [ ] Log errors with structured logging
- [ ] Include context in error messages
- [ ] Don't expose internal details to clients

### Controllers
- [ ] Use pattern matching with Result<T>
- [ ] Return appropriate HTTP status codes
- [ ] Use ProblemDetails for errors
- [ ] Don't catch exceptions (let global handler deal with them)
- [ ] Validate input with FluentValidation

### Exception Handling
- [ ] Catch specific exceptions, not generic Exception
- [ ] Log exceptions with full context
- [ ] Don't use exceptions for flow control
- [ ] Preserve stack traces when rethrowing
- [ ] Clean up resources with using/finally

### Validation
- [ ] Use FluentValidation for input validation
- [ ] Validate at API boundary
- [ ] Return validation errors to client
- [ ] Don't duplicate validation logic

---

**Remember**: Exceptions are for exceptional conditions, Result<T> is for business logic!
