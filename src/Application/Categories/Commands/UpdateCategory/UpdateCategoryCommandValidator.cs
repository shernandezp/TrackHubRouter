using Common.Domain.Constants;

namespace TrackHubRouter.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(v => v.Category.Name)
            .MaximumLength(ColumnMetadata.DefaultNameLength)
            .NotEmpty();

        RuleFor(v => v.Category.Description)
            .MaximumLength(ColumnMetadata.DefaultDescriptionLength)
            .NotEmpty();
    }
}
