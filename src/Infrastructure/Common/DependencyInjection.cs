using TrackHub.Router.Infrastructure.Common;
using TrackHub.Router.Infrastructure.Common.Helpers;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonContext(this IServiceCollection services)
    {
        services.AddScoped<ICredentialHttpClientFactory, CredentialHttpClientFactory>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddScoped<IRefreshTokenHelper, RefreshTokenHelper>();
        return services;
    }
}
