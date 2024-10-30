namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IDeviceReader
{
    Task<IEnumerable<DeviceTransporterVm>> GetDevicesByOperatorAsync(Guid operatorId, CancellationToken cancellationToken);
    Task<IEnumerable<DeviceTransporterVm>> GetDeviceTransporterAsync(Guid operatorId, CancellationToken cancellationToken);
}
