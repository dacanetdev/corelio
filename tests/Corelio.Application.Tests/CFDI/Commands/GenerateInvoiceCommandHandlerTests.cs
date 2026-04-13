using Corelio.Application.CFDI.Commands.GenerateInvoice;
using Corelio.Application.Common.Interfaces;
using Corelio.Application.Common.Models;
using Corelio.Domain.Entities;
using Corelio.Domain.Entities.CFDI;
using Corelio.Domain.Enums;
using Corelio.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Corelio.Application.Tests.CFDI.Commands;

[Trait("Category", "Unit")]
public class GenerateInvoiceCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<ITenantConfigurationRepository> _tenantConfigRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly GenerateInvoiceCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public GenerateInvoiceCommandHandlerTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _tenantConfigRepositoryMock = new Mock<ITenantConfigurationRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantServiceMock = new Mock<ITenantService>();

        _handler = new GenerateInvoiceCommandHandler(
            _saleRepositoryMock.Object,
            _invoiceRepositoryMock.Object,
            _tenantConfigRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _tenantServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCompletedSale_CreatesInvoiceInDraftStatus()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateCompletedSale(saleId, _tenantId, withRfc: "XAXX010101000");
        var config = CreateValidConfig(_tenantId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _invoiceRepositoryMock.Setup(x => x.GetBySaleIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        _tenantConfigRepositoryMock.Setup(x => x.GetByTenantIdAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        Invoice? capturedInvoice = null;
        _invoiceRepositoryMock.Setup(x => x.Add(It.IsAny<Invoice>()))
            .Callback<Invoice>(inv => capturedInvoice = inv);

        var command = new GenerateInvoiceCommand(saleId, ReceiverCfdiUse: "G01");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        capturedInvoice.Should().NotBeNull();
        capturedInvoice!.Status.Should().Be(CfdiStatus.Draft);
        capturedInvoice.InvoiceType.Should().Be(CfdiType.Ingreso);
        capturedInvoice.TenantId.Should().Be(_tenantId);
        capturedInvoice.SaleId.Should().Be(saleId);
        capturedInvoice.ReceiverCfdiUse.Should().Be("G01");
        capturedInvoice.Folio.Should().Be("F-00001"); // First folio (CfdiNextFolio=1)
        capturedInvoice.IssuerRfc.Should().Be(config.IssuerRfc);
        capturedInvoice.ReceiverRfc.Should().Be("XAXX010101000");

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCashPayment_SetsCashPaymentFormCode()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateCompletedSale(saleId, _tenantId, withRfc: "XAXX010101000");
        sale.Payments.Add(new Payment { Method = PaymentMethod.Cash, Amount = sale.Total });
        var config = CreateValidConfig(_tenantId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _invoiceRepositoryMock.Setup(x => x.GetBySaleIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        _tenantConfigRepositoryMock.Setup(x => x.GetByTenantIdAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        Invoice? capturedInvoice = null;
        _invoiceRepositoryMock.Setup(x => x.Add(It.IsAny<Invoice>()))
            .Callback<Invoice>(inv => capturedInvoice = inv);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedInvoice!.PaymentForm.Should().Be("01"); // Cash
        capturedInvoice.PaymentMethod.Should().Be("PUE");
    }

    [Fact]
    public async Task Handle_WithCardPayment_SetsCardPaymentFormCode()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateCompletedSale(saleId, _tenantId, withRfc: "XAXX010101000");
        sale.Payments.Add(new Payment { Method = PaymentMethod.Card, Amount = sale.Total });
        var config = CreateValidConfig(_tenantId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _invoiceRepositoryMock.Setup(x => x.GetBySaleIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        _tenantConfigRepositoryMock.Setup(x => x.GetByTenantIdAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        Invoice? capturedInvoice = null;
        _invoiceRepositoryMock.Setup(x => x.Add(It.IsAny<Invoice>()))
            .Callback<Invoice>(inv => capturedInvoice = inv);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedInvoice!.PaymentForm.Should().Be("04"); // Card
    }

    [Fact]
    public async Task Handle_WhenTenantNotResolved_ReturnsUnauthorizedError()
    {
        // Arrange
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns((Guid?)null);

        var command = new GenerateInvoiceCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Tenant.NotResolved");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        _saleRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSaleNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenSaleNotCompleted_ReturnsValidationError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            TenantId = _tenantId,
            Status = SaleStatus.Draft, // Not completed
            Folio = "V-00001"
        };

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Sale.NotCompleted");
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_WhenSaleHasNoCustomer_ReturnsValidationError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            TenantId = _tenantId,
            Status = SaleStatus.Completed,
            Folio = "V-00001",
            Customer = null // No customer
        };

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.CustomerRequired");
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_WhenCustomerHasNoRfc_ReturnsValidationError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateCompletedSale(saleId, _tenantId, withRfc: null); // No RFC

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.RfcRequired");
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_WhenActiveInvoiceAlreadyExists_ReturnsConflictError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateCompletedSale(saleId, _tenantId, withRfc: "XAXX010101000");
        var existingInvoice = new Invoice { Id = Guid.NewGuid(), Status = CfdiStatus.Stamped };

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _invoiceRepositoryMock.Setup(x => x.GetBySaleIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingInvoice);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.AlreadyExists");
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_WhenExistingInvoiceIsCancelled_AllowsNewInvoice()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateCompletedSale(saleId, _tenantId, withRfc: "XAXX010101000");
        var cancelledInvoice = new Invoice { Id = Guid.NewGuid(), Status = CfdiStatus.Cancelled };
        var config = CreateValidConfig(_tenantId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _invoiceRepositoryMock.Setup(x => x.GetBySaleIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cancelledInvoice); // Cancelled → allow new invoice
        _tenantConfigRepositoryMock.Setup(x => x.GetByTenantIdAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenConfigNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateCompletedSale(saleId, _tenantId, withRfc: "XAXX010101000");

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _invoiceRepositoryMock.Setup(x => x.GetBySaleIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        _tenantConfigRepositoryMock.Setup(x => x.GetByTenantIdAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantConfiguration?)null);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.ConfigNotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenConfigIncomplete_ReturnsValidationError()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateCompletedSale(saleId, _tenantId, withRfc: "XAXX010101000");
        var incompleteConfig = new TenantConfiguration
        {
            TenantId = _tenantId,
            IssuerRfc = "ABC123456789", // Has RFC
            IssuerName = null,          // Missing name — incomplete
            IssuerTaxRegime = "601",
            IssuerPostalCode = "06600"
        };

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _invoiceRepositoryMock.Setup(x => x.GetBySaleIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        _tenantConfigRepositoryMock.Setup(x => x.GetByTenantIdAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(incompleteConfig);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Invoice.IssuerConfigIncomplete");
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_WithMultipleItems_MapsAllItemsToInvoice()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = CreateCompletedSale(saleId, _tenantId, withRfc: "XAXX010101000");
        sale.Items.Add(new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Producto B", Quantity = 2, UnitPrice = 50m, TaxPercentage = 16m });
        var config = CreateValidConfig(_tenantId);

        _tenantServiceMock.Setup(x => x.GetCurrentTenantId()).Returns(_tenantId);
        _saleRepositoryMock.Setup(x => x.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);
        _invoiceRepositoryMock.Setup(x => x.GetBySaleIdAsync(saleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        _tenantConfigRepositoryMock.Setup(x => x.GetByTenantIdAsync(_tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        Invoice? capturedInvoice = null;
        _invoiceRepositoryMock.Setup(x => x.Add(It.IsAny<Invoice>()))
            .Callback<Invoice>(inv => capturedInvoice = inv);

        var command = new GenerateInvoiceCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedInvoice!.Items.Should().HaveCount(2); // Original + added item
        capturedInvoice.Items.Should().AllSatisfy(item =>
        {
            item.TenantId.Should().Be(_tenantId);
            item.ItemNumber.Should().BeGreaterThan(0);
        });
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private static Sale CreateCompletedSale(Guid saleId, Guid tenantId, string? withRfc)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            FirstName = "Juan",
            LastName = "García",
            Rfc = withRfc
        };

        return new Sale
        {
            Id = saleId,
            TenantId = tenantId,
            Folio = "V-00001",
            Status = SaleStatus.Completed,
            Customer = customer,
            CustomerId = customer.Id,
            SubTotal = 100m,
            DiscountAmount = 0m,
            TaxAmount = 16m,
            Total = 116m,
            Items = [new SaleItem
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                ProductName = "Producto A",
                Quantity = 1,
                UnitPrice = 100m,
                TaxPercentage = 16m,
                DiscountPercentage = 0m
            }],
            Payments = []
        };
    }

    private static TenantConfiguration CreateValidConfig(Guid tenantId) =>
        new()
        {
            TenantId = tenantId,
            IssuerRfc = "ABC123456789",
            IssuerName = "Ferretería ABC S.A. de C.V.",
            IssuerTaxRegime = "601",
            IssuerPostalCode = "06600",
            CfdiSeries = "A",
            CfdiNextFolio = 1
        };
}
