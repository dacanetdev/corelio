using FluentValidation;

namespace Corelio.Application.ProductCategories.Commands.DeleteCategory;

/// <summary>
/// Validator for the DeleteCategoryCommand.
/// </summary>
public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required.");
    }
}
