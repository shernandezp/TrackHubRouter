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

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Common.Helpers;

public static class ProtocolRegistrationExtensions
{
    public static IServiceCollection RegisterProtocol(
        this IServiceCollection services,
        string protocolName,
        bool hasAdapters = false)
    {
        var protocolAssembly = GetProtocolAssembly(protocolName);
        if (protocolAssembly is null)
        {
            return services;
        }

        var protocolNamespace = $"TrackHub.Router.Infrastructure.{protocolName}";

        if (hasAdapters)
        {
            RegisterWithAdapters(services, protocolAssembly, protocolNamespace);
        }
        else
        {
            RegisterWithoutAdapters(services, protocolAssembly, protocolNamespace);
        }

        return services;
    }

    private static void RegisterWithAdapters(
        IServiceCollection services,
        Assembly assembly,
        string protocolNamespace)
    {
        var deviceReaderType = assembly.GetType($"{protocolNamespace}.DeviceReader");
        var positionReaderType = assembly.GetType($"{protocolNamespace}.PositionReader");
        var deviceReaderAdapterType = assembly.GetType($"{protocolNamespace}.Adapters.DeviceReaderAdapter");
        var positionReaderAdapterType = assembly.GetType($"{protocolNamespace}.Adapters.PositionReaderAdapter");
        var connectivityTesterType = assembly.GetType($"{protocolNamespace}.ConnectivityTester");

        if (deviceReaderType is not null)
        {
            services.AddScoped(deviceReaderType);
        }

        if (positionReaderType is not null)
        {
            services.AddScoped(positionReaderType);
        }

        if (deviceReaderAdapterType is not null && deviceReaderType is not null)
        {
            services.AddScoped(typeof(IExternalDeviceReader), provider =>
            {
                var concreteReader = provider.GetRequiredService(deviceReaderType);
                return Activator.CreateInstance(deviceReaderAdapterType, concreteReader)!;
            });
        }

        if (positionReaderAdapterType is not null && positionReaderType is not null)
        {
            services.AddScoped(typeof(IPositionReader), provider =>
            {
                var concreteReader = provider.GetRequiredService(positionReaderType);
                return Activator.CreateInstance(positionReaderAdapterType, concreteReader)!;
            });
        }

        if (connectivityTesterType is not null)
        {
            services.AddScoped(typeof(IConnectivityTester), connectivityTesterType);
        }
    }

    private static void RegisterWithoutAdapters(
        IServiceCollection services,
        Assembly assembly,
        string protocolNamespace)
    {
        var deviceReaderType = assembly.GetType($"{protocolNamespace}.DeviceReader");
        var positionReaderType = assembly.GetType($"{protocolNamespace}.PositionReader");
        var connectivityTesterType = assembly.GetType($"{protocolNamespace}.ConnectivityTester");

        if (deviceReaderType is not null && typeof(IExternalDeviceReader).IsAssignableFrom(deviceReaderType))
        {
            services.AddScoped(typeof(IExternalDeviceReader), deviceReaderType);
        }

        if (positionReaderType is not null && typeof(IPositionReader).IsAssignableFrom(positionReaderType))
        {
            services.AddScoped(typeof(IPositionReader), positionReaderType);
        }

        if (connectivityTesterType is not null && typeof(IConnectivityTester).IsAssignableFrom(connectivityTesterType))
        {
            services.AddScoped(typeof(IConnectivityTester), connectivityTesterType);
        }
    }

    private static Assembly? GetProtocolAssembly(string protocolName)
    {
        var protocolNamespace = $"TrackHub.Router.Infrastructure.{protocolName}";

        // First, check if the assembly is already loaded by looking for types in the expected namespace
        // that implement our protocol interfaces
        var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => !a.IsDynamic &&
                a.GetTypes().Any(t =>
                    t.Namespace?.StartsWith(protocolNamespace, StringComparison.Ordinal) == true &&
                    (typeof(IExternalDeviceReader).IsAssignableFrom(t) ||
                     typeof(IPositionReader).IsAssignableFrom(t) ||
                     typeof(IConnectivityTester).IsAssignableFrom(t))));

        if (loadedAssembly is not null)
        {
            return loadedAssembly;
        }

        // If not loaded, try to load by expected assembly name
        try
        {
            var assemblyName = $"TrackHub.Router.Infrastructure.{protocolName}";
            return Assembly.Load(assemblyName);
        }
        catch
        {
            return null;
        }
    }
}
