using Common.Application.Interfaces;
using Common.Domain.Constants;
using Common.Infrastructure;
using TrackHubRouter.Domain.Models;
using GraphQL;
using TrackHubRouter.Domain.Interfaces.Manager;

namespace ManagerApi;

// This class represents a device reader that implements the IDeviceReader interface.
// It is responsible for retrieving devices by operator from the GraphQL service.
public class DeviceReader(IGraphQLClientFactory graphQLClient) : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IDeviceReader
{

    // Retrieves devices by operator asynchronously.
    // Parameters:
    //   userId: The ID of the user.
    //   operatorId: The ID of the operator.
    //   cancellationToken: The cancellation token.
    // Returns:
    //   A collection of DeviceVm objects representing the devices.
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
