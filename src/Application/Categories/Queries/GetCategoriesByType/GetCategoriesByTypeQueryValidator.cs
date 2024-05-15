namespace TrackHubRouter.Application.Categories.Queries.GetCategoriesByType;

public sealed class GetCategoriesByTypeQueryValidator : AbstractValidator<GetCategoriesByTypeQuery>
{
    public GetCategoriesByTypeQueryValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Category Type is required.");
    }
}
