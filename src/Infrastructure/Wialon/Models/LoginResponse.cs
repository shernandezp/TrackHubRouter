namespace TrackHub.Router.Infrastructure.Wialon.Models;

/// <summary>
/// Response from Wialon token/login API
/// Eid = Session ID, User = User name, Uid = User ID
/// </summary>
internal sealed record LoginResponse(
    string Eid,   // Session ID
    string User,  // User name
    long Uid      // User ID
);
