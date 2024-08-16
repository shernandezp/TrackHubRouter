namespace TrackHubRouter.Domain.Interfaces;

public interface IConnectivityTester
{
    ProtocolType Protocol { get; }
    Task Ping(CredentialTokenDto credential, CancellationToken cancellationToken);
}
