using TrackHubRouter.Domain.Models;

namespace TrackHub.Router.Infrastructure.Common;

public class ExecutionIntervalManager : IExecutionIntervalManager
{
    private readonly Dictionary<Guid, DateTimeOffset> _lastExecutionTimes = [];

    public bool ShouldExecuteTask(AccountSettingsVm account)
    {
        if (!_lastExecutionTimes.TryGetValue(account.AccountId, out var lastExecutionTime))
        {
            return true; // If there's no record of the last execution, execute the task
        }

        var interval = TimeSpan.FromSeconds(account.StoringInterval);
        return (DateTimeOffset.Now - lastExecutionTime) >= interval;
    }

    public void UpdateLastExecutionTime(Guid accountId)
    {
        _lastExecutionTimes[accountId] = DateTimeOffset.Now;
    }
}
