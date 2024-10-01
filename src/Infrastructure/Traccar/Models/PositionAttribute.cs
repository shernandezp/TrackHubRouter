namespace TrackHub.Router.Infrastructure.Traccar.Models;

internal readonly record struct PositionAttribute(
    bool? Ignition,
    double? TotalDistance,
    double? Odometer
    );
