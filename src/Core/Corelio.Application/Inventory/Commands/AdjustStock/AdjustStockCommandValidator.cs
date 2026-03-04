using FluentValidation;

namespace Corelio.Application.Inventory.Commands.AdjustStock;

/// <summary>
/// Validator for AdjustStockCommand.
/// </summary>
public class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    private static readonly string[] ValidReasonCodes = ["Damaged", "Lost", "Stolen", "Found", "CountCorrection", "Other"];

    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .NotEmpty().WithMessage("Inventory item ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.ReasonCode)
            .NotEmpty().WithMessage("Reason code is required.")
            .Must(r => ValidReasonCodes.Contains(r))
            .WithMessage($"Reason code must be one of: {string.Join(", ", ValidReasonCodes)}.");
    }
}
