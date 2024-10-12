namespace TrackHub.Router.Infrastructure.Traccar.Mappers;

internal static class PositionMapper
{

    /// <summary>
    /// Maps a Position object to a PositionVm object
    /// </summary>
    /// <param name="position"></param>
    /// <param name="deviceDto"></param>
    /// <returns>returns a PositionVm object</returns>
    public static PositionVm MapToPositionVm(this Position position, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.DeviceId,
            deviceDto.Name,
            deviceDto.TransporterType,
            position.Latitude,
            position.Longitude,
            position.Altitude,
            position.DeviceTime,
            position.ServerTime,
            position.Speed,
            position.Course,
            null,
            position.Address,
            null,
            null,
            null,
            new AttributesVm
            {
                Ignition = position.Attributes.Ignition,
                Mileage = position.Attributes.Odometer != null ? position.Attributes.Odometer : position.Attributes.TotalDistance
            }
        );

    /// <summary>
    /// Maps a collection of Position objects to a collection of PositionVm objects
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="deviceDto"></param>
    /// <returns>returns a collection of PositionVm objects</returns>
    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<Position> positions, DeviceTransporterVm deviceDto)
    {
        foreach (var position in positions)
        {
            yield return position.MapToPositionVm(deviceDto);
        }
    }

    /// <summary>
    /// Maps a collection of Position objects to a collection of PositionVm objects based on a devicesDictionary
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="devicesDictionary"></param>
    /// <returns>returns a collection of PositionVm objects</returns>
    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<Position> positions, IDictionary<int, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var position in positions)
        {
            if (devicesDictionary.TryGetValue(position.DeviceId, out var device))
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
