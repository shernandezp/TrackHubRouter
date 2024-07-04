namespace TrackHubRouter.Domain.Models;

public readonly record struct CredentialTokenVm(
    Guid CredentialId,
    string Uri,
    string Username,
    string Password,
    string Key,
    string Key2,
    string? Token,
    DateTime? TokenExpiration,
    string? RefreshToken,
    DateTime? RefreshTokenExpiration);
