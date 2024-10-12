namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IDeviceReader
{
    Task<IEnumerable<DeviceTransporterVm>> GetDevicesByOperatorAsync(Guid operatorId, CancellationToken cancellationToken);
}
