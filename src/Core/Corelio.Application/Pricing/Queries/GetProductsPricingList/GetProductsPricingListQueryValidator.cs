using FluentValidation;

namespace Corelio.Application.Pricing.Queries.GetProductsPricingList;

/// <summary>
/// Validator for the GetProductsPricingListQuery.
/// </summary>
public class GetProductsPricingListQueryValidator : AbstractValidator<GetProductsPricingListQuery>
{
    public GetProductsPricingListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
