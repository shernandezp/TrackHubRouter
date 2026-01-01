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

using ManagerApi;
using TrackHub.Router.Infrastructure.ManagerApi;

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
        else
        {
            services.AddHttpClient(Clients.Manager,
                    client => client.Timeout = TimeSpan.FromSeconds(30));
        }

        services.AddScoped<IAccountReader, AccountReader>();
        services.AddScoped<ICredentialWriter, CredentialWriter>();
        services.AddScoped<IDeviceTransporterReader, DeviceTransporterReader>();
        services.AddScoped<IOperatorReader, OperatorReader>();
        services.AddScoped<ITransporterPositionReader, TransporterPositionReader>();
        services.AddScoped<ITransporterTypeReader, TransporterTypeReader>();
        services.AddScoped<IPositionWriter, PositionWriter>();

        return services;
    }
}
