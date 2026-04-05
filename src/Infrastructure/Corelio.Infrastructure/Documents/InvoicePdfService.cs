using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Entities.CFDI;
using QuestPDF.Fluent;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// Generates A4 PDF for CFDI invoices using QuestPDF.
/// </summary>
public class InvoicePdfService : IInvoicePdfService
{
    public Task<byte[]> GenerateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var document = new InvoiceDocument(invoice);
        var bytes = document.GeneratePdf();
        return Task.FromResult(bytes);
    }
}
