using TrackHub.Router.Infrastructure.CommandTrack.Extensions;

namespace TrackHub.Router.Infrastructure.CommandTrack.Mappers;

internal static class PositionMapper
{
    public static PositionVm MapToPositionVm(this Position position, DeviceVm deviceDto)
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
            position.Address.GetAddress(position.DistanceToAddress),
            position.City,
            position.State,
            position.Country,
            new AttributesVm 
            {
                Ignition = position.Ignition,
                Satellites = position.Satellites.HasValue ? (int?)position.Satellites.Value : null,
                Mileage = position.Mileage,
                Temperature = position.Temperature
            }
        );

    public static PositionVm MapToPositionVm(this DevicePosition position, DeviceVm deviceDto)
        => new(
            deviceDto.DeviceId,
            position.Latitude,
            position.Longitude,
            position.Altitude,
            position.DeviceDateTime,
            null,
            position.Speed,
            position.Course,
            null,
            position.Address.GetAddress(position.DistanceToAddress),
            position.City,
            position.State,
            position.Country,
            new AttributesVm
            {
                Ignition = position.Ignition,
                Satellites = position.Satellites.HasValue ? (int?)position.Satellites.Value : null,
                Mileage = position.Mileage,
                Temperature = position.Temperature,
                HobbsMeter = position.HobbsMeter
            }
        );

    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<Position> positions, DeviceVm deviceDto)
    {
        foreach (var position in positions)
        {
            yield return position.MapToPositionVm(deviceDto);
        }
    }

    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<Position> positions, IDictionary<string, DeviceVm> devicesDictionary)
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

    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<DevicePosition> positions, IDictionary<string, DeviceVm> devicesDictionary)
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
