namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IDeviceReader
{
    Task<IEnumerable<DeviceOperatorVm>> GetDevicesByOperatorAsync(Guid operatorId, CancellationToken cancellationToken);
}
