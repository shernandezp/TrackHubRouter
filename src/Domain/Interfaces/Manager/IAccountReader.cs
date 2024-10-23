namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IAccountReader
{
    Task<AccountSettingsVm> GetAccountSettingsAsync(Guid operatorId, CancellationToken cancellationToken);
}
