using System.Text.Json;
using TrackHubRouter.Domain.Interfaces.Manager;

namespace TrackHub.Router.Infrastructure.ManagerApi;

public class PositionWriter(IGraphQLClientFactory graphQLClient) : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IPositionWriter
{
    public async Task<bool> AddOrUpdatePositionAsync(IEnumerable<PositionVm> positions, CancellationToken token)
    {
        var request = new GraphQLRequest
        {
            Query = @"
            mutation($command: BulkTransporterPositionCommandInput!) {
                bulkTransporterPosition(command: $command)
            }",
            Variables = new
            {
                command = new
                {
                    positions = positions.Select(p => new
                    {
                        transporterId = p.TransporterId,
                        speed = p.Speed,
                        longitude = p.Longitude,
                        state = p.State,
                        latitude = p.Latitude,
                        eventId = p.EventId,
                        deviceDateTime = p.DeviceDateTime,
                        course = p.Course,
                        country = p.Country,
                        city = p.City,
                        altitude = p.Altitude,
                        address = p.Address,
                        attributes = JsonSerializer.Serialize(p.Attributes)
                    }).ToArray()
                }
            }
        };

        return await MutationAsync<bool>(request, token);
    }
}
