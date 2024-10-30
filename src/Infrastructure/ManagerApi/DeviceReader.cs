using TrackHubRouter.Domain.Interfaces.Manager;

namespace ManagerApi;

// This class represents a device reader that implements the IDeviceReader interface.
// It is responsible for retrieving devices by operator from the GraphQL service.
public class DeviceReader(IGraphQLClientFactory graphQLClient) 
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IDeviceReader
{

    /// <summary>
    /// Retrieves devices by operator asynchronously.
    /// </summary>
    /// <param name="operatorId">The ID of the operator.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of DeviceVm objects representing the devices.</returns>
    public async Task<IEnumerable<DeviceTransporterVm>> GetDevicesByOperatorAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($operatorId: UUID!) {
                    deviceByUserByOperator(query: { operatorId: $operatorId })
                    {
                        transporterId,
                        identifier,
                        serial,
                        name,
                        transporterType,
                        transporterTypeId
                    }
                }",
            Variables = new { operatorId }
        };
        return await QueryAsync<IEnumerable<DeviceTransporterVm>>(request, cancellationToken);
    }

    public async Task<IEnumerable<DeviceTransporterVm>> GetDeviceTransporterAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
            query($filter: FiltersInput!) {
                deviceTransporterMaster(
                    query: { filter: $filter }
                    ) {
                        transporterId,
                        identifier,
                        serial,
                        name,
                        transporterType,
                        transporterTypeId
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
                            key = "OperatorId",
                            value = operatorId
                        }
                    }
                }
            }
        };
        return await QueryAsync<IEnumerable<DeviceTransporterVm>>(request, cancellationToken);
    }
}
