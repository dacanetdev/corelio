---
name: api-design
description: Ensures RESTful API consistency, proper HTTP status codes, and validation patterns
---

# API Design Best Practices

## RESTful Endpoint Design

### Resource Naming
```csharp
// ✅ CORRECT - Plural nouns, lowercase, kebab-case
[Route("api/products")]
[Route("api/product-categories")]
[Route("api/sales")]
[Route("api/cfdi-invoices")]

// ❌ WRONG - Singular, verbs, mixed case
[Route("api/product")] // Should be plural
[Route("api/GetProducts")] // No verbs in URLs
[Route("api/ProductCategories")] // Use kebab-case
```

### HTTP Method Usage
```csharp
// ✅ GET - Retrieve resources (idempotent, cacheable)
[HttpGet]
public async Task<ActionResult<List<ProductDto>>> GetProducts()

[HttpGet("{id}")]
public async Task<ActionResult<ProductDto>> GetProduct(Guid id)

// ✅ POST - Create new resource
[HttpPost]
public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductRequest request)

// ✅ PUT - Replace entire resource
[HttpPut("{id}")]
public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, UpdateProductRequest request)

// ✅ PATCH - Partial update
[HttpPatch("{id}")]
public async Task<ActionResult<ProductDto>> PatchProduct(Guid id, JsonPatchDocument<Product> patch)

// ✅ DELETE - Remove resource
[HttpDelete("{id}")]
public async Task<ActionResult> DeleteProduct(Guid id)
```

## HTTP Status Codes

### Success Codes (2xx)
```csharp
// 200 OK - Successful GET, PUT, PATCH
return Ok(product);

// 201 Created - Successful POST
return CreatedAtAction(
    nameof(GetProduct),
    new { id = product.Id },
    product);

// 204 No Content - Successful DELETE or update with no response body
return NoContent();
```

### Client Error Codes (4xx)
```csharp
// 400 Bad Request - Validation failed
if (!ModelState.IsValid)
    return BadRequest(ModelState);

// 401 Unauthorized - Not authenticated
return Unauthorized();

// 403 Forbidden - Authenticated but not authorized
if (!User.HasPermission("products.delete"))
    return Forbid();

// 404 Not Found - Resource doesn't exist
if (product == null)
    return NotFound();

// 409 Conflict - Business rule violation
if (await _context.Products.AnyAsync(p => p.Sku == sku))
    return Conflict(new { message = "SKU already exists" });

// 422 Unprocessable Entity - Semantic error
if (request.Price < 0)
    return UnprocessableEntity(new { message = "Price cannot be negative" });
```

### Server Error Codes (5xx)
```csharp
// 500 Internal Server Error - Unhandled exception (logged automatically)
// 503 Service Unavailable - Dependency unavailable

if (!await _pacService.IsAvailableAsync())
    return StatusCode(503, new { message = "CFDI service temporarily unavailable" });
```

## Request/Response Patterns

### DTOs (Data Transfer Objects)
```csharp
// ✅ CORRECT - Separate DTOs for requests and responses
public record CreateProductRequest(
    string Name,
    string Sku,
    decimal Price,
    Guid CategoryId);

public record UpdateProductRequest(
    string Name,
    decimal Price,
    bool IsActive);

public record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    string CategoryName,
    bool IsActive,
    DateTime CreatedAt);

// ❌ WRONG - Using domain entity directly
[HttpPost]
public async Task<ActionResult<Product>> Create(Product product) // DON'T!
```

### Pagination
```csharp
// Request parameters
public record PaginationParams(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,
    bool SortDescending = false);

// Response wrapper
public record PagedResult<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

// Usage
[HttpGet]
public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
    [FromQuery] PaginationParams pagination)
{
    var query = _context.Products.AsQueryable();

    var totalCount = await query.CountAsync();

    var items = await query
        .OrderBy(p => p.Name)
        .Skip((pagination.Page - 1) * pagination.PageSize)
        .Take(pagination.PageSize)
        .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
        .ToListAsync();

    return Ok(new PagedResult<ProductDto>(
        items,
        pagination.Page,
        pagination.PageSize,
        totalCount,
        (int)Math.Ceiling(totalCount / (double)pagination.PageSize)));
}
```

### Filtering and Search
```csharp
[HttpGet]
public async Task<ActionResult<List<ProductDto>>> GetProducts(
    [FromQuery] string? search = null,
    [FromQuery] Guid? categoryId = null,
    [FromQuery] bool? isActive = null,
    [FromQuery] decimal? minPrice = null,
    [FromQuery] decimal? maxPrice = null)
{
    var query = _context.Products.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(p =>
            p.Name.Contains(search) ||
            p.Sku.Contains(search));

    if (categoryId.HasValue)
        query = query.Where(p => p.CategoryId == categoryId.Value);

    if (isActive.HasValue)
        query = query.Where(p => p.IsActive == isActive.Value);

    if (minPrice.HasValue)
        query = query.Where(p => p.Price >= minPrice.Value);

    if (maxPrice.HasValue)
        query = query.Where(p => p.Price <= maxPrice.Value);

    var products = await query
        .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
        .ToListAsync();

    return Ok(products);
}
```

## Validation

