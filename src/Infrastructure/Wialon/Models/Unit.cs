namespace TrackHub.Router.Infrastructure.Wialon.Models;

/// <summary>
/// Wialon unit (device) model
/// Id = unit id, Nm = name, Cls = class (2 for avl_unit), Uid = unique id, Pos = last position
/// </summary>
internal readonly record struct Unit(
    long Id,         // unit id
    string Nm,       // name
    int Cls,         // class (should be 2 for units)
    string? Uid,     // unique id
    Position? Pos    // last position
);
