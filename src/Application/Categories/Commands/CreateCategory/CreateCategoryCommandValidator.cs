using Common.Domain.Constants;
using TrackHubRouter.Application.Categories.Commands.CreateCategory;

namespace TrackHubRouter.Application.TodoItems.Commands.CreateTodoItem;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(v => v.Category.Name)
            .MaximumLength(ColumnMetadata.DefaultNameLength)
            .NotEmpty();

        RuleFor(v => v.Category.Description)
            .MaximumLength(ColumnMetadata.DefaultDescriptionLength)
            .NotEmpty();
    }
}
