namespace TrackHub.Router.Infrastructure.Samsara.Models;

/// <summary>
/// Samsara vehicle stats model with current GPS data
/// </summary>
internal readonly record struct VehicleStats(
    string Id,
    string Name,
    GpsData? Gps
);

/// <summary>
/// Samsara vehicle history model with GPS snapshots
/// </summary>
internal readonly record struct VehicleHistory(
    string Id,
    string Name,
    IEnumerable<GpsData>? Gps
);
