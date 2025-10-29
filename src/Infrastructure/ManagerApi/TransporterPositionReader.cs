namespace TrackHub.Router.Infrastructure.ManagerApi;

public class TransporterPositionReader(IGraphQLClientFactory graphQLClient)
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), ITransporterPositionReader
{

    public async Task<IEnumerable<PositionVm>> GetTransporterPositionAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($operatorId: UUID!) {
                    transporterPositionByOperator(query: { operatorId: $operatorId })
                    {
                        transporterId
                        transporterType
                        state
                        speed
                        longitude
                        latitude
                        geometryId
                        eventId
                        deviceName
                        deviceDateTime
                        course
                        country
                        city
                        attributes {
                            temperature
                            satellites
                            mileage
                            ignition
                            hourmeter
                        }
                        altitude
                        address
                    }
                }",
            Variables = new { operatorId }
        };
        return await QueryAsync<IEnumerable<PositionVm>>(request, cancellationToken);
    }
}
