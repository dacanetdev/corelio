using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using Corelio.SharedKernel.Messaging;

namespace Corelio.Application.CFDI.Commands.GenerateInvoice;

/// <summary>
/// Creates a Draft CFDI invoice from a completed sale.
/// Validates issuer config, customer RFC, maps sale items to CFDI concepts.
/// </summary>
public class GenerateInvoiceCommandHandler(
    ISaleRepository saleRepository,
    IInvoiceRepository invoiceRepository,
    ITenantConfigurationRepository tenantConfigRepository,
    IUnitOfWork unitOfWork,
    ITenantService tenantService) : IRequestHandler<GenerateInvoiceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        if (!tenantId.HasValue)
        {
            return Result<Guid>.Failure(
                new Error("Tenant.NotResolved", "Unable to resolve current tenant.", ErrorType.Unauthorized));
        }

        // Load the sale with items, payments, and customer
        var sale = await saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale is null)
        {
            return Result<Guid>.Failure(
                new Error("Sale.NotFound", $"Sale '{request.SaleId}' not found.", ErrorType.NotFound));
        }

        if (sale.Status != SaleStatus.Completed)
        {
            return Result<Guid>.Failure(
                new Error("Sale.NotCompleted", "Only completed sales can be invoiced.", ErrorType.Validation));
        }

        // Validate receiver (customer must have RFC)
        if (sale.Customer is null)
        {
            return Result<Guid>.Failure(
                new Error("Invoice.CustomerRequired", "A customer with RFC is required to generate an invoice.", ErrorType.Validation));
        }

        if (string.IsNullOrWhiteSpace(sale.Customer.Rfc))
        {
            return Result<Guid>.Failure(
                new Error("Invoice.RfcRequired", "Customer RFC is required to generate a CFDI invoice.", ErrorType.Validation));
        }

        // Check for existing invoice on this sale
        var existingInvoice = await invoiceRepository.GetBySaleIdAsync(request.SaleId, cancellationToken);
        if (existingInvoice is not null && existingInvoice.Status != CfdiStatus.Cancelled)
        {
            return Result<Guid>.Failure(
                new Error("Invoice.AlreadyExists", "An active invoice already exists for this sale.", ErrorType.Conflict));
        }

        // Load tenant CFDI configuration
        var config = await tenantConfigRepository.GetByTenantIdAsync(tenantId.Value, cancellationToken);
        if (config is null)
        {
            return Result<Guid>.Failure(
                new Error("Invoice.ConfigNotFound", "Tenant CFDI configuration not found.", ErrorType.NotFound));
        }

        if (string.IsNullOrWhiteSpace(config.IssuerRfc) ||
            string.IsNullOrWhiteSpace(config.IssuerName) ||
            string.IsNullOrWhiteSpace(config.IssuerTaxRegime) ||
            string.IsNullOrWhiteSpace(config.IssuerPostalCode))
        {
            return Result<Guid>.Failure(
                new Error("Invoice.IssuerConfigIncomplete",
                    "Issuer RFC, name, tax regime, and postal code must be configured before generating invoices.",
                    ErrorType.Validation));
        }

        // Assign folio and increment counter
        var folioNumber = config.CfdiNextFolio;
        config.CfdiNextFolio++;
        tenantConfigRepository.Update(config);

        var folio = $"F-{folioNumber:D5}";

        // Map payment form from first payment
        var paymentForm = MapPaymentForm(sale.Payments.FirstOrDefault()?.Method ?? PaymentMethod.Cash);

        // Build invoice items
        var invoiceItems = sale.Items
            .OrderBy(i => i.ProductName)
            .Select((item, index) => new InvoiceItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId.Value,
                ItemNumber = index + 1,
                ProductId = item.ProductId,
                ProductKey = "25171500", // Generic hardware SAT code; configurable per product in future
                UnitKey = "H87",         // Piece
                Description = item.ProductName,
                Quantity = item.Quantity,
                UnitValue = item.UnitPrice,
                Amount = item.UnitPrice * item.Quantity,
                Discount = item.UnitPrice * item.Quantity * (item.DiscountPercentage / 100),
                TaxObject = item.TaxPercentage > 0 ? "02" : "01",
                TaxRate = item.TaxPercentage / 100,
                TaxAmount = item.UnitPrice * item.Quantity * (item.TaxPercentage / 100)
            })
            .ToList();

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            SaleId = sale.Id,
            Folio = folio,
            Serie = config.CfdiSeries,
            Status = CfdiStatus.Draft,
            InvoiceType = CfdiType.Ingreso,
            // Issuer snapshot
            IssuerRfc = config.IssuerRfc,
            IssuerName = config.IssuerName,
            IssuerTaxRegime = config.IssuerTaxRegime,
            // Receiver snapshot
            ReceiverRfc = sale.Customer.Rfc,
            ReceiverName = sale.Customer.BusinessName ?? sale.Customer.FullName,
            ReceiverTaxRegime = sale.Customer.TaxRegime ?? "616",
            ReceiverPostalCode = config.IssuerPostalCode, // Use issuer postal code as expedition place
            ReceiverCfdiUse = request.ReceiverCfdiUse,
            // Amounts
            Subtotal = sale.SubTotal,
            Discount = sale.DiscountAmount,
            Total = sale.Total,
            // Payment SAT codes
            PaymentForm = paymentForm,
            PaymentMethod = "PUE",
            Items = invoiceItems
        };

        invoiceRepository.Add(invoice);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(invoice.Id);
    }

    private static string MapPaymentForm(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "01",
        PaymentMethod.Card => "04",
        PaymentMethod.Transfer => "03",
        PaymentMethod.Check => "02",
        _ => "99" // Other
    };
}
