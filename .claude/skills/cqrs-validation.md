---
name: cqrs-validation
description: Ensures proper CQRS separation with MediatR commands and queries
---

# CQRS Validation

## Command vs Query Separation

### Commands (Write Operations)
```csharp
// ✅ COMMAND - Changes state, returns Result<T>
public record CreateProductCommand(
    string Name,
    string Sku,
    decimal Price,
    Guid CategoryId) : IRequest<Result<ProductDto>>;

public class CreateProductCommandHandler(
    IApplicationDbContext context,
    ITenantService tenantService,
    IMapper mapper)
    : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Validation
        var exists = await context.Products
            .AnyAsync(p => p.Sku == request.Sku, cancellationToken);

        if (exists)
            return Result<ProductDto>.Failure("SKU already exists");

        // Create entity
        var product = new Product
        {
            Name = request.Name,
            Sku = request.Sku,
            Price = request.Price,
            CategoryId = request.CategoryId,
            TenantId = tenantService.GetCurrentTenantId()
        };

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<ProductDto>(product);
        return Result<ProductDto>.Success(dto);
    }
}
```

### Queries (Read Operations)
```csharp
// ✅ QUERY - Reads data, returns DTO
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

public class GetProductByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Query filter automatically applies tenant isolation
        var product = await context.Products
            .AsNoTracking() // Read-only!
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return Result<ProductDto>.Failure("Product not found");

        var dto = mapper.Map<ProductDto>(product);
        return Result<ProductDto>.Success(dto);
    }
}
```

## CQRS Rules

### ✅ ALWAYS

1. **Commands change state, Queries read state**
```csharp
// ✅ CORRECT
public record CreateProductCommand(...) : IRequest<Result<ProductDto>>;
public record GetProductsQuery(...) : IRequest<Result<List<ProductDto>>>;

// ❌ WRONG - Query should not change state
public record GetProductsQuery(...) : IRequest<Result<List<ProductDto>>>;

public class GetProductsQueryHandler
{
    public async Task<Result<List<ProductDto>>> Handle(...)
    {
        await context.SaveChangesAsync(); // DON'T change state in query!
    }
}
```

2. **Queries use AsNoTracking()**
```csharp
// ✅ CORRECT - Read-only query
public class GetProductsQueryHandler
{
    public async Task<Result<List<ProductDto>>> Handle(...)
    {
        var products = await context.Products
            .AsNoTracking() // Don't track for read-only
            .ToListAsync(cancellationToken);
    }
}

// ❌ WRONG - Tracking unnecessary for reads
public class GetProductsQueryHandler
{
    public async Task<Result<List<ProductDto>>> Handle(...)
    {
        var products = await context.Products
            .ToListAsync(cancellationToken); // Will track changes unnecessarily
    }
}
```

3. **Queries project to DTOs**
```csharp
// ✅ CORRECT - Project to DTO
public class GetProductsQueryHandler
{
    public async Task<Result<List<ProductDto>>> Handle(...)
    {
        var products = await context.Products
            .AsNoTracking()
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<ProductDto>>.Success(products);
    }
}

// ❌ WRONG - Returning domain entities
public record GetProductsQuery : IRequest<Result<List<Product>>>; // DON'T!
```

4. **Commands return Result<T>**
```csharp
// ✅ CORRECT - Return Result<T>
public record CreateProductCommand(...) : IRequest<Result<ProductDto>>;

// ❌ WRONG - Throwing exceptions for business errors
public record CreateProductCommand(...) : IRequest<ProductDto>;

public class CreateProductCommandHandler
{
    public async Task<ProductDto> Handle(...)
    {
        if (exists)
            throw new DuplicateSkuException(); // DON'T!
    }
}
```

### ❌ NEVER

1. **Don't mix commands and queries**
```csharp
// ❌ WRONG - Command that also queries
public record CreateAndGetProductsCommand(...) : IRequest<Result<List<ProductDto>>>;

// ✅ CORRECT - Separate commands and queries
public record CreateProductCommand(...) : IRequest<Result<ProductDto>>;
public record GetProductsQuery(...) : IRequest<Result<List<ProductDto>>>;

// Controller
[HttpPost]
public async Task<ActionResult<ProductDto>> CreateProduct(...)
{
    var createResult = await mediator.Send(new CreateProductCommand(...));
    if (createResult.IsFailure)
        return BadRequest(createResult.Errors);

    // Separate query if needed
    var product = createResult.Value;
    return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
}
```

2. **Don't query inside commands (for data to modify)**
```csharp
// ❌ WRONG - Querying just to get data to modify
public class UpdateProductCommandHandler
{
    public async Task<Result<ProductDto>> Handle(...)
    {
        // Don't do this!
        var product = await context.Products
            .AsNoTracking() // Then why query?
            .FirstOrDefaultAsync(...);

        if (product != null)
        {
            product.Name = request.Name; // Can't modify, it's not tracked!
        }
    }
}

// ✅ CORRECT - Query for modification
public class UpdateProductCommandHandler
{
    public async Task<Result<ProductDto>> Handle(...)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return Result<ProductDto>.Failure("Product not found");

        product.Name = request.Name;
        product.Price = request.Price;

        await context.SaveChangesAsync(cancellationToken);
    }
}
```

