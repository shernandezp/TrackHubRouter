using Common.Application.Attributes;
using Common.Domain.Constants;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Categories.Queries.GetCategory;

[Authorize(Resource = Resources.MapScreen, Action = Actions.View)]
public readonly record struct GetCategoryQuery : IRequest<CategoryVm>
{
    public required Guid Id { get; init; }
}

public class GetCategoryQueryHandler(ICategoryReader reader) : IRequestHandler<GetCategoryQuery, CategoryVm>
{

    public async Task<CategoryVm> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        => await reader.GetCategoryAsync(request.Id, cancellationToken);

}
