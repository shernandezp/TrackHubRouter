namespace TrackHub.Router.Infrastructure.Flespi.Models;

/// <summary>
/// Flespi message (contains GPS position data)
/// Timestamp = Unix timestamp in seconds
/// </summary>
internal readonly record struct Message(
    long? Ident,
    long? Device_id,
    long? Channel_id,
    double? Timestamp,
    double? Position_latitude,
    double? Position_longitude,
    double? Position_altitude,
    double? Position_speed,
    int? Position_direction,
    int? Position_satellites
);
