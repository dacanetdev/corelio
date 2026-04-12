using Corelio.Application.CFDI.Commands.CancelInvoice;
using Corelio.Application.CFDI.Commands.GenerateInvoice;
using Corelio.Application.CFDI.Commands.StampInvoice;
using Corelio.Application.CFDI.Queries.GetInvoiceById;
using Corelio.Application.CFDI.Queries.GetInvoices;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;
using Corelio.WebAPI.Extensions;
using Corelio.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Corelio.WebAPI.Endpoints;

/// <summary>
/// CFDI invoice management endpoints.
/// </summary>
public static class CfdiEndpoints
{
    public static IEndpointRouteBuilder MapCfdiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/invoices")
            .WithTags("CFDI Invoices")
            .RequireAuthorization()
            .AddEndpointFilter<ModelBindingErrorFilter>();

        group.MapGet("/", GetInvoices)
            .WithName("GetInvoices")
            .WithSummary("Get paged list of invoices with optional filters")
            .RequireAuthorization("cfdi.view");

        group.MapGet("/{id:guid}", GetInvoiceById)
            .WithName("GetInvoiceById")
            .WithSummary("Get a CFDI invoice by ID")
            .RequireAuthorization("cfdi.view");

        group.MapPost("/generate", GenerateInvoice)
            .WithName("GenerateInvoice")
            .WithSummary("Generate a Draft CFDI invoice from a completed sale")
            .RequireAuthorization("cfdi.generate");

        group.MapPost("/{id:guid}/stamp", StampInvoice)
            .WithName("StampInvoice")
            .WithSummary("Stamp a Draft invoice via PAC and get SAT UUID")
            .RequireAuthorization("cfdi.generate");

        group.MapDelete("/{id:guid}", CancelInvoice)
            .WithName("CancelInvoice")
            .WithSummary("Cancel a Stamped invoice via PAC (72-hour window enforced)")
            .RequireAuthorization("cfdi.cancel");

        group.MapGet("/{id:guid}/xml", DownloadXml)
            .WithName("DownloadInvoiceXml")
            .WithSummary("Download the signed CFDI XML")
            .RequireAuthorization("cfdi.view");

        group.MapGet("/{id:guid}/pdf", DownloadPdf)
            .WithName("DownloadInvoicePdf")
            .WithSummary("Download the invoice PDF")
            .RequireAuthorization("cfdi.view");

        return app;
    }

    private static async Task<IResult> GetInvoices(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        [FromQuery] CfdiStatus? status,
        [FromQuery] string? searchTerm,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetInvoicesQuery(
            PageNumber: pageNumber > 0 ? pageNumber : 1,
            PageSize: pageSize > 0 ? pageSize : 20,
            Status: status,
            SearchTerm: searchTerm);

        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetInvoiceById(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetInvoiceByIdQuery(id);
        var result = await mediator.Send(query, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GenerateInvoice(
        GenerateInvoiceCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? Results.Created($"/api/v1/invoices/{result.Value}", new { id = result.Value })
            : result.Error!.ToHttpResult();
    }

    private static async Task<IResult> StampInvoice(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new StampInvoiceCommand(id);
        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CancelInvoice(
        Guid id,
        [FromQuery] string reason,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CancelInvoiceCommand(id, reason);
        var result = await mediator.Send(command, ct);
        return result.ToHttpResult();
    }

    private static async Task<IResult> DownloadXml(
        Guid id,
        IInvoiceRepository invoiceRepository,
        CancellationToken ct)
    {
        var invoice = await invoiceRepository.GetByIdAsync(id, ct);
        if (invoice is null)
        {
            return Results.NotFound();
        }

        if (string.IsNullOrEmpty(invoice.XmlContent))
        {
            return Results.Problem("Invoice has no XML content. It may not be stamped yet.", statusCode: 400);
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(invoice.XmlContent);
        return Results.File(bytes, "application/xml", $"CFDI_{invoice.Folio}.xml");
    }

    private static async Task<IResult> DownloadPdf(
        Guid id,
        IInvoiceRepository invoiceRepository,
        IInvoicePdfService pdfService,
        CancellationToken ct)
    {
        var invoice = await invoiceRepository.GetByIdAsync(id, ct);
        if (invoice is null)
        {
            return Results.NotFound();
        }

        var pdfBytes = await pdfService.GenerateAsync(invoice, ct);
        return Results.File(pdfBytes, "application/pdf", $"Factura_{invoice.Folio}.pdf");
    }
}
