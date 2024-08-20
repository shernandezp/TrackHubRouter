using System.Text.Json;

namespace TrackHub.Router.Infrastructure.Traccar.Mappers;

internal static class PositionMapper
{
    private static AttributesVm? ExtractAttributes(this string attributes) 
    {
        if (string.IsNullOrEmpty(attributes))
        {
            return null;
        }

        using JsonDocument doc = JsonDocument.Parse(attributes);
        JsonElement root = doc.RootElement;

        bool? ignition = null;
        if (root.TryGetProperty("ignition", out JsonElement ignitionElement) &&
                (ignitionElement.ValueKind == JsonValueKind.True || ignitionElement.ValueKind == JsonValueKind.False))
        {
            ignition = ignitionElement.GetBoolean();
        }

        double? totalDistance = null;
        if (root.TryGetProperty("totalDistance", out JsonElement totalDistanceElement) && 
                totalDistanceElement.ValueKind == JsonValueKind.Number)
        {
            totalDistance = totalDistanceElement.GetDouble();
        }
        return new AttributesVm
        {
            Ignition = ignition,
            Mileage = totalDistance
        };
    }

    public static PositionVm MapToPositionVm(this Position position, DeviceOperatorVm deviceDto)
        => new(
            deviceDto.DeviceId,
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
            position.Attributes.ExtractAttributes()
        );

    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<Position> positions, DeviceOperatorVm deviceDto)
    {
        foreach (var position in positions)
        {
            yield return position.MapToPositionVm(deviceDto);
        }
    }

    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<Position> positions, IDictionary<int, DeviceOperatorVm> devicesDictionary)
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
