using Common.Application.Security;
using Common.Domain.Constants;
using TrackHubRouter.Application.Categories.Events;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Application.Categories.Commands.CreateCategory;

[Authorize(Roles = Roles.Administrator)]
public record CreateCategoryCommand : IRequest<CategoryVm>
{
    public required CategoryDto Category { get; set; }
}

public class CreateCategoryCommandHandler(ICategoryWriter writer, IPublisher publisher) : IRequestHandler<CreateCategoryCommand, CategoryVm>
{
    public async Task<CategoryVm> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await writer.CreateCategoryAsync(request.Category, cancellationToken);

        await publisher.Publish(new CategoryCreated.Notification(category.CategoryId), cancellationToken);

        return category;
    }
}
