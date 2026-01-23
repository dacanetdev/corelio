using FluentValidation;

namespace Corelio.Application.Products.Queries.SearchProducts;

/// <summary>
/// Validator for the SearchProductsQuery.
/// </summary>
public class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Search query is required.")
            .MinimumLength(1).WithMessage("Search query must be at least 1 character.")
            .MaximumLength(200).WithMessage("Search query must not exceed 200 characters.");

        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Limit must not exceed 100.");
    }
}
