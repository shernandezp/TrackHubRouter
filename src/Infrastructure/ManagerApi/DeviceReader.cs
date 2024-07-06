using Common.Application.Interfaces;
using Common.Domain.Constants;
using Common.Infrastructure;
using TrackHubRouter.Domain.Models;
using GraphQL;
using TrackHubRouter.Domain.Interfaces.Manager;

namespace ManagerApi;

public class DeviceReader(IGraphQLClientFactory graphQLClient) : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IDeviceReader
{

    public async Task<IEnumerable<DeviceVm>> GetDevicesByOperatorAsync(Guid userId, Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($userId: UUID!, $operatorId: UUID!) {
                        devicesByUserByOperator(query: { userId: $userId, operatorId: $operatorId })
                        {
                            deviceId,
                            identifier,
                            serial,
                            name
                        }
                    }",
            Variables = new { userId, operatorId }
        };
        return await QueryAsync<IEnumerable<DeviceVm>>(request, cancellationToken);
    }
}
