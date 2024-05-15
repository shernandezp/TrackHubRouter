using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Domain.Interfaces;
public interface ICategoryWriter
{
    Task<CategoryVm> CreateCategoryAsync(CategoryDto categoryDto, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(CategoryDto categoryDto, CancellationToken cancellationToken = default);
}
