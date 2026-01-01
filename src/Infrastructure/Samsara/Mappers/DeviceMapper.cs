using Common.Domain.Enums;

namespace TrackHub.Router.Infrastructure.Samsara.Mappers;

internal static class DeviceMapper
{
    // Default device type and transporter type if not provided
    private const DeviceType DefaultDeviceType = DeviceType.Cellular;
    private const TransporterType DefaultTransporterType = TransporterType.Truck;

    /// <summary>
    /// Maps a VehicleStats object and a DeviceTransporterVm to a DeviceVm.
    /// </summary>
    public static DeviceVm MapToDeviceVm(this VehicleStats vehicle, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            int.TryParse(vehicle.Id, out var id) ? id : 0,
            vehicle.Id,
            vehicle.Name,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    /// <summary>
    /// Maps a VehicleStats object to a DeviceVm with null DeviceId.
    /// </summary>
    public static DeviceVm MapToDeviceVm(this VehicleStats vehicle)
        => new(
            null,
            int.TryParse(vehicle.Id, out var id) ? id : 0,
            vehicle.Id,
            vehicle.Name,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    /// <summary>
    /// Maps a collection of VehicleStats objects to DeviceVm objects using a dictionary of DeviceTransporterVm.
    /// </summary>
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<VehicleStats> vehicles, IDictionary<string, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var vehicle in vehicles)
        {
            if (devicesDictionary.TryGetValue(vehicle.Id, out var selectedDevice))
            {
                yield return vehicle.MapToDeviceVm(selectedDevice);
            }
        }
    }

    /// <summary>
    /// Maps a collection of VehicleStats objects to DeviceVm objects.
    /// </summary>
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<VehicleStats> vehicles)
    {
        foreach (var vehicle in vehicles)
        {
            yield return vehicle.MapToDeviceVm();
        }
    }
}
