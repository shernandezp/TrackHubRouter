using TrackHubRouter.Application.Positions.Queries.Get;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.Endpoints;

public class Positions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetPositions);
    }

    public async Task<IEnumerable<PositionVm>> GetPositions(ISender sender, [AsParameters] GetPositionsQuery query)
        => await sender.Send(query);

}
