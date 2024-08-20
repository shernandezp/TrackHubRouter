using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Traccar.Adapters;

// Adapter is required here to make the Init method asynchronous
// This class is an adapter that implements the IExternalDeviceReader interface
// It wraps the DeviceReader class and provides an asynchronous implementation of the methods
public sealed class DeviceReaderAdapter(DeviceReader deviceReader) : IExternalDeviceReader
{
    // Gets the protocol type from the wrapped DeviceReader instance
    public ProtocolType Protocol => deviceReader.Protocol;

    // Initializes the device reader asynchronously
    // It runs the Init method of the wrapped DeviceReader instance in a separate task
    public Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
        => Task.Run(() => deviceReader.Init(credential), cancellationToken);

    // Retrieves a device asynchronously
    // It calls the GetDeviceAsync method of the wrapped DeviceReader instance
    public Task<DeviceVm> GetDeviceAsync(DeviceOperatorVm deviceDto, CancellationToken cancellationToken)
        => deviceReader.GetDeviceAsync(deviceDto, cancellationToken);

    // Retrieves multiple devices asynchronously
    // It calls the GetDevicesAsync method of the wrapped DeviceReader instance
    public Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceOperatorVm> devices, CancellationToken cancellationToken)
        => deviceReader.GetDevicesAsync(devices, cancellationToken);

    // Retrieves all devices asynchronously
    // It calls the GetDevicesAsync method of the wrapped DeviceReader instance
    public Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
        => deviceReader.GetDevicesAsync(cancellationToken);
}
