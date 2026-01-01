namespace TrackHub.Router.Infrastructure.Navixy.Models;

/// <summary>
/// Response from Navixy tracker/list API
/// </summary>
internal sealed record TrackerListResponse(
    bool Success,
    IEnumerable<Tracker>? List
);

/// <summary>
/// Response from Navixy track/list API
/// </summary>
internal sealed record TrackListResponse(
    bool Success,
    IEnumerable<TrackListEntry>? List
);

/// <summary>
/// Response from Navixy track/read API
/// </summary>
internal sealed record TrackReadResponse(
    bool Success,
    bool Limit_exceeded,
    IEnumerable<TrackPoint>? List
);

/// <summary>
/// Response from Navixy user/auth API
/// </summary>
internal sealed record AuthResponse(
    bool Success,
    string? Hash
);
