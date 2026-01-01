namespace TrackHub.Router.Infrastructure.Wialon.Models;

/// <summary>
/// Response from Wialon messages/load_interval API
/// </summary>
internal sealed record MessageResponse(
    IEnumerable<Message>? Messages,
    int Count
);

/// <summary>
/// Wialon message (historical position data)
/// T = timestamp, Tp = type, Pos = position, P = params
/// </summary>
internal readonly record struct Message(
    long T,              // timestamp
    string? Tp,          // type
    Position? Pos,       // position
    MessageParams? P     // params
);

/// <summary>
/// Message parameters containing sensor data
/// Odo = odometer, Pwr_ext = external power, Hdop = hdop
/// </summary>
internal readonly record struct MessageParams(
    double? Odo,         // odometer
    double? Pwr_ext,     // external power
    double? Hdop         // hdop
);
