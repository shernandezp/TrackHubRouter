﻿using ManagerApi;
using TrackHub.Router.Infrastructure.ManagerApi;
using TrackHubRouter.Domain.Interfaces.Manager;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAppManagerContext(this IServiceCollection services, bool headerPropagation = true)
    {
        if (headerPropagation)
        {
            services.AddHeaderPropagation(o => o.Headers.Add("Authorization"));

            services.AddHttpClient(Clients.Manager,
                client => client.Timeout = TimeSpan.FromSeconds(30))
                .AddHeaderPropagation();
        }
        services.AddHttpClient(Clients.Manager,
                client => client.Timeout = TimeSpan.FromSeconds(30));

        services.AddScoped<IAccountReader, AccountReader>();
        services.AddScoped<ICredentialWriter, CredentialWriter>();
        services.AddScoped<IDeviceReader, DeviceReader>();
        services.AddScoped<IOperatorReader, OperatorReader>();
        services.AddScoped<ITransporterPositionReader, TransporterPositionReader>();
        services.AddScoped<IPositionWriter, PositionWriter>();

        return services;
    }
}
