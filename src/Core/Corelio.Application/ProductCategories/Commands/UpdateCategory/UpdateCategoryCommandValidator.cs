using FluentValidation;

namespace Corelio.Application.ProductCategories.Commands.UpdateCategory;

/// <summary>
/// Validator for the UpdateCategoryCommand.
/// </summary>
public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(200)
            .WithMessage("Category name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("Image URL cannot exceed 500 characters.");

        RuleFor(x => x.ColorHex)
            .Matches(@"^#[0-9A-Fa-f]{6}$")
            .When(x => !string.IsNullOrEmpty(x.ColorHex))
            .WithMessage("Color must be in hex format (#RRGGBB).");

        RuleFor(x => x.IconName)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.IconName))
            .WithMessage("Icon name cannot exceed 50 characters.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Sort order cannot be negative.");
    }
}