### FluentValidation
```csharp
// Request validator
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters")
            .MustAsync(async (sku, cancellation) =>
                !await context.Products.AnyAsync(p => p.Sku == sku, cancellation))
            .WithMessage("SKU already exists");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required")
            .MustAsync(async (categoryId, cancellation) =>
                await context.ProductCategories.AnyAsync(c => c.Id == categoryId, cancellation))
            .WithMessage("Category does not exist");
    }
}

// Controller
[HttpPost]
public async Task<ActionResult<ProductDto>> CreateProduct(
    [FromBody] CreateProductRequest request)
{
    // Validation happens automatically via middleware
    var command = new CreateProductCommand(request.Name, request.Sku, request.Price, request.CategoryId);
    var result = await _mediator.Send(command);

    return result.Match<ActionResult<ProductDto>>(
        success => CreatedAtAction(nameof(GetProduct), new { id = success.Id }, success),
        errors => BadRequest(errors));
}
```

### Model State Validation
```csharp
// Automatic validation filter
public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}

// Register globally
services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelStateAttribute>();
});
```

## Error Handling

### Problem Details (RFC 7807)
```csharp
// Standard error response
public class ErrorResponse : ProblemDetails
{
    public ErrorResponse(string message, int statusCode = 400)
    {
        Title = "Validation Error";
        Detail = message;
        Status = statusCode;
        Type = "https://httpstatuses.com/" + statusCode;
    }

    public Dictionary<string, string[]>? Errors { get; set; }
}

// Usage
[HttpPost]
public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductRequest request)
{
    var result = await _mediator.Send(new CreateProductCommand(request));

    return result.Match<ActionResult<ProductDto>>(
        success => CreatedAtAction(nameof(GetProduct), new { id = success.Id }, success),
        errors => BadRequest(new ErrorResponse("Validation failed")
        {
            Errors = errors.ToDictionary(e => e.PropertyName, e => new[] { e.ErrorMessage })
        }));
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
        _logger.LogError(exception, "An unhandled exception occurred");

        var problemDetails = exception switch
        {
            ValidationException validationException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = validationException.Message
            },
            NotFoundException notFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = notFoundException.Message
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = "You do not have permission to access this resource"
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An error occurred processing your request"
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? 500;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
```

## Versioning

### URL Path Versioning
```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsV1Controller : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        // V1 implementation
    }
}

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsV2Controller : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDtoV2>>> GetProducts(
        [FromQuery] PaginationParams pagination)
    {
        // V2 implementation with pagination
    }
}

// Startup configuration
services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
```

## Authentication & Authorization

### JWT Bearer Token
```csharp
[Authorize]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    // Requires authentication

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        // All authenticated users can view products
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductRequest request)
    {
        // Only Admin or Manager can create products
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CanDeleteProducts")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        // Custom policy
    }
}

// Allow anonymous access to specific endpoint
[AllowAnonymous]
[HttpGet("public")]
public async Task<ActionResult<List<ProductDto>>> GetPublicProducts()
{
    // No authentication required
}
```

### Custom Authorization
```csharp
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.HasClaim("permission", requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

// Usage
[Authorize(Policy = "products.delete")]
[HttpDelete("{id}")]
public async Task<ActionResult> DeleteProduct(Guid id)
```

## CORS

### Configuration
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7001",
                "https://app.corelio.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

app.UseCors("AllowBlazorClient");
```

## Rate Limiting

### Configuration
```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

app.UseRateLimiter();

// Per-endpoint
[EnableRateLimiting("fixed")]
[HttpPost]
public async Task<ActionResult> CreateProduct(CreateProductRequest request)
```

## Swagger/OpenAPI Documentation

### Configuration
```csharp
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Corelio API",
        Version = "v1",
        Description = "Multi-tenant ERP API with CFDI compliance",
        Contact = new OpenApiContact
        {
            Name = "Corelio Support",
            Email = "support@corelio.com"
        }
    });

    // JWT authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
```

### XML Documentation
```csharp
/// <summary>
/// Creates a new product
/// </summary>
/// <param name="request">Product creation details</param>
/// <returns>The created product</returns>
/// <response code="201">Product created successfully</response>
/// <response code="400">Invalid request data</response>
/// <response code="409">SKU already exists</response>
[HttpPost]
[ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
public async Task<ActionResult<ProductDto>> CreateProduct(
    [FromBody] CreateProductRequest request)
```

## Best Practices Checklist

### Endpoint Design
- [ ] Use plural nouns for resource names
- [ ] Use kebab-case for multi-word names
- [ ] Use proper HTTP methods (GET, POST, PUT, PATCH, DELETE)
- [ ] Return appropriate status codes
- [ ] Version your API

### Request/Response
- [ ] Use DTOs (never expose domain entities)
- [ ] Implement pagination for list endpoints
- [ ] Support filtering and sorting
- [ ] Use meaningful property names (camelCase in JSON)
- [ ] Include timestamps in UTC

### Validation
- [ ] Use FluentValidation for complex rules
- [ ] Validate at the API boundary
- [ ] Return detailed error messages
- [ ] Use Problem Details (RFC 7807) format

### Security
- [ ] Require authentication by default
- [ ] Implement authorization checks
- [ ] Never accept tenant_id from client
- [ ] Sanitize user inputs
- [ ] Use HTTPS only
- [ ] Implement rate limiting
- [ ] Configure CORS properly

### Documentation
- [ ] Add XML comments to all endpoints
- [ ] Document expected status codes
- [ ] Provide request/response examples
- [ ] Keep Swagger documentation up to date

---

**Remember**: Consistency across endpoints improves developer experience!
