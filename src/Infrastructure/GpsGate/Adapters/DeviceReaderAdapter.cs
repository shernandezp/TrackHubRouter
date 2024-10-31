using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces.Operator;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.GpsGate.Adapters;

// Adapter is required here to make the Init method asynchronous
// This class is an adapter that implements the IExternalDeviceReader interface
// It wraps the DeviceReader class and provides an asynchronous implementation of the methods
public sealed class DeviceReaderAdapter(DeviceReader deviceReader) : IExternalDeviceReader
{
    // Gets the protocol type from the wrapped DeviceReader instance
    public ProtocolType Protocol => deviceReader.Protocol;

    /// <summary>
    /// Initializes the device reader asynchronously
    /// </summary>
    /// <param name="credential"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>It runs the Init method of the wrapped DeviceReader instance in a separate task</returns>
    public Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
        => Task.Run(() => deviceReader.Init(credential), cancellationToken);

    /// <summary>
    /// Retrieves a device asynchronously
    /// </summary>
    /// <param name="deviceDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>It calls the GetDeviceAsync method of the wrapped DeviceReader instance</returns>
    public Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
        => deviceReader.GetDeviceAsync(deviceDto, cancellationToken);

    /// <summary>
    /// Retrieves multiple devices asynchronously
    /// </summary>
    /// <param name="devices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>It calls the GetDevicesAsync method of the wrapped DeviceReader instance</returns>
    public Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
        => deviceReader.GetDevicesAsync(devices, cancellationToken);

    /// <summary>
    /// Retrieves all devices asynchronously
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>It calls the GetDevicesAsync method of the wrapped DeviceReader instance</returns>
    public Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
        => deviceReader.GetDevicesAsync(cancellationToken);
}
