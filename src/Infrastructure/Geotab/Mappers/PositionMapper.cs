using Geotab.Checkmate.ObjectModel;
using TrackHubRouter.Domain.Models;

namespace TrackHub.Router.Infrastructure.Geotab.Mappers;

internal static class PositionMapper
{

    /// <summary>
    /// Maps a LogRecord object to a PositionVm object
    /// </summary>
    /// <param name="position"></param>
    /// <param name="deviceDto"></param>
    /// <returns>returns a PositionVm object</returns>
    public static PositionVm MapToPositionVm(this LogRecord position, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            deviceDto.Name,
            deviceDto.TransporterType,
            position.Latitude ?? 0,
            position.Longitude ?? 0,
            null,
            position.DateTime ?? DateTime.Now,
            null,
            position.Speed ?? 0,
            0,
            null,
            null,
            null,
            null,
            null,
            null
        );

    /// <summary>
    /// Maps a DeviceStatusInfo object to a PositionVm object
    /// </summary>
    /// <param name="position"></param>
    /// <param name="deviceDto"></param>
    /// <returns>returns a PositionVm object</returns>
    public static PositionVm MapToPositionVm(this DeviceStatusInfo position, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            deviceDto.Name,
            deviceDto.TransporterType,
            position.Latitude ?? 0,
            position.Longitude ?? 0,
            null,
            position.DateTime ?? DateTime.Now,
            null,
            position.Speed ?? 0,
            position.Bearing,
            null,
            null,
            null,
            null,
            null,
            null
        );

    /// <summary>
    /// Maps a collection of LogRecord objects to a collection of PositionVm objects
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="deviceDto"></param>
    /// <returns>returns a collection of PositionVm objects</returns>
    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<LogRecord> positions, DeviceTransporterVm deviceDto)
    {
        foreach (var position in positions)
        {
            yield return position.MapToPositionVm(deviceDto);
        }
    }

    /// <summary>
    /// Maps a collection of LogRecord objects to a collection of PositionVm objects using a dictionary of device transporters
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="devicesDictionary"></param>
    /// <returns>returns a collection of PositionVm objects</returns>
    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<LogRecord> positions, IDictionary<string, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var position in positions)
        {
            if (position.Device != null && position.Device.Name != null && devicesDictionary.TryGetValue(position.Device.Name, out var device))
            {
                yield return position.MapToPositionVm(device);
            }
            else
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Maps a collection of DeviceStatusInfo objects to a collection of PositionVm objects using a dictionary of device transporters
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="devicesDictionary"></param>
    /// <returns>returns a collection of PositionVm objects</returns>
    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<DeviceStatusInfo> positions, IDictionary<string, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var position in positions)
        {
            if (position.Device != null && position.Device.Name != null && devicesDictionary.TryGetValue(position.Device.Name, out var device))
            {
                yield return position.MapToPositionVm(device);
            }
            else
            {
                continue;
            }
        }
    }
}
