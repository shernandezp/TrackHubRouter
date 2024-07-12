namespace TrackHub.Router.Infrastructure.Traccar.Models;
internal readonly record struct Device(
    int Id,
    string Name,
    string UniqueId,
    string Status,
    bool Disabled,
    DateTimeOffset? LastUpdate,
    int PositionId,
    int GroupId,
    string Phone,
    string Model,
    string Contact,
    string Category,
    object Attributes
    );
