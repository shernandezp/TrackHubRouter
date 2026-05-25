namespace TrackHub.Router.Infrastructure.ManagerApi;

public class DeviceSyncWriter(IGraphQLClientFactory graphQLClient)
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IDeviceSyncWriter
{
    public async Task SynchronizeAsync(Guid accountId, Guid operatorId, IEnumerable<SynchronizedDeviceDto> devices, string correlationId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                mutation($command: SynchronizeOperatorDevicesCommandInput!) {
                    synchronizeOperatorDevices(command: $command) { operatorSyncRunId }
                }",
            Variables = new
            {
                command = new
                {
                    accountId,
                    operatorId,
                    correlationId,
                    devices = devices.Select(d => new
                    {
                        accountId = d.AccountId,
                        operatorId = d.OperatorId,
                        serial = d.Serial,
                        name = d.Name,
                        identifier = d.Identifier,
                        providerDisplayName = d.ProviderDisplayName,
                        deviceTypeId = d.DeviceTypeId,
                        description = d.Description,
                        providerMetadataHash = d.ProviderMetadataHash,
                        providerStatus = d.ProviderStatus
                    }).ToArray()
                }
            }
        };
        await MutationAsync<object>(request, cancellationToken);
    }
}
