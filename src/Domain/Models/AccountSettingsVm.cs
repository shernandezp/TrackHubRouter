namespace TrackHubRouter.Domain.Models;

public readonly record struct AccountSettingsVm(
    Guid AccountId,
    bool StoreLastPosition,
    int StoringTimeLapse
    );
