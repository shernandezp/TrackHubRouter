using Common.Domain.Enums;

namespace TrackHub.Router.Infrastructure.Wialon.Mappers;

internal static class DeviceMapper
{
    // Default device type and transporter type if not provided
    private const DeviceType DefaultDeviceType = DeviceType.Cellular;
    private const TransporterType DefaultTransporterType = TransporterType.Truck;

    /// <summary>
    /// Maps a Unit object and a DeviceTransporterVm to a DeviceVm.
    /// </summary>
    public static DeviceVm MapToDeviceVm(this Unit unit, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            (int)unit.Id,
            unit.Uid ?? unit.Id.ToString(),
            unit.Nm,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    /// <summary>
    /// Maps a Unit object to a DeviceVm with null DeviceId.
    /// </summary>
    public static DeviceVm MapToDeviceVm(this Unit unit)
        => new(
            null,
            (int)unit.Id,
            unit.Uid ?? unit.Id.ToString(),
            unit.Nm,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    /// <summary>
    /// Maps a collection of Unit objects to DeviceVm objects using a dictionary of DeviceTransporterVm.
    /// </summary>
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Unit> units, IDictionary<int, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var unit in units)
        {
            if (devicesDictionary.TryGetValue((int)unit.Id, out var selectedDevice))
            {
                yield return unit.MapToDeviceVm(selectedDevice);
            }
        }
    }

    /// <summary>
    /// Maps a collection of Unit objects to DeviceVm objects.
    /// </summary>
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Unit> units)
    {
        foreach (var unit in units)
        {
            yield return unit.MapToDeviceVm();
        }
    }
}
