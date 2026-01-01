using Common.Domain.Enums;
using Microsoft.Extensions.Configuration;
using TrackHub.Router.Infrastructure.Common;
using TrackHub.Router.Infrastructure.Common.Helpers;
using TrackHubRouter.Application.Devices.Registry;
using TrackHubRouter.Application.PingOperator;
using TrackHubRouter.Application.DevicePositions.Registry;
using TrackHubRouter.Domain.Interfaces.Operator;
using TrackHubRouter.Domain.Interfaces.Registry;
using CommandTrack = TrackHub.Router.Infrastructure.CommandTrack;
using GeoTab = TrackHub.Router.Infrastructure.Geotab;
using GpsGate = TrackHub.Router.Infrastructure.GpsGate;
using Traccar = TrackHub.Router.Infrastructure.Traccar;
using Wialon = TrackHub.Router.Infrastructure.Wialon;
using Samsara = TrackHub.Router.Infrastructure.Samsara;
using Navixy = TrackHub.Router.Infrastructure.Navixy;
using Flespi = TrackHub.Router.Infrastructure.Flespi;

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
            },
            {
                ProtocolType.GpsGate.ToString(), services =>
                {
                    services.AddScoped<GpsGate.DeviceReader>();
                    services.AddScoped<GpsGate.PositionReader>();
                    services.AddScoped<IPositionReader, GpsGate.Adapters.PositionReaderAdapter>(provider
                        => new GpsGate.Adapters.PositionReaderAdapter(provider.GetRequiredService<GpsGate.PositionReader>()));
                    services.AddScoped<IExternalDeviceReader, GpsGate.Adapters.DeviceReaderAdapter>(provider
                        => new GpsGate.Adapters.DeviceReaderAdapter(provider.GetRequiredService<GpsGate.DeviceReader>()));
                    services.AddScoped<IConnectivityTester, GpsGate.ConnectivityTester>();
                }
            },
            {
                ProtocolType.Wialon.ToString(), services =>
                {
                    services.AddScoped<IExternalDeviceReader, Wialon.DeviceReader>();
                    services.AddScoped<IPositionReader, Wialon.PositionReader>();
                    services.AddScoped<IConnectivityTester, Wialon.ConnectivityTester>();
                }
            },
            {
                ProtocolType.Samsara.ToString(), services =>
                {
                    services.AddScoped<Samsara.DeviceReader>();
                    services.AddScoped<Samsara.PositionReader>();
                    services.AddScoped<IPositionReader, Samsara.Adapters.PositionReaderAdapter>(provider
                        => new Samsara.Adapters.PositionReaderAdapter(provider.GetRequiredService<Samsara.PositionReader>()));
                    services.AddScoped<IExternalDeviceReader, Samsara.Adapters.DeviceReaderAdapter>(provider
                        => new Samsara.Adapters.DeviceReaderAdapter(provider.GetRequiredService<Samsara.DeviceReader>()));
                    services.AddScoped<IConnectivityTester, Samsara.ConnectivityTester>();
                }
            },
            {
                ProtocolType.Navixy.ToString(), services =>
                {
                    services.AddScoped<IExternalDeviceReader, Navixy.DeviceReader>();
                    services.AddScoped<IPositionReader, Navixy.PositionReader>();
                    services.AddScoped<IConnectivityTester, Navixy.ConnectivityTester>();
                }
            },
            {
                 ProtocolType.Flespi.ToString(), services =>
                 {
                     services.AddScoped<Flespi.DeviceReader>();
                     services.AddScoped<Flespi.PositionReader>();
                     services.AddScoped<IPositionReader, Flespi.Adapters.PositionReaderAdapter>(provider
                         => new Flespi.Adapters.PositionReaderAdapter(provider.GetRequiredService<Flespi.PositionReader>()));
                     services.AddScoped<IExternalDeviceReader, Flespi.Adapters.DeviceReaderAdapter>(provider
                         => new Flespi.Adapters.DeviceReaderAdapter(provider.GetRequiredService<Flespi.DeviceReader>()));
                     services.AddScoped<IConnectivityTester, Flespi.ConnectivityTester>();
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
