using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Flespi.Adapters;

/// <summary>
/// Adapter that implements IPositionReader interface.
/// Wraps PositionReader to provide async initialization.
/// </summary>
public sealed class PositionReaderAdapter(PositionReader positionReader) : IPositionReader
{
    public ProtocolType Protocol => positionReader.Protocol;

    /// <summary>
    /// Initializes the position reader asynchronously.
    /// </summary>
    public Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
        => Task.Run(() => positionReader.Init(credential, cancellationToken), cancellationToken);

    /// <summary>
    /// Retrieves the last position of a single device asynchronously.
    /// </summary>
    public Task<PositionVm> GetDevicePositionAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
        => positionReader.GetDevicePositionAsync(deviceDto, cancellationToken);

    /// <summary>
    /// Retrieves the last positions of multiple devices asynchronously.
    /// </summary>
    public Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
        => positionReader.GetDevicePositionAsync(devices, cancellationToken);

    /// <summary>
    /// Retrieves positions within a time range asynchronously.
    /// </summary>
    public Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
        => positionReader.GetPositionAsync(from, to, deviceDto, cancellationToken);
}
