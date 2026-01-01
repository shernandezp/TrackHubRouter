using Common.Domain.Enums;

namespace TrackHub.Router.Infrastructure.Flespi.Mappers;

internal static class DeviceMapper
{
    // Default device type and transporter type if not provided
    private const DeviceType DefaultDeviceType = DeviceType.Cellular;
    private const TransporterType DefaultTransporterType = TransporterType.Truck;

    /// <summary>
    /// Maps a Device object and a DeviceTransporterVm to a DeviceVm.
    /// </summary>
    public static DeviceVm MapToDeviceVm(this Device device, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            (int)device.Id,
            device.Ident ?? device.Id.ToString(),
            device.Name,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    /// <summary>
    /// Maps a Device object to a DeviceVm with null DeviceId.
    /// </summary>
    public static DeviceVm MapToDeviceVm(this Device device)
        => new(
            null,
            (int)device.Id,
            device.Ident ?? device.Id.ToString(),
            device.Name,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    /// <summary>
    /// Maps a collection of Device objects to DeviceVm objects using a dictionary of DeviceTransporterVm.
    /// </summary>
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices, IDictionary<int, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var device in devices)
        {
            if (devicesDictionary.TryGetValue((int)device.Id, out var selectedDevice))
            {
                yield return device.MapToDeviceVm(selectedDevice);
            }
        }
    }

    /// <summary>
    /// Maps a collection of Device objects to DeviceVm objects.
    /// </summary>
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Device> devices)
    {
        foreach (var device in devices)
        {
            yield return device.MapToDeviceVm();
        }
    }
}
