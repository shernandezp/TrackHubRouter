namespace TrackHub.Router.Infrastructure.Wialon.Models;

/// <summary>
/// Wialon position data model
/// T = timestamp (unix), X = longitude, Y = latitude, Z = altitude, S = speed, C = course, Sc = satellites
/// </summary>
internal readonly record struct Position(
    long T,      // timestamp (unix)
    double X,    // longitude
    double Y,    // latitude
    double Z,    // altitude
    double S,    // speed
    int C,       // course
    int? Sc      // satellites
);
