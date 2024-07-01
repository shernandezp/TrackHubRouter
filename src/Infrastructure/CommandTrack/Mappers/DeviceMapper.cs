namespace TrackHub.Router.Infrastructure.CommandTrack.Mappers;

internal static class DeviceMapper
{
    public static DeviceVm MapToDeviceVm(this DevicePosition device, DeviceDto deviceDto)
        => new(
            deviceDto.DeviceId,
            device.Id,
            device.Serial,
            device.Plate
    );

    public static DeviceVm MapToDeviceVm(this DevicePosition device)
        => new(
            null,
            device.Id,
            device.Serial,
            device.Plate
    );

    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<DevicePosition> devices, IDictionary<int, DeviceDto> devicesDictionary)
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

    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<DevicePosition> devices)
    {
        foreach (var device in devices)
        {
            yield return device.MapToDeviceVm();
        }
    }
}
