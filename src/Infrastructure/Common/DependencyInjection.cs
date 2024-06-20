using Common;
using TrackHubRouter.Domain.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonContext(this IServiceCollection services)
    {
        services.AddScoped<ICredentialHttpClientFactory, CredentialHttpClientFactory>();
        return services;
    }
}
