using TrackHubRouter.Application.Positions.Queries.Get;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.GraphQL;

public partial class Query
{
    public async Task<IEnumerable<PositionVm>> GetPositionsByUser([Service] ISender sender)
        => await sender.Send(new GetPositionsByUserQuery());

}
