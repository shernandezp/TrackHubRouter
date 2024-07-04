namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IDeviceReader
{
    Task<IEnumerable<DeviceVm>> GetOperatorsAsync(Guid userId, CancellationToken cancellationToken);
}
