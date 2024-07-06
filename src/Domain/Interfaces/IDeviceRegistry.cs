using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHubRouter.Domain.Interfaces;

public interface IDeviceRegistry
{
    IExternalDeviceReader GetReader(ProtocolType type);
    IEnumerable<IExternalDeviceReader> GetReaders(IEnumerable<ProtocolType> types);
}
