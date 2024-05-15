using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Categories.Queries.GetCategoriesByType;

public record GetCategoriesByTypeQuery(CategoryType Type) : IRequest<IReadOnlyCollection<CategoryVm>>;

public class GetCategoriesByTypeQueryHandler(ICategoryReader reader) : IRequestHandler<GetCategoriesByTypeQuery, IReadOnlyCollection<CategoryVm>>
{
    public async Task<IReadOnlyCollection<CategoryVm>> Handle(GetCategoriesByTypeQuery request, CancellationToken cancellationToken)
        => await reader.GetCategoryByTypeAsync(request.Type, cancellationToken);

}
