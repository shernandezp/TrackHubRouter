using Common.Domain.Enums;
using TrackHubRouter.Application.Categories.Commands.CreateCategory;
using TrackHubRouter.Application.Categories.Commands.DeleteCategory;
using TrackHubRouter.Application.Categories.Commands.UpdateCategory;
using TrackHubRouter.Application.Categories.Queries.GetCategoriesByType;
using TrackHubRouter.Application.Categories.Queries.GetCategory;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.Endpoints;

public class Categories : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetCategory)
            .MapGet(GetCategories, "ByType/{id}")
            .MapPost(CreateCategory)
            .MapPut(UpdateCategory, "{id}")
            .MapDelete(DeleteCategory, "{id}");
    }

    public async Task<CategoryVm> GetCategory(ISender sender, [AsParameters] GetCategoryQuery query)
        => await sender.Send(query);

    public async Task<IReadOnlyCollection<CategoryVm>> GetCategories(ISender sender, CategoryType id)
        => await sender.Send(new GetCategoriesByTypeQuery(id));

    public async Task<CategoryVm> CreateCategory(ISender sender, CreateCategoryCommand command)
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
