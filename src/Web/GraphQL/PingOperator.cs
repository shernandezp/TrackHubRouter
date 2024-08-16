using TrackHubRouter.Application.PingOperator.Queries;

namespace TrackHubRouter.Web.GraphQL;

public partial class Query
{
    public async Task<bool> PingOperator([Service] ISender sender, [AsParameters] PingOperatorQuery query)
        => await sender.Send(query);
}