3. **Don't call SaveChanges in queries**
```csharp
// ❌ WRONG - Saving in query
public class GetProductsQueryHandler
{
    public async Task<Result<List<ProductDto>>> Handle(...)
    {
        var products = await context.Products.ToListAsync();

        // Update last accessed timestamp - DON'T!
        foreach (var product in products)
        {
            product.LastAccessed = DateTime.UtcNow;
        }
        await context.SaveChangesAsync();

        return Result<List<ProductDto>>.Success(...);
    }
}

// ✅ CORRECT - Use a command for state changes
public record UpdateLastAccessedCommand(Guid ProductId) : IRequest<Result>;
```

## Command Patterns

### Create Command
```csharp
public record CreateProductCommand(
    string Name,
    string Sku,
    decimal Price,
    Guid CategoryId) : IRequest<Result<ProductDto>>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (sku, ct) =>
                !await context.Products.AnyAsync(p => p.Sku == sku, ct))
            .WithMessage("SKU already exists");
    }
}

public class CreateProductCommandHandler(
    IApplicationDbContext context,
    ITenantService tenantService,
    IMapper mapper)
    : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Sku = request.Sku,
            Price = request.Price,
            CategoryId = request.CategoryId,
            TenantId = tenantService.GetCurrentTenantId()
        };

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Success(mapper.Map<ProductDto>(product));
    }
}
```

### Update Command
```csharp
public record UpdateProductCommand(
    Guid Id,
    string Name,
    decimal Price,
    bool IsActive) : IRequest<Result<ProductDto>>;

public class UpdateProductCommandHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return Result<ProductDto>.Failure("Product not found");

        // Update properties
        product.Name = request.Name;
        product.Price = request.Price;
        product.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Success(mapper.Map<ProductDto>(product));
    }
}
```

### Delete Command
```csharp
public record DeleteProductCommand(Guid Id) : IRequest<Result>;

public class DeleteProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return Result.Failure("Product not found");

        // Soft delete or hard delete
        product.IsDeleted = true;
        // OR: context.Products.Remove(product);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
```

## Query Patterns

### Get by ID
```csharp
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

public class GetProductByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await context.Products
            .AsNoTracking()
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        return product != null
            ? Result<ProductDto>.Success(product)
            : Result<ProductDto>.Failure("Product not found");
    }
}
```

### Get All (with filtering)
```csharp
public record GetProductsQuery(
    string? SearchTerm,
    Guid? CategoryId,
    bool? IsActive,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<ProductDto>>>;

public class GetProductsQueryHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductDto>>>
{
    public async Task<Result<PagedResult<ProductDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Products.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(request.SearchTerm) ||
                p.Sku.Contains(request.SearchTerm));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive);
        }

        // Count before paging
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply paging and project
        var items = await query
            .OrderBy(p => p.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var result = new PagedResult<ProductDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)request.PageSize));

        return Result<PagedResult<ProductDto>>.Success(result);
    }
}
```

## MediatR Pipeline Behaviors

### Logging Behavior
```csharp
public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation(
            "Handling {RequestName}",
            requestName);

        var response = await next();

        logger.LogInformation(
            "Handled {RequestName}",
            requestName);

        return response;
    }
}
```

### Validation Behavior
```csharp
public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures.ToDictionary(
                f => f.PropertyName,
                f => new[] { f.ErrorMessage }));
        }

        return await next();
    }
}
```

### Performance Behavior
```csharp
public class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            logger.LogWarning(
                "Slow request: {RequestName} took {ElapsedMs}ms",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds);
        }

        return response;
    }
}
```

## Controller Integration

```csharp
[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] GetProductsQuery query)
    {
        var result = await mediator.Send(query);

        return result.Match<ActionResult<PagedResult<ProductDto>>>(
            success => Ok(success),
            errors => BadRequest(new { errors }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));

        return result.Match<ActionResult<ProductDto>>(
            success => Ok(success),
            errors => NotFound(new { errors }));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductCommand command)
    {
        var result = await mediator.Send(command);

        return result.Match<ActionResult<ProductDto>>(
            success => CreatedAtAction(
                nameof(GetProduct),
                new { id = success.Id },
                success),
            errors => BadRequest(new { errors }));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Price,
            request.IsActive);

        var result = await mediator.Send(command);

        return result.Match<ActionResult<ProductDto>>(
            success => Ok(success),
            errors => NotFound(new { errors }));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        var result = await mediator.Send(new DeleteProductCommand(id));

        return result.Match<ActionResult>(
            _ => NoContent(),
            errors => NotFound(new { errors }));
    }
}
```

## CQRS Checklist

### Commands
- [ ] Name ends with "Command"
- [ ] Returns `Result<T>` or `Result`
- [ ] Changes state (insert, update, delete)
- [ ] Calls SaveChangesAsync
- [ ] Has validator (FluentValidation)
- [ ] Sets TenantId from ITenantService
- [ ] Logs important operations

### Queries
- [ ] Name ends with "Query"
- [ ] Returns `Result<TDto>` (never domain entities)
- [ ] Uses AsNoTracking()
- [ ] Projects to DTO with AutoMapper
- [ ] Does NOT call SaveChangesAsync
- [ ] Does NOT modify entities
- [ ] Applies proper filtering

### Handlers
- [ ] One handler per command/query
- [ ] Uses primary constructors for DI
- [ ] Handles errors with Result<T>
- [ ] Includes cancellation token
- [ ] Uses async/await properly

---

**Remember**: Commands change state, Queries read state - never mix them!
