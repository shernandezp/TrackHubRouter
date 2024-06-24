namespace TrackHub.Router.Infrastructure.CommandTrack.Mappers;

internal static class PositionMapper
{
    public static PositionVm MapToPositionVm(this Position position, DeviceDto deviceDto)
        => new (
            deviceDto.DeviceId,
            position.Latitude,
            position.Longitude,
            position.Altitude,
            position.DeviceDateTime,
            position.ServerDateTime,
            position.Speed,
            position.Course,
            position.EventId,
            position.Address,
            position.City,
            position.State,
            position.Country,
            new AttributesVm 
            {
                Ignition = position.Ignition,
                Satellites = position.Satellites,
                Mileage = position.Mileage,
                HobbsMeter = position.HobbsMeter,
                Temperature = position.Temperature
            }
        );

    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<Position> positions, DeviceDto deviceDto)
    {
        foreach (var position in positions)
        {
            yield return position.MapToPositionVm(deviceDto);
        }
    }

    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<Position> positions, IDictionary<string, DeviceDto> devicesDictionary)
    {
        foreach (var position in positions)
        {
            if (devicesDictionary.TryGetValue(position.Plate, out var device))
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
