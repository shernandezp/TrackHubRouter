namespace TrackHub.Router.Infrastructure.ManagerApi;

public class OperatorHealthCheckWriter(IGraphQLClientFactory graphQLClient)
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IOperatorHealthCheckWriter
{
    public async Task RecordAsync(OperatorHealthCheckDto dto, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                mutation($command: RecordOperatorHealthCommandInput!) {
                    recordOperatorHealth(command: $command) { operatorHealthCheckId }
                }",
            Variables = new
            {
                command = new
                {
                    check = new
                    {
                        accountId = dto.AccountId,
                        operatorId = dto.OperatorId,
                        checkType = dto.CheckType,
                        status = dto.Status,
                        latencyMs = dto.LatencyMs,
                        startedAt = dto.StartedAt,
                        completedAt = dto.CompletedAt,
                        errorCode = dto.ErrorCode,
                        errorMessage = dto.ErrorMessage,
                        retryCount = dto.RetryCount,
                        correlationId = dto.CorrelationId
                    }
                }
            }
        };
        await MutationAsync<object>(request, cancellationToken);
    }
}
