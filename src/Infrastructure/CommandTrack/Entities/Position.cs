namespace TrackHub.Router.Infrastructure.CommandTrack.Entities;

internal readonly record struct Position
(
    long PositionId,
    string Serial,
    string Plate,
    double Latitude,
    double Longitude,
    int Altitude,
    DateTimeOffset DeviceDateTime,
    DateTimeOffset ServerDateTime,
    int Speed,
    int Course,
    int? EventId,
    string? Address,
    string? City,
    string? State,
    string? Country,
    bool Ignition,
    int Satellites,
    double Mileage,
    double HobbsMeter,
    double Temperature
);
