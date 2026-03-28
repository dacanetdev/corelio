using Corelio.Application.Common.Interfaces;
using Corelio.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Corelio.Infrastructure.Documents;

/// <summary>
/// Generates PDF receipts using QuestPDF (Community license).
/// </summary>
internal sealed class SaleReceiptService : ISaleReceiptService
{
    static SaleReceiptService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GenerateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var document = new SaleReceiptDocument(sale);
        var bytes = document.GeneratePdf();
        return Task.FromResult(bytes);
    }
}
