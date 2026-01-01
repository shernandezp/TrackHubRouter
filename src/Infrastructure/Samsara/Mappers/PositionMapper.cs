namespace TrackHub.Router.Infrastructure.Samsara.Mappers;

internal static class PositionMapper
{
    // Conversion factor from miles per hour to km/h
    private const double MphToKmh = 1.60934;

    /// <summary>
    /// Maps a VehicleStats with GPS data to a PositionVm object.
    /// </summary>
    public static PositionVm MapToPositionVm(this VehicleStats vehicle, DeviceTransporterVm deviceDto)
    {
        var gps = vehicle.Gps;
        return new PositionVm(
            deviceDto.TransporterId,
            deviceDto.Name,
            deviceDto.TransporterType,
            gps?.Latitude ?? 0,
            gps?.Longitude ?? 0,
            null,  // Samsara doesn't provide altitude in basic GPS data
            gps?.Time ?? DateTimeOffset.MinValue,
            null,
            (gps?.SpeedMilesPerHour ?? 0) * MphToKmh,  // Convert to km/h
            gps?.HeadingDegrees,
            null,
            gps?.ReverseGeo?.FormattedLocation,
            null,
            null,
            null,
            null
        );
    }

    /// <summary>
    /// Maps a GpsData to a PositionVm object.
    /// </summary>
    public static PositionVm MapToPositionVm(this GpsData gps, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            deviceDto.Name,
            deviceDto.TransporterType,
            gps.Latitude,
            gps.Longitude,
            null,  // Samsara doesn't provide altitude in basic GPS data
            gps.Time,
            null,
            gps.SpeedMilesPerHour * MphToKmh,  // Convert to km/h
            gps.HeadingDegrees,
            null,
            gps.ReverseGeo?.FormattedLocation,
            null,
            null,
            null,
            null
        );

    /// <summary>
    /// Maps a collection of VehicleStats objects to PositionVm objects using a dictionary of DeviceTransporterVm.
    /// </summary>
    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<VehicleStats> vehicles, IDictionary<string, DeviceTransporterVm> devicesDictionary)
    {
        foreach (var vehicle in vehicles)
        {
            if (vehicle.Gps.HasValue && devicesDictionary.TryGetValue(vehicle.Id, out var device))
            {
                yield return vehicle.MapToPositionVm(device);
            }
        }
    }

    /// <summary>
    /// Maps a VehicleHistory to a collection of PositionVm objects.
    /// </summary>
    public static IEnumerable<PositionVm> MapToPositionVm(this VehicleHistory vehicle, DeviceTransporterVm deviceDto)
    {
        if (vehicle.Gps is null)
        {
            yield break;
        }

        foreach (var gps in vehicle.Gps)
        {
            yield return gps.MapToPositionVm(deviceDto);
        }
    }
}
