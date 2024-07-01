namespace TrackHub.Router.Infrastructure.Traccar.Mappers;

internal static class DeviceMapper
{
    public static DeviceVm MapToDeviceVm(this Device device, DeviceDto deviceDto)
        => new(
            deviceDto.DeviceId,
            device.Id,
            device.UniqueId,
            device.Name
        );

    public static DeviceVm MapToDeviceVm(this Device device)
        => new(
            null,
            device.Id,
            device.UniqueId,
            device.Name
        );

    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices, IDictionary<int, DeviceDto> devicesDictionary)
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

    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices)
    {
        foreach (var device in devices)
        {
            yield return device.MapToDeviceVm();
        }
    }

}
