using Corelio.Application.Common.Models;
using Corelio.Application.Suppliers.Common;
using Corelio.Application.Suppliers.Commands.CreateSupplier;
using Corelio.Application.Suppliers.Commands.DeleteSupplier;
using Corelio.Application.Suppliers.Commands.UpdateSupplier;
using Corelio.Application.Suppliers.Queries.GetSupplierById;
using Corelio.Application.Suppliers.Queries.GetSuppliers;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Supplier management endpoints.
/// </summary>
public static class SupplierEndpoints
{
    public static IEndpointRouteBuilder MapSupplierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/suppliers")
            .WithTags("Suppliers")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/", GetSuppliers)
            .WithName("GetSuppliers")
            .WithSummary("Get paged list of suppliers")
            .WithDescription("Returns a paginated list of suppliers for the current tenant. Supports a free-text `search` parameter that matches against name, RFC, contact name, or email. Optionally filter by `isActive` status. Results default to page 1, 20 items per page.")
            .Produces<PagedResult<SupplierListDto>>(200)
            .ProducesProblem(400)
            .RequireAuthorization("suppliers.view");

        group.MapGet("/{id:guid}", GetSupplierById)
            .WithName("GetSupplierById")
            .WithSummary("Get supplier by ID")
            .WithDescription("Returns the complete supplier record including contact details, address, payment terms, and tax regime. Returns 404 if the supplier does not exist or belongs to a different tenant.")
            .Produces<SupplierDto>(200)
            .Produces(404)
            .RequireAuthorization("suppliers.view");

        group.MapPost("/", CreateSupplier)
            .WithName("CreateSupplier")
            .WithSummary("Create a new supplier")
            .WithDescription("Creates a new supplier in the current tenant. RFC must be unique per tenant when provided. Returns 201 Created with the new supplier ID.")
            .Produces<object>(201)
            .ProducesProblem(400)
            .RequireAuthorization("suppliers.create");

        group.MapPut("/{id:guid}", UpdateSupplier)
            .WithName("UpdateSupplier")
            .WithSummary("Update an existing supplier")
            .WithDescription("Updates all mutable fields of an existing supplier record. Returns 204 No Content on success. Returns 404 if the supplier is not found.")
            .Produces(204)
            .Produces(404)
            .ProducesProblem(400)
            .RequireAuthorization("suppliers.update");

        group.MapDelete("/{id:guid}", DeleteSupplier)
            .WithName("DeleteSupplier")
            .WithSummary("Soft-delete a supplier")
            .WithDescription("Soft-deletes a supplier by marking them as deleted. The supplier record is retained for historical purchase order data. Returns 204 No Content on success.")
            .Produces(204)
            .Produces(404)
            .RequireAuthorization("suppliers.delete");

        return app;
    }

    private static async Task<IResult> GetSuppliers(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetSuppliersQuery(
            Page: pageNumber > 0 ? pageNumber : 1,
            Size: pageSize > 0 ? pageSize : 20,
            Search: search,
            IsActive: isActive);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetSupplierById(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetSupplierByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreateSupplier(
        [FromBody] CreateSupplierRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreateSupplierCommand(
            request.Name,
            request.Rfc,
            request.ContactName,
            request.Email,
            request.Phone,
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.PaymentTermsDays,
            request.TaxRegime,
            request.Notes);

        var result = await mediator.Send(command, ct);
        return result.ToCreatedResult(
            $"/api/v1/suppliers/{result.Value}",
            new { supplierId = result.Value });
    }

    private static async Task<IResult> UpdateSupplier(
        Guid id,
        [FromBody] UpdateSupplierRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateSupplierCommand(
            id,
            request.Name,
            request.Rfc,
            request.ContactName,
            request.Email,
            request.Phone,
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.PaymentTermsDays,
            request.TaxRegime,
            request.Notes,
            request.IsActive);

        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> DeleteSupplier(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new DeleteSupplierCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }
}

/// <summary>Request body for creating a supplier.</summary>
public record CreateSupplierRequest(
    string Name,
    string? Rfc,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Street,
    string? City,
    string? State,
    string? ZipCode,
    int PaymentTermsDays,
    string? TaxRegime,
    string? Notes);

/// <summary>Request body for updating a supplier.</summary>
public record UpdateSupplierRequest(
    string Name,
    string? Rfc,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Street,
    string? City,
    string? State,
    string? ZipCode,
    int PaymentTermsDays,
    string? TaxRegime,
    string? Notes,
    bool IsActive);
