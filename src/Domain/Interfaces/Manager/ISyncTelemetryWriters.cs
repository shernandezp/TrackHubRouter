using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IOperatorSyncRunWriter
{
    Task RecordAsync(OperatorSyncRunDto dto, CancellationToken cancellationToken);
}

public interface IOperatorHealthCheckWriter
{
    Task RecordAsync(OperatorHealthCheckDto dto, CancellationToken cancellationToken);
}

public interface IDeviceSyncWriter
{
    Task ResetAsync(Guid accountId, Guid operatorId, CancellationToken cancellationToken);
    Task SynchronizeAsync(Guid accountId, Guid operatorId, IEnumerable<SynchronizedDeviceDto> devices, string correlationId, string triggerType, bool autoAssignNewDevices, CancellationToken cancellationToken);
}

public interface IAlertEventWriter
{
    Task RecordAsync(AlertEventDto dto, CancellationToken cancellationToken);
}
