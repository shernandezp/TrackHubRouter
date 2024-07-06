namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IDeviceReader
{
    Task<IEnumerable<DeviceVm>> GetDevicesByOperatorAsync(Guid userId, Guid operatorId, CancellationToken cancellationToken);
}
