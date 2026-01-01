namespace TrackHub.Router.Infrastructure.Navixy.Models;

/// <summary>
/// Navixy track list entry (summary of a track segment)
/// </summary>
internal readonly record struct TrackListEntry(
    long Id,
    string Start_date,  // yyyy-MM-dd HH:mm:ss format
    string End_date,    // yyyy-MM-dd HH:mm:ss format
    int Points
);
