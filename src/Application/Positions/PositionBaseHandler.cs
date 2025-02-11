

using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Extensions;

namespace TrackHubRouter.Application.Positions;

public abstract class PositionBaseHandler
{
    /// <summary>
    /// Retrieves the device positions asynchronously
    /// </summary>
    /// <param name="operator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>returns the collection of PositionVm</returns>
    protected async Task<IEnumerable<PositionVm>> GetDevicePositionAsync(
        IPositionRegistry positionRegistry,
        IDeviceTransporterReader deviceReader,
        string encryptionKey,
        OperatorVm @operator,
        DateTimeOffset from,
        DateTimeOffset to,
        Guid transporterId,
        CancellationToken cancellationToken)
    {
        var reader = positionRegistry.GetReader((ProtocolType)@operator.ProtocolTypeId);
        if (@operator.Credential is not null)
        {
            await reader.Init(@operator.Credential.Value.Decrypt(encryptionKey), cancellationToken);
            var device = await deviceReader.GetDevicesTransporterAsync(transporterId, cancellationToken);
            var positions = await reader.GetPositionAsync(from, to, device, cancellationToken);
            return positions;
        }
        return [];
    }
}
