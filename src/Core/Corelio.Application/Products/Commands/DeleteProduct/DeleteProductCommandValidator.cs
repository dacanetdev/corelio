using FluentValidation;

namespace Corelio.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Validator for the DeleteProductCommand.
/// </summary>
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");
    }
}
