namespace TrackHub.Router.Infrastructure.Samsara.Models;

/// <summary>
/// Samsara GPS data model
/// </summary>
internal readonly record struct GpsData(
    DateTime Time,
    double Latitude,
    double Longitude,
    double HeadingDegrees,
    double SpeedMilesPerHour,
    bool IsEcuSpeed,
    ReverseGeo? ReverseGeo
);

/// <summary>
/// Reverse geocoding data
/// </summary>
internal readonly record struct ReverseGeo(
    string FormattedLocation
);
