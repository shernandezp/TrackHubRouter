using Common.Domain.Enums;
using Microsoft.Extensions.Configuration;
using TrackHub.Router.Infrastructure.Common;
using TrackHub.Router.Infrastructure.Common.Helpers;
using TrackHubRouter.Application.Devices.Registry;
using TrackHubRouter.Application.PingOperator;
using TrackHubRouter.Application.Positions.Registry;
using TrackHubRouter.Domain.Interfaces.Operator;
using TrackHubRouter.Domain.Interfaces.Registry;
using CommandTrack = TrackHub.Router.Infrastructure.CommandTrack;
using GeoTab = TrackHub.Router.Infrastructure.Geotab;
using Traccar = TrackHub.Router.Infrastructure.Traccar;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonContext(this IServiceCollection services, IConfiguration configuration)
    {
        var protocols = configuration.GetSection("AppSettings:Protocols").Get<IEnumerable<string>>();
        Guard.Against.Null(protocols, message: $"Client configuration for Protocols not loaded");

        var protocolRegistrations = new Dictionary<string, Action<IServiceCollection>>
        {
            {
                ProtocolType.CommandTrack.ToString(), services =>
                {
                    services.AddScoped<IExternalDeviceReader, CommandTrack.DeviceReader>();
                    services.AddScoped<IPositionReader, CommandTrack.PositionReader>();
                    services.AddScoped<IConnectivityTester, CommandTrack.ConnectivityTester>();
                }
            },
            {
                ProtocolType.GeoTab.ToString(), services =>
                {
                    services.AddScoped<IExternalDeviceReader, GeoTab.DeviceReader>();
                    services.AddScoped<IPositionReader, GeoTab.PositionReader>();
                    services.AddScoped<IConnectivityTester, GeoTab.ConnectivityTester>();
                }
            },
            {
                ProtocolType.Traccar.ToString(), services =>
                {
                    services.AddScoped<Traccar.DeviceReader>();
                    services.AddScoped<Traccar.PositionReader>();
                    services.AddScoped<IPositionReader, Traccar.Adapters.PositionReaderAdapter>(provider
                        => new Traccar.Adapters.PositionReaderAdapter(provider.GetRequiredService<Traccar.PositionReader>()));
                    services.AddScoped<IExternalDeviceReader, Traccar.Adapters.DeviceReaderAdapter>(provider
                        => new Traccar.Adapters.DeviceReaderAdapter(provider.GetRequiredService<Traccar.DeviceReader>()));
                    services.AddScoped<IConnectivityTester, Traccar.ConnectivityTester>();
                }
            }
        };

        foreach (var protocol in protocols)
        {
            if (protocolRegistrations.TryGetValue(protocol, out var registerServices))
            {
                registerServices(services);
            }
        }

        services.AddSingleton<IExecutionIntervalManager, ExecutionIntervalManager>();
        services.AddScoped<ICredentialHttpClientFactory, CredentialHttpClientFactory>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddScoped<IRefreshTokenHelper, RefreshTokenHelper>();
        services.AddSingleton<IConnectivityRegistry, ConnectivityRegistry>();
        services.AddSingleton<IDeviceRegistry, DeviceRegistry>();
        services.AddSingleton<IPositionRegistry, PositionRegistry>();
        return services;
    }
}
