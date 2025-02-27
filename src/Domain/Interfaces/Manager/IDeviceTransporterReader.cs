namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IDeviceTransporterReader
{
    Task<IEnumerable<DeviceTransporterVm>> GetDevicesByOperatorAsync(Guid operatorId, CancellationToken cancellationToken);
    Task<IEnumerable<DeviceTransporterVm>> GetDeviceTransporterAsync(Guid operatorId, CancellationToken cancellationToken);
    Task<DeviceTransporterVm> GetDevicesTransporterAsync(Guid transporterId, CancellationToken cancellationToken);
}
