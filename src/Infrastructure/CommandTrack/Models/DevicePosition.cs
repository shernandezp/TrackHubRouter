namespace TrackHub.Router.Infrastructure.CommandTrack.Models;

internal readonly record struct DevicePosition(
    int Id,
    string Serial,
    string Plate,
    double Latitude,
    double Longitude,
    double? Altitude,
    DateTimeOffset DeviceDateTime,
    double Speed,
    double Course,
    string Address,
    double DistanceToAddress,
    string City,
    string State,
    string Country,
    bool Ignition,
    double? Satellites,
    double? Mileage,
    double? Hourmeter,
    double? Temperature
    );
