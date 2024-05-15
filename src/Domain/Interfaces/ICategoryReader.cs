namespace TrackHubRouter.Domain.Interfaces;

public interface ICategoryReader
{
    Task<CategoryVm> GetCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CategoryVm>> GetCategoryByTypeAsync(CategoryType type, CancellationToken cancellationToken);
}
