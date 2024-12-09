using TrackHubRouter.Application.DevicePositions.Queries.Get;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.Endpoints;

public class DevicePositions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetDevicePositions);
    }

    public async Task<IEnumerable<PositionVm>> GetDevicePositions(ISender sender, [AsParameters] GetPositionsByUserQuery query)
        => await sender.Send(query);

}
