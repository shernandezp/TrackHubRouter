using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Categories.Queries.GetCategory;

public record GetCategoryQuery : IRequest<CategoryVm>
{
    public required Guid Id { get; init; }
}

public class GetCategoryQueryHandler(ICategoryReader reader) : IRequestHandler<GetCategoryQuery, CategoryVm>
{

    public async Task<CategoryVm> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        => await reader.GetCategoryAsync(request.Id, cancellationToken);

}
