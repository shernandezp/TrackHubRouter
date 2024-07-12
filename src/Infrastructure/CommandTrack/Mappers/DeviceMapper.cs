namespace TrackHub.Router.Infrastructure.CommandTrack.Mappers;

internal static class DeviceMapper
{
    // Maps a DevicePosition and a DeviceVm to an ExternalDeviceVm
    public static ExternalDeviceVm MapToDeviceVm(this DevicePosition device, DeviceVm deviceDto)
        => new(
            deviceDto.DeviceId,
            device.Id,
            device.Serial,
            device.Plate
    );

    // Maps a DevicePosition to an ExternalDeviceVm with null DeviceId
    public static ExternalDeviceVm MapToDeviceVm(this DevicePosition device)
        => new(
            null,
            device.Id,
            device.Serial,
            device.Plate
    );

    // Maps a collection of DevicePosition objects to a collection of ExternalDeviceVm objects using a dictionary of DeviceVm objects
    public static IEnumerable<ExternalDeviceVm> MapToDeviceVm(this IEnumerable<DevicePosition> devices, IDictionary<int, DeviceVm> devicesDictionary)
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
    public static IEnumerable<ExternalDeviceVm> MapToDeviceVm(this IEnumerable<DevicePosition> devices)
    {
        foreach (var device in devices)
        {
            yield return device.MapToDeviceVm();
        }
    }
}
