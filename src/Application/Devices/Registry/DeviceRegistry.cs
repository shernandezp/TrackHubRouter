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

namespace TrackHubRouter.Application.Devices.Registry;

// This class represents a device registry that manages external device readers.
public class DeviceRegistry(IServiceScopeFactory scopeFactory) : IDeviceRegistry
{

    // Retrieves all external device readers that support the specified protocol types.
    // Parameters:
    //   types - The collection of protocol types.
    // Returns:
    //   An IEnumerable of IExternalDeviceReader representing the matching device readers.
    public IEnumerable<IExternalDeviceReader> GetReaders(IEnumerable<ProtocolType> types)
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IExternalDeviceReader>()
            .Where(reader => types.Contains(reader.Protocol));
    }

    // Retrieves the first external device reader that supports the specified protocol type.
    // Parameters:
    //   type - The protocol type.
    // Returns:
    //   An IExternalDeviceReader representing the matching device reader.
    public IExternalDeviceReader GetReader(ProtocolType type)
    {
        using var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetServices<IExternalDeviceReader>()
            .First(reader => reader.Protocol == type);
    }

}
