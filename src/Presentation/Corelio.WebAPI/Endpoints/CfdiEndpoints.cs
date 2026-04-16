using Corelio.Application.CFDI.Commands.CancelInvoice;
using Corelio.Application.CFDI.Commands.GenerateInvoice;
using Corelio.Application.CFDI.Commands.StampInvoice;
using Corelio.Application.CFDI.Queries.GetInvoiceById;
using Corelio.Application.CFDI.Queries.GetInvoices;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using InvoiceDto = Corelio.Application.CFDI.Queries.GetInvoiceById.InvoiceDto;
using InvoiceListDto = Corelio.Application.CFDI.Queries.GetInvoices.InvoiceListDto;
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
            .WithDescription("Returns a paginated list of CFDI invoices for the current tenant. Supports filtering by status (Draft, Stamped, Cancelled) and a free-text search term matching the folio number or receiver RFC/name. Results default to page 1, 20 items per page, sorted by creation date descending.")
            .Produces<PagedResult<InvoiceListDto>>(200)
            .ProducesProblem(400)
            .RequireAuthorization("cfdi.view");

        group.MapGet("/{id:guid}", GetInvoiceById)
            .WithName("GetInvoiceById")
            .WithSummary("Get a CFDI invoice by ID")
            .WithDescription("Returns the complete CFDI invoice record including all concepts (line items), issuer and receiver data, tax breakdown, PAC stamp data (UUID, stamp date, PAC certificate), and current status. Returns 404 if the invoice is not found.")
            .Produces<InvoiceDto>(200)
            .Produces(404)
            .RequireAuthorization("cfdi.view");

        group.MapPost("/generate", GenerateInvoice)
            .WithName("GenerateInvoice")
            .WithSummary("Generate a Draft CFDI invoice from a completed sale")
            .WithDescription("Creates a Draft CFDI invoice from a completed sale. The sale must have a customer with a valid RFC. The invoice is populated with SAT product codes, unit codes, and tax rates from the product catalog. Returns 201 Created with the new invoice ID. Returns 422 if the sale is not in Completed status or has no eligible customer.")
            .Produces<object>(201)
            .ProducesProblem(400)
            .ProducesProblem(422)
            .RequireAuthorization("cfdi.generate");

        group.MapPost("/{id:guid}/stamp", StampInvoice)
            .WithName("StampInvoice")
            .WithSummary("Stamp a Draft invoice via PAC and get SAT UUID")
            .WithDescription("Sends the Draft invoice to the PAC (Proveedor Autorizado de Certificación) for digital stamping. The PAC validates the XML against SAT schemas, applies the Timbre Fiscal Digital, and returns a UUID (Folio Fiscal). On success, the invoice transitions to Stamped status and the UUID is persisted. Returns 422 if the invoice is not in Draft status or the CSD certificate is expired/missing.")
            .Produces<InvoiceDto>(200)
            .Produces(404)
            .ProducesProblem(422)
            .RequireAuthorization("cfdi.generate");

        group.MapDelete("/{id:guid}", CancelInvoice)
            .WithName("CancelInvoice")
            .WithSummary("Cancel a Stamped invoice via PAC (72-hour window enforced)")
            .WithDescription("Requests cancellation of a Stamped invoice through the PAC. Requires a SAT cancellation reason code via the `reason` query parameter (01 = errors with relation, 02 = errors without relation, 03 = operation not carried out, 04 = normative invoice). The 72-hour cancellation window is enforced — returns 422 if the window has passed.")
            .Produces(200)
            .Produces(404)
            .ProducesProblem(422)
            .RequireAuthorization("cfdi.cancel");

        group.MapGet("/{id:guid}/xml", DownloadXml)
            .WithName("DownloadInvoiceXml")
            .WithSummary("Download the signed CFDI XML")
            .WithDescription("Returns the signed CFDI 4.0 XML file as a download (Content-Type: application/xml). The XML includes the Timbre Fiscal Digital from the PAC. Only available for Stamped invoices. Returns 400 if the invoice has not been stamped yet.")
            .Produces(200)
            .Produces(404)
            .ProducesProblem(400)
            .RequireAuthorization("cfdi.view");

        group.MapGet("/{id:guid}/pdf", DownloadPdf)
            .WithName("DownloadInvoicePdf")
            .WithSummary("Download the invoice PDF")
            .WithDescription("Generates and returns the invoice representation as a PDF file (Content-Type: application/pdf), generated on-demand using QuestPDF. The PDF includes all CFDI fields, a QR code for SAT verification, and the digital seal summary. Available for all invoice statuses.")
            .Produces(200)
            .Produces(404)
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
