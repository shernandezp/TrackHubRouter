using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Samsara.Adapters;

/// <summary>
/// Adapter that implements IExternalDeviceReader interface.
/// Wraps DeviceReader to provide async initialization.
/// </summary>
public sealed class DeviceReaderAdapter(DeviceReader deviceReader) : IExternalDeviceReader
{
    public ProtocolType Protocol => deviceReader.Protocol;

    /// <summary>
    /// Initializes the device reader asynchronously.
    /// </summary>
    public Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
        => Task.Run(() => deviceReader.Init(credential, cancellationToken), cancellationToken);

    /// <summary>
    /// Retrieves a device asynchronously.
    /// </summary>
    public Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
        => deviceReader.GetDeviceAsync(deviceDto, cancellationToken);

    /// <summary>
    /// Retrieves multiple devices asynchronously.
    /// </summary>
    public Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
        => deviceReader.GetDevicesAsync(devices, cancellationToken);

    /// <summary>
    /// Retrieves all devices asynchronously.
    /// </summary>
    public Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
        => deviceReader.GetDevicesAsync(cancellationToken);
}
