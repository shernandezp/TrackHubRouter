using TrackHubRouter.Application.Positions.Get;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.GraphQL;

public partial class Query
{
    public async Task<IEnumerable<PositionVm>> GetPositions([Service] ISender sender, [AsParameters] GetPositionsQuery query)
        => await sender.Send(query);

}
