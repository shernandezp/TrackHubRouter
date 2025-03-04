﻿namespace TrackHubRouter.Domain.Interfaces.Manager;

public interface IOperatorReader
{
    Task<OperatorVm> GetOperatorAsync(Guid operatorId, CancellationToken cancellationToken);
    Task<IEnumerable<OperatorVm>> GetOperatorsAsync(CancellationToken cancellationToken);
    Task<OperatorVm> GetOperatorByTransporterAsync(Guid transporterId, CancellationToken cancellationToken);
    Task<IEnumerable<OperatorVm>> GetOperatorsByAccountsAsync(Guid accountId, CancellationToken cancellationToken);
}
