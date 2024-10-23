using Common.Domain.Enums;

namespace TrackHub.Router.Infrastructure.CommandTrack.Mappers;

internal static class DeviceMapper
{
    // Default device type and transporter type if not provided
    const DeviceType DefaultDeviceType = DeviceType.Cellular;
    const TransporterType DefaultTransporterType = TransporterType.Truck;

    // Maps a DevicePosition and a DeviceVm to an ExternalDeviceVm
    public static DeviceVm MapToDeviceVm(this DevicePosition device, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            device.Id,
            device.Serial,
            device.Plate,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
    );

    // Maps a DevicePosition to an ExternalDeviceVm with null DeviceId
    public static DeviceVm MapToDeviceVm(this DevicePosition device)
        => new(
            null,
            device.Id,
            device.Serial,
            device.Plate,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
    );

    // Maps a collection of DevicePosition objects to a collection of ExternalDeviceVm objects using a dictionary of DeviceVm objects
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<DevicePosition> devices, IDictionary<int, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var device in devices)
        {
            if (devicesDictionary.TryGetValue(device.Id, out var selectedDevice))
            {
                yield return device.MapToDeviceVm(selectedDevice);
            }
            else
            {
                continue;
            }
        }
    }

    // Maps a collection of DevicePosition objects to a collection of ExternalDeviceVm objects
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<DevicePosition> devices)
    {
        foreach (var device in devices)
        {
            yield return device.MapToDeviceVm();
        }
    }
}
