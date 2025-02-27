using Ardalis.GuardClauses;
using Common.Application.Attributes;
using Common.Domain.Constants;
using Microsoft.Extensions.Configuration;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Positions.Queries.GetRange;

[Authorize(Resource = Resources.Positions, Action = Actions.Read)]
public readonly record struct GetPositionsRecordQuery(Guid TransporterId, DateTimeOffset From, DateTimeOffset To) : IRequest<IEnumerable<PositionVm>>;

public class GetPositionsRecordQueryHandler(
        IConfiguration configuration,
        IOperatorReader operatorReader,
        IPositionRegistry positionRegistry,
        IDeviceTransporterReader deviceReader)
        : PositionBaseHandler, IRequestHandler<GetPositionsRecordQuery, IEnumerable<PositionVm>>
{
    private string? EncryptionKey { get; } = configuration["AppSettings:EncryptionKey"];

    /// <summary>
    /// Retrieves the operator, and device positions asynchronously
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the collection of PositionVm</returns>
    public async Task<IEnumerable<PositionVm>> Handle(GetPositionsRecordQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(EncryptionKey, message: "Credential key not found.");
        var @operator = await operatorReader.GetOperatorByTransporterAsync(request.TransporterId, cancellationToken);
        return await GetDevicePositionAsync(
            positionRegistry,
            deviceReader,
            EncryptionKey,
            @operator, 
            request.From, 
            request.To, 
            request.TransporterId, 
            cancellationToken);

    }

}
