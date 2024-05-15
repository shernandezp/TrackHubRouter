using TrackHubRouter.Application.Categories.Commands.CreateCategory;
using TrackHubRouter.Application.Categories.Commands.DeleteCategory;
using TrackHubRouter.Application.Categories.Commands.UpdateCategory;
using TrackHubRouter.Application.Categories.Queries.GetCategoriesByType;
using TrackHubRouter.Application.Categories.Queries.GetCategory;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.GraphQL;

//[AuthorizeQL]
public sealed class Categories
{
    public async Task<CategoryVm> GetCategory([Service] ISender sender, [AsParameters] GetCategoryQuery query)
        => await sender.Send(query);

    public async Task<IReadOnlyCollection<CategoryVm>> GetCategories([Service] ISender sender, [AsParameters] GetCategoriesByTypeQuery query)
        => await sender.Send(query);

    public async Task<CategoryVm> CreateCategory([Service] ISender sender, CreateCategoryCommand command)
        => await sender.Send(command);

    public async Task<IResult> UpdateCategory(ISender sender, Guid id, UpdateCategoryCommand command)
    {
        if (id != command.Category.CategoryId) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteCategory(ISender sender, Guid id)
    {
        await sender.Send(new DeleteCategoryCommand(id));
        return Results.NoContent();
    }
}
