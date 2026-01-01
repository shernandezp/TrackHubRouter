namespace TrackHub.Router.Infrastructure.Navixy.Models;

/// <summary>
/// Navixy tracker (device) model
/// Tracker_id = device id, Imei = unique identifier, Label = device name
/// </summary>
internal readonly record struct Tracker(
    long Tracker_id,
    string Imei,
    string Label,
    TrackerLastUpdate? Last_update
);

/// <summary>
/// Last known state of the tracker
/// </summary>
internal readonly record struct TrackerLastUpdate(
    string? Time,
    double? Lat,
    double? Lng,
    int? Speed,
    int? Heading
);
