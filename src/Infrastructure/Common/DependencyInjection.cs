// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Common.Domain.Enums;
using Microsoft.Extensions.Configuration;
using TrackHub.Router.Infrastructure.Common;
using TrackHub.Router.Infrastructure.Common.Helpers;
using TrackHubRouter.Application.Devices.Registry;
using TrackHubRouter.Application.PingOperator;
using TrackHubRouter.Application.DevicePositions.Registry;
using TrackHubRouter.Domain.Interfaces.Registry;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonContext(this IServiceCollection services, IConfiguration configuration)
    {
        var protocols = configuration.GetSection("AppSettings:Protocols").Get<IEnumerable<string>>();
        Guard.Against.Null(protocols, message: $"Client configuration for Protocols not loaded");

        var protocolsWithAdapters = new HashSet<string>
        {
            ProtocolType.Traccar.ToString(),
            ProtocolType.GpsGate.ToString(),
            ProtocolType.Samsara.ToString(),
            ProtocolType.Flespi.ToString()
        };

        foreach (var protocol in protocols)
        {
            var hasAdapters = protocolsWithAdapters.Contains(protocol);
            services.RegisterProtocol(protocol, hasAdapters);
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
