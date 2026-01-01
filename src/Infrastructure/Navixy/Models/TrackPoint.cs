namespace TrackHub.Router.Infrastructure.Navixy.Models;

/// <summary>
/// Navixy track point (historical position) model
/// Lat/Lng = coordinates, Alt = altitude, Get_time = timestamp (yyyy-MM-dd HH:mm:ss format)
/// </summary>
internal readonly record struct TrackPoint(
    double Lat,
    double Lng,
    int Alt,
    string Get_time,  // yyyy-MM-dd HH:mm:ss format
    int Speed,
    int Heading,
    string? Address,
    int? Precision,
    bool? Gsm_lbs,
    bool? Parking
);
