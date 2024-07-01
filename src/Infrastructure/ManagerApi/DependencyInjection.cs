using Common.Domain.Constants;
using ManagerApi;
using TrackHubRouter.Domain.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAppManagerContext(this IServiceCollection services)
    {
        services.AddHeaderPropagation(o => o.Headers.Add("Authorization"));

        services.AddHttpClient(Clients.Manager,
            client => client.Timeout = TimeSpan.FromSeconds(30))
            .AddHeaderPropagation();

        services.AddScoped<ICredentialReader, CredentialReader>();
        services.AddScoped<ICredentialWriter, CredentialWriter>();

        return services;
    }
}
