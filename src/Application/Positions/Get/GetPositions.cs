using System.Runtime.CompilerServices;
using Common.Application.Attributes;
using Common.Application.Interfaces;
using Common.Domain.Constants;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Positions.Get;

[Authorize(Resource = Resources.Positions, Action = Actions.Read)]
public readonly record struct GetPositionsQuery() : IRequest<IEnumerable<PositionVm>>;

public class GetPositionsQueryHandler(IOperatorReader operatorReader, 
    IPositionRegistry positionRegistry,
    IDeviceReader deviceReader,
    IUser user) 
    : IRequestHandler<GetPositionsQuery, IEnumerable<PositionVm>>
{

    private Guid UserId { get; } = user.Id is null ? throw new UnauthorizedAccessException() : new Guid(user.Id);

    public async Task<IEnumerable<PositionVm>> Handle(GetPositionsQuery request, CancellationToken cancellationToken)
    {
        var operators = await operatorReader.GetOperatorsAsync(UserId, cancellationToken);
        var protocols = operators.Select(o => o.ProtocolType).Distinct();
        
        var allPositions = new List<PositionVm>();
        await foreach (var positionsCollection in GetDevicePositionAsync(operators, protocols, cancellationToken))
        {
            allPositions.AddRange(positionsCollection);
        }

        return allPositions;
    }

    private async IAsyncEnumerable<IEnumerable<PositionVm>> GetDevicePositionAsync(
        IEnumerable<OperatorVm> operators,
        IEnumerable<ProtocolType> protocols,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var tasks = positionRegistry.GetReaders(protocols)
            .Select(reader 
                => FetchAndProcessPositionsAsync(reader, operators, cancellationToken));
        var fetchTasks = Task.WhenAll(tasks);

        var results = await fetchTasks;
        foreach (var positions in results)
        {
            if (positions.Any())
            {
                yield return positions;
            }
        }
    }

    private async Task<IEnumerable<PositionVm>> FetchAndProcessPositionsAsync(
        IPositionReader reader,
        IEnumerable<OperatorVm> operators,
        CancellationToken cancellationToken)
    {
        var @operator = operators.FirstOrDefault(o => o.ProtocolType == reader.Protocol);
        if (@operator.Credential is not null)
        {
            await reader.Init(@operator.Credential.Value, cancellationToken);
            var devices = await deviceReader.GetDevicesByOperatorAsync(UserId, @operator.OperatorId, cancellationToken);
            return await reader.GetDevicePositionAsync(devices, cancellationToken);
        }
        return [];
    }

}
