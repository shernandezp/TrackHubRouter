using Common.Domain.Enums;

namespace TrackHub.Router.Infrastructure.Navixy.Mappers;

internal static class DeviceMapper
{
    // Default device type and transporter type if not provided
    private const DeviceType DefaultDeviceType = DeviceType.Cellular;
    private const TransporterType DefaultTransporterType = TransporterType.Truck;

    /// <summary>
    /// Maps a Tracker object and a DeviceTransporterVm to a DeviceVm.
    /// </summary>
    public static DeviceVm MapToDeviceVm(this Tracker tracker, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            (int)tracker.Tracker_id,
            tracker.Imei,
            tracker.Label,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    /// <summary>
    /// Maps a Tracker object to a DeviceVm with null DeviceId.
    /// </summary>
    public static DeviceVm MapToDeviceVm(this Tracker tracker)
        => new(
            null,
            (int)tracker.Tracker_id,
            tracker.Imei,
            tracker.Label,
            (short)DefaultDeviceType,
            (short)DefaultTransporterType
        );

    /// <summary>
    /// Maps a collection of Tracker objects to DeviceVm objects using a dictionary of DeviceTransporterVm.
    /// </summary>
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Tracker> trackers, IDictionary<int, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var tracker in trackers)
        {
            if (devicesDictionary.TryGetValue((int)tracker.Tracker_id, out var selectedDevice))
            {
                yield return tracker.MapToDeviceVm(selectedDevice);
            }
        }
    }

    /// <summary>
    /// Maps a collection of Tracker objects to DeviceVm objects.
    /// </summary>
    public static IEnumerable<DeviceVm> MapToDeviceVm(this IEnumerable<Tracker> trackers)
    {
        foreach (var tracker in trackers)
        {
            yield return tracker.MapToDeviceVm();
        }
    }
}
