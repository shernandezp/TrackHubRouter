namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IPositionWriter
{
    Task<bool> AddOrUpdatePositionAsync(IEnumerable<PositionVm> positions, CancellationToken token);
}
