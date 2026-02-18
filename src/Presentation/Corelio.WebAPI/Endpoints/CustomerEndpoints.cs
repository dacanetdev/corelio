using Corelio.Application.Customers.Commands.CreateCustomer;
using Corelio.Application.Customers.Commands.DeleteCustomer;
using Corelio.Application.Customers.Commands.UpdateCustomer;
using Corelio.Application.Customers.Queries.GetCustomerById;
using Corelio.Application.Customers.Queries.GetCustomers;
using Corelio.Application.Customers.Queries.SearchCustomers;
using Corelio.Domain.Enums;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// Customer management endpoints.
/// </summary>
public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/customers")
            .WithTags("Customers")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/", GetCustomers)
            .WithName("GetCustomers")
            .WithSummary("Get paged list of customers")
            .RequireAuthorization("customers.view");

        group.MapGet("/{id:guid}", GetCustomerById)
            .WithName("GetCustomerById")
            .WithSummary("Get customer by ID")
            .RequireAuthorization("customers.view");

        group.MapGet("/search", SearchCustomers)
            .WithName("SearchCustomers")
            .WithSummary("Search customers by name or RFC (for POS)")
            .RequireAuthorization("customers.view");

        group.MapPost("/", CreateCustomer)
            .WithName("CreateCustomer")
            .WithSummary("Create a new customer")
            .RequireAuthorization("customers.create");

        group.MapPut("/{id:guid}", UpdateCustomer)
            .WithName("UpdateCustomer")
            .WithSummary("Update an existing customer")
            .RequireAuthorization("customers.update");

        group.MapDelete("/{id:guid}", DeleteCustomer)
            .WithName("DeleteCustomer")
            .WithSummary("Soft-delete a customer")
            .RequireAuthorization("customers.delete");

        return app;
    }

    private static async Task<IResult> GetCustomers(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetCustomersQuery(
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 20,
            Search: search);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetCustomerById(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> SearchCustomers(
        [FromQuery] string q,
        IMediator mediator,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Results.Ok(Array.Empty<object>());
        }

        var query = new SearchCustomersQuery(q);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CreateCustomer(
        [FromBody] CreateCustomerRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreateCustomerCommand(
            request.CustomerType,
            request.FirstName,
            request.LastName,
            request.BusinessName,
            request.Rfc,
            request.Curp,
            request.Email,
            request.Phone,
            request.TaxRegime,
            request.CfdiUse,
            request.PreferredPaymentMethod);

        var result = await mediator.Send(command, ct);
        return result.ToCreatedResult(
            $"/api/v1/customers/{result.Value}",
            new { customerId = result.Value });
    }

    private static async Task<IResult> UpdateCustomer(
        Guid id,
        [FromBody] UpdateCustomerRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateCustomerCommand(
            id,
            request.CustomerType,
            request.FirstName,
            request.LastName,
            request.BusinessName,
            request.Rfc,
            request.Curp,
            request.Email,
            request.Phone,
            request.TaxRegime,
            request.CfdiUse,
            request.PreferredPaymentMethod);

        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> DeleteCustomer(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new DeleteCustomerCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToNoContentResult();
    }
}

/// <summary>Request body for creating a customer.</summary>
public record CreateCustomerRequest(
    CustomerType CustomerType,
    string FirstName,
    string LastName,
    string? BusinessName,
    string? Rfc,
    string? Curp,
    string? Email,
    string? Phone,
    string? TaxRegime,
    string? CfdiUse,
    PaymentMethod? PreferredPaymentMethod);

/// <summary>Request body for updating a customer.</summary>
public record UpdateCustomerRequest(
    CustomerType CustomerType,
    string FirstName,
    string LastName,
    string? BusinessName,
    string? Rfc,
    string? Curp,
    string? Email,
    string? Phone,
    string? TaxRegime,
    string? CfdiUse,
    PaymentMethod? PreferredPaymentMethod);
