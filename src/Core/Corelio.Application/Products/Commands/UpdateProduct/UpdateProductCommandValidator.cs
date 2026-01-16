using FluentValidation;

namespace Corelio.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Validator for the UpdateProductCommand.
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(100).WithMessage("SKU must not exceed 100 characters.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(300).WithMessage("Product name must not exceed 300 characters.");

        RuleFor(x => x.SalePrice)
            .GreaterThan(0).WithMessage("Sale price must be greater than 0.");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Cost price must be 0 or greater.");

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0).WithMessage("Tax rate must be 0 or greater.")
            .LessThanOrEqualTo(1).WithMessage("Tax rate must not exceed 1 (100%).");

        RuleFor(x => x.Barcode)
            .MaximumLength(100).WithMessage("Barcode must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Barcode));

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Short description must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.ShortDescription));

        RuleFor(x => x.Brand)
            .MaximumLength(100).WithMessage("Brand must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Brand));

        RuleFor(x => x.Manufacturer)
            .MaximumLength(200).WithMessage("Manufacturer must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Manufacturer));

        RuleFor(x => x.ModelNumber)
            .MaximumLength(100).WithMessage("Model number must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.ModelNumber));

        RuleFor(x => x.SatProductCode)
            .Matches(@"^\d{8}$").WithMessage("SAT product code must be 8 digits.")
            .When(x => !string.IsNullOrWhiteSpace(x.SatProductCode));

        RuleFor(x => x.SatUnitCode)
            .Matches(@"^[A-Z0-9]{2,3}$").WithMessage("SAT unit code must be 2-3 alphanumeric characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SatUnitCode));

        RuleFor(x => x.SatHazardousMaterial)
            .Length(4).WithMessage("SAT hazardous material code must be exactly 4 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.SatHazardousMaterial));

        RuleFor(x => x.MinStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level must be 0 or greater.");

        RuleFor(x => x.MaxStockLevel)
            .GreaterThan(x => x.MinStockLevel).WithMessage("Maximum stock level must be greater than minimum stock level.")
            .When(x => x.MaxStockLevel.HasValue);

        RuleFor(x => x.ReorderPoint)
            .GreaterThanOrEqualTo(0).WithMessage("Reorder point must be 0 or greater.")
            .When(x => x.ReorderPoint.HasValue);

        RuleFor(x => x.ReorderQuantity)
            .GreaterThan(0).WithMessage("Reorder quantity must be greater than 0.")
            .When(x => x.ReorderQuantity.HasValue);

        RuleFor(x => x.WeightKg)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.")
            .When(x => x.WeightKg.HasValue);

        RuleFor(x => x.LengthCm)
            .GreaterThan(0).WithMessage("Length must be greater than 0.")
            .When(x => x.LengthCm.HasValue);

        RuleFor(x => x.WidthCm)
            .GreaterThan(0).WithMessage("Width must be greater than 0.")
            .When(x => x.WidthCm.HasValue);

        RuleFor(x => x.HeightCm)
            .GreaterThan(0).WithMessage("Height must be greater than 0.")
            .When(x => x.HeightCm.HasValue);

        RuleFor(x => x.VolumeCm3)
            .GreaterThan(0).WithMessage("Volume must be greater than 0.")
            .When(x => x.VolumeCm3.HasValue);
    }
}
