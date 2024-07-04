namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IOperatorReader
{
    Task<IEnumerable<OperatorVm>> GetOperatorsAsync(Guid userId, CancellationToken cancellationToken);
}
