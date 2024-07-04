namespace TrackHubRouter.Domain.Models;

public readonly record struct OperatorVm(
    Guid OperatorId,
    ProtocolType ProtocolType,
    CredentialTokenVm? Credential
    );
