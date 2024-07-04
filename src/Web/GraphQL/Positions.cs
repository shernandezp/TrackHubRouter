using TrackHubRouter.Application.Positions.GetByUser;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.GraphQL;

public partial class Query
{
    public async Task<IEnumerable<PositionVm>> GetPositionByUser([Service] ISender sender, [AsParameters] GetPositionByUserQuery query)
        => await sender.Send(query);

}
