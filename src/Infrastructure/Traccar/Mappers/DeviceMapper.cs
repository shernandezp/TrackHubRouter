namespace TrackHub.Router.Infrastructure.Traccar.Mappers;

internal static class DeviceMapper
{
    public static ExternalDeviceVm MapToDeviceVm(this Device device, DeviceVm deviceDto)
        => new(
            deviceDto.DeviceId,
            device.Id,
            device.UniqueId,
            device.Name
        );

    public static ExternalDeviceVm MapToDeviceVm(this Device device)
        => new(
            null,
            device.Id,
            device.UniqueId,
            device.Name
        );

    public static IEnumerable<ExternalDeviceVm> MapToDeviceVm(this IEnumerable<Device> devices, IDictionary<int, DeviceVm> devicesDictionary)
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

    public static IEnumerable<ExternalDeviceVm> MapToDeviceVm(this IEnumerable<Device> devices)
    {
        foreach (var device in devices)
        {
            yield return device.MapToDeviceVm();
        }
    }

}
