using TrackHubRouter.Application.Positions.Events;

namespace TrackHubRouter.Application.Positions.Commands.Sync;

public readonly record struct SyncPositionCommand() : IRequest<bool>;

public class UpdateTransporterCommandHandler(IAccountReader reader,
    IOperatorReader operatorReader,
    IPublisher publisher) : IRequestHandler<SyncPositionCommand, bool>
{
    public async Task<bool> Handle(SyncPositionCommand request, CancellationToken cancellationToken)
    { 
        var accounts = await reader.GetAccountsToSyncAsync(cancellationToken);
        foreach (var account in accounts)
        {
            var operators = await operatorReader.GetOperatorsByAccountsAsync(account.AccountId, cancellationToken);
            foreach (var @operator in operators)
            {
                var operatorCredential = await operatorReader.GetOperatorAsync(@operator.OperatorId, cancellationToken);
                await publisher.Publish(new OperatorRetrieved.Notification(operatorCredential), cancellationToken);
            }
        }
        return true;
    }
}
