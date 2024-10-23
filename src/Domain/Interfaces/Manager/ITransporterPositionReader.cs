namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface ITransporterPositionReader
{
    Task<IEnumerable<PositionVm>> GetTransporterPositionAsync(Guid operatorId, CancellationToken cancellationToken);
}
