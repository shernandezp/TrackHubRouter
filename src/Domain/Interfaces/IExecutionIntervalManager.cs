namespace TrackHubRouter.Domain.Interfaces;

public interface IExecutionIntervalManager
{
    bool ShouldExecuteTask(AccountSettingsVm account);
    void UpdateLastExecutionTime(Guid accountId);
}
