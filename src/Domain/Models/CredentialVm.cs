namespace TrackHubRouter.Domain.Models;

public readonly record struct CredentialVm(
    Guid CredentialId,
    string Uri,
    string Username,
    string Password,
    string Key,
    string Key2);
