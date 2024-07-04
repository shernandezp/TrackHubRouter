using System.Runtime.CompilerServices;
using Common.Application.Attributes;
using Common.Domain.Constants;
using Common.Domain.Enums;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Interfaces.Operator;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Positions.GetByUser;

[Authorize(Resource = Resources.Positions, Action = Actions.Read)]
public readonly record struct GetPositionByUserQuery(Guid UserId) : IRequest<IEnumerable<PositionVm>>;

public class GetPositionByUserQueryHandler(IOperatorReader operatorReader, IPositionRegistry positionRegistry) 
    : IRequestHandler<GetPositionByUserQuery, IEnumerable<PositionVm>>
{
    public async Task<IEnumerable<PositionVm>> Handle(GetPositionByUserQuery request, CancellationToken cancellationToken)
    {
        var operators = await operatorReader.GetOperatorsAsync(request.UserId, cancellationToken);
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

    private static async Task<IEnumerable<PositionVm>> FetchAndProcessPositionsAsync(
        IPositionReader reader,
        IEnumerable<OperatorVm> operators,
        CancellationToken cancellationToken)
    {
        var credential = operators.FirstOrDefault(o => o.ProtocolType == reader.Protocol).Credential;
        if (credential is not null)
        {
            await reader.Init(credential.Value, cancellationToken);
            //TO DO: get devices by operator
            return await reader.GetDevicePositionAsync([], cancellationToken);
        }
        return [];
    }

}
