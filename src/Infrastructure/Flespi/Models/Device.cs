namespace TrackHub.Router.Infrastructure.Flespi.Models;

/// <summary>
/// Flespi device model
/// </summary>
internal readonly record struct Device(
    long Id,
    string Name,
    string? Ident,
    long? Device_type_id
);
