// Copyright (c) 2026 Sergio Hernandez. All rights reserved.
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

using TrackHub.Router.Infrastructure.TelemetryApi;

namespace Microsoft.Extensions.DependencyInjection;

// Positions / history / health / sync-run readers and writers re-pointed to the TrackHub.Telemetry
// service (spec 01.3 §5.5). Device/operator/account/geocoding readers stay on ManagerApi.
public static class TelemetryApiDependencyInjection
{
    public static IServiceCollection AddAppTelemetryContext(this IServiceCollection services, bool headerPropagation = true)
    {
        if (headerPropagation)
        {
            services.AddHeaderPropagation(o => o.Headers.Add("Authorization"));

            services.AddHttpClient(Clients.Telemetry,
                client => client.Timeout = TimeSpan.FromSeconds(30))
                .AddHeaderPropagation();
        }
        else
        {
            services.AddHttpClient(Clients.Telemetry,
                    client => client.Timeout = TimeSpan.FromSeconds(30));
        }

        // Dedicated client for system (client-credentials) calls: never propagates the user's
        // Authorization header, so the factory applies the Router's own service identity.
        services.AddHttpClient($"{Clients.Telemetry}AsService",
                client => client.Timeout = TimeSpan.FromSeconds(30));

        services.AddScoped<ITransporterPositionReader, TransporterPositionReader>();
        services.AddScoped<IPositionWriter, PositionWriter>();
        services.AddScoped<IPositionSystemWriter, PositionSystemWriter>();
        services.AddScoped<IPositionHistoryReader, PositionHistoryReader>();
        services.AddScoped<IPositionHistorySystemWriter, PositionHistorySystemWriter>();
        services.AddScoped<IOperatorSyncRunWriter, OperatorSyncRunWriter>();
        services.AddScoped<IOperatorHealthCheckWriter, OperatorHealthCheckWriter>();
        services.AddScoped<IOperatorHealthCheckSystemWriter, OperatorHealthCheckSystemWriter>();
        services.AddScoped<IResolvedAddressWriter, ResolvedAddressWriter>();
        services.AddScoped<IResolvedAddressSystemWriter, ResolvedAddressSystemWriter>();

        return services;
    }
}
