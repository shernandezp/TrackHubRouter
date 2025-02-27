namespace TrackHubRouter.Domain.Models;

public readonly record struct TripPointVm(

    double Latitude,
    double Longitude,
    DateTimeOffset DeviceDateTime,
    double Speed,
    double? Course,
    int? EventId,
    bool Movement
);
