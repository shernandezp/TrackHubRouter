namespace TrackHub.Router.Infrastructure.ManagerApi;

public class DeviceSyncWriter(IGraphQLClientFactory graphQLClient)
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IDeviceSyncWriter
{
    public async Task ResetAsync(Guid accountId, Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                mutation($operatorId: UUID!) {
                    wipeDevices(operatorId: $operatorId)
                }",
            Variables = new { operatorId }
        };
        await MutationAsync<object>(request, cancellationToken);
    }

    public async Task SynchronizeAsync(
        Guid accountId,
        Guid operatorId,
        IEnumerable<SynchronizedDeviceDto> devices,
        string correlationId,
        string triggerType,
        bool autoAssignNewDevices,
        CancellationToken cancellationToken)
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
                    triggerType,
                    autoAssignNewDevices,
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
