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

    public async Task<IEnumerable<AccountSettingsVm>> GetAccountsToSyncAsync(CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($filter: FiltersInput!) {
                    accountsSettings(
                        query: { filter: $filter }
                      ) {
                            accountId
                            storeLastPosition
                            storingTimeLapse
                      }
                }",
            Variables = new
            {
                filter = new
                {
                    filters = new[]
                    {
                        new
                        {
                            key = "StoreLastPosition",
                            value = false
                        }
                    }
                }
            }
        };
        return await QueryAsync<IEnumerable<AccountSettingsVm>>(request, cancellationToken);
    }
}
