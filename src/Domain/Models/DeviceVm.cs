namespace TrackHubRouter.Domain.Models;
public readonly record struct DeviceVm(
    Guid? DeviceId,
    int Identifier,
    string Serial,
    string Name,
    short DeviceTypeId,
    short TransporterTypeId
    );
