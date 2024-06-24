namespace TrackHub.Router.Infrastructure.Traccar.Entities;

internal readonly record struct Position(
    int Id,
    int DeviceId,
    string Protocol,
    DateTimeOffset DeviceTime,
    DateTimeOffset FixTime,
    DateTimeOffset ServerTime,
    bool Outdated,
    bool Valid,
    double Latitude,
    double Longitude,
    double Altitude,
    double Speed,
    double Course,
    string Address,
    double Accuracy,
    string Attributes
    );
