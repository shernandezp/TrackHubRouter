using TrackHubRouter.Application.Positions.GetByUser;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.Endpoints;

public class Positions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetPositionByUser);
    }

    public async Task<IEnumerable<PositionVm>> GetPositionByUser(ISender sender, [AsParameters] GetPositionByUserQuery query)
        => await sender.Send(query);

}
