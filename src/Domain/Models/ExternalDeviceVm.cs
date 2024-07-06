namespace TrackHubRouter.Domain.Models;

public readonly record struct ExternalDeviceVm(
    Guid? DeviceId,
    int Identifier,
    string Serial,
    string Name
    );
