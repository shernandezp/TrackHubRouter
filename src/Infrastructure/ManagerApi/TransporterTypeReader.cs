namespace TrackHub.Router.Infrastructure.ManagerApi;

// This class represents a device reader that implements the IDeviceReader interface.
// It is responsible for retrieving devices by operator from the GraphQL service.
public class TransporterTypeReader(IGraphQLClientFactory graphQLClient)
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), ITransporterTypeReader
{

    /// <summary>
    /// Retrieves a transporter type asynchronously.
    /// </summary>
    /// <param name="transporterTypeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TransporterTypeVm> GetTransporterTypeAsync(short transporterTypeId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($transporterTypeId: Short!) {
                    transporterType(query: { transporterTypeId: $transporterTypeId }) {
                        accBased
                        stoppedGap
                        maxTimeGap
                        maxDistance
                    }
                }",
            Variables = new { transporterTypeId }
        };
        return await QueryAsync<TransporterTypeVm>(request, cancellationToken);
    }
}
