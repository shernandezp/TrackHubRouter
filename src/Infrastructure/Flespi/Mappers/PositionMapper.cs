using Common.Domain.Extensions;

namespace TrackHub.Router.Infrastructure.Flespi.Mappers;

internal static class PositionMapper
{

    /// <summary>
    /// Maps a Message to a PositionVm object.
    /// </summary>
    public static PositionVm MapToPositionVm(this Message message, DeviceTransporterVm deviceDto)
        => new(
            deviceDto.TransporterId,
            deviceDto.Name,
            deviceDto.TransporterType,
            message.Position_latitude ?? 0,
            message.Position_longitude ?? 0,
            message.Position_altitude,
            message.Timestamp.FromUnixTimestamp(),
            null,
            message.Position_speed ?? 0,
            message.Position_direction,
            null,
            null,
            null,
            null,
            null,
            new AttributesVm
            {
                Satellites = message.Position_satellites
            }
        );

    /// <summary>
    /// Maps a collection of Message objects to PositionVm objects.
    /// </summary>
    public static IEnumerable<PositionVm> MapToPositionVm(this IEnumerable<Message> messages, DeviceTransporterVm deviceDto)
    {
        foreach (var message in messages)
        {
            if (message.Position_latitude.HasValue && message.Position_longitude.HasValue)
            {
                yield return message.MapToPositionVm(deviceDto);
            }
        }
    }
}
