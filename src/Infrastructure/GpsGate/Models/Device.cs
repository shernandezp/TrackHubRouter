namespace TrackHub.Router.Infrastructure.GpsGate.Models;

internal readonly record struct Device(
    int Id,
    string Name,
    string IMEI,
    DateTimeOffset? TimeStamp
    );
