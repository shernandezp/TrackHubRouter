namespace TrackHubRouter.Domain.Records;

public readonly record struct CredentialTokenDto(
    Guid CredentialId,
    string Uri,
    string Username,
    string Password,
    string? Key,
    string? Key2,
    string? Token,
    DateTime? TokenExpiration,
    string? RefreshToken,
    DateTime? RefreshTokenExpiration);
