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
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHub.Router.Infrastructure.Traccar.Adapters;

// Adapter is required here to make the Init method asynchronous
// This class is an adapter that implements the IExternalDeviceReader interface
// It wraps the DeviceReader class and provides an asynchronous implementation of the methods
public sealed class DeviceReaderAdapter(DeviceReader deviceReader) : IExternalDeviceReader
{
    // Gets the protocol type from the wrapped DeviceReader instance
    public ProtocolType Protocol => deviceReader.Protocol;

    /// <summary>
    /// Initializes the device reader asynchronously
    /// </summary>
    /// <param name="credential"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>It runs the Init method of the wrapped DeviceReader instance in a separate task</returns>
    public Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
        => Task.Run(() => deviceReader.Init(credential), cancellationToken);

    /// <summary>
    /// Retrieves a device asynchronously
    /// </summary>
    /// <param name="deviceDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>It calls the GetDeviceAsync method of the wrapped DeviceReader instance</returns>
    public Task<DeviceVm> GetDeviceAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
        => deviceReader.GetDeviceAsync(deviceDto, cancellationToken);

    /// <summary>
    /// Retrieves multiple devices asynchronously
    /// </summary>
    /// <param name="devices"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>It calls the GetDevicesAsync method of the wrapped DeviceReader instance</returns>
    public Task<IEnumerable<DeviceVm>> GetDevicesAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
        => deviceReader.GetDevicesAsync(devices, cancellationToken);

    /// <summary>
    /// Retrieves all devices asynchronously
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>It calls the GetDevicesAsync method of the wrapped DeviceReader instance</returns>
    public Task<IEnumerable<DeviceVm>> GetDevicesAsync(CancellationToken cancellationToken)
        => deviceReader.GetDevicesAsync(cancellationToken);
}
