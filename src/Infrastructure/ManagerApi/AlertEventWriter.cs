namespace TrackHub.Router.Infrastructure.ManagerApi;

public class AlertEventWriter(IGraphQLClientFactory graphQLClient)
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IAlertEventWriter
{
    public async Task RecordAsync(AlertEventDto dto, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                mutation($command: RecordAlertEventCommandInput!) {
                    recordAlertEvent(command: $command) { alertEventId }
                }",
            Variables = new
            {
                command = new
                {
                    alertEvent = new
                    {
                        accountId = dto.AccountId,
                        eventType = dto.EventType,
                        severity = dto.Severity,
                        sourceModule = dto.SourceModule,
                        resourceType = dto.ResourceType,
                        resourceId = dto.ResourceId,
                        status = dto.Status,
                        payloadJson = dto.PayloadJson,
                        deduplicationKey = dto.DeduplicationKey
                    }
                }
            }
        };
        await MutationAsync<object>(request, cancellationToken);
    }
}
