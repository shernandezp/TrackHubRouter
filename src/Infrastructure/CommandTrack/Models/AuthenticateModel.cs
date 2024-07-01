namespace TrackHub.Router.Infrastructure.CommandTrack.Models;

internal readonly record struct AuthenticateModel
(
    string Username,
    string Password,
    string UniqueId
);
