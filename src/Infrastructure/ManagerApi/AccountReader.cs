using TrackHubRouter.Domain.Interfaces.Manager;

namespace TrackHub.Router.Infrastructure.ManagerApi;

public class AccountReader(IGraphQLClientFactory graphQLClient) 
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IAccountReader
{
    public async Task<AccountSettingsVm> GetAccountSettingsAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($id: UUID!) {
                        accountSettings(query: { id: $id })
                        {
                            storingTimeLapse
                            storeLastPosition
                            accountId
                        }
                    }",
            Variables = new { id = operatorId }
        };
        return await QueryAsync<AccountSettingsVm>(request, cancellationToken);
    }
}
