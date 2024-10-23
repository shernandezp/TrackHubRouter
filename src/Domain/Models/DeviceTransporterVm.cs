namespace TrackHubRouter.Domain.Models;

public readonly record struct DeviceTransporterVm(
    Guid TransporterId,
    int Identifier,
    string Serial,
    string Name,
    string TransporterType,
    short TransporterTypeId
    );
