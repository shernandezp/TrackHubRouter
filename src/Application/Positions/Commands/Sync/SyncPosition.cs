namespace TrackHubRouter.Application.Positions.Commands.Sync;

public readonly record struct SyncPositionCommand() : IRequest<bool>;

public class UpdateTransporterCommandHandler(IAccountReader reader) : IRequestHandler<SyncPositionCommand, bool>
{
    public async Task<bool> Handle(SyncPositionCommand request, CancellationToken cancellationToken)
    { 
        var accounts = await reader.GetAccountsToSyncAsync(cancellationToken); 
        return true;
    }
}
