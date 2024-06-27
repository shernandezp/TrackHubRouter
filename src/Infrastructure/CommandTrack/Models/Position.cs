namespace TrackHub.Router.Infrastructure.CommandTrack.Models;

internal readonly record struct Position
(
    int PositionId,
    string Serial,
    string Plate,
    double Latitude,
    double Longitude,
    double? Altitude,
    DateTimeOffset DeviceDateTime,
    DateTimeOffset ServerDateTime,
    double Speed,
    double Course,
    int EventId,
    string Address,
    double DistanceToAddress,
    string City,
    string State,
    string Country,
    bool Ignition,
    double? Satellites,
    double? Mileage,
    double? Temperature
);
