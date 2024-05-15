namespace TrackHubRouter.Application.Categories.Queries.GetCategory;

public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
{
    public GetCategoryQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category Id is required.");
    }
}
