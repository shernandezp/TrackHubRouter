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
using TrackHubRouter.Domain.Records;

namespace TrackHub.Router.Infrastructure.GpsGate.Adapters;

public sealed class PositionReaderAdapter(PositionReader positionReader) : IPositionReader
{
    public ProtocolType Protocol => positionReader.Protocol;

    public Task Init(CredentialTokenDto credential, CancellationToken cancellationToken = default)
        => Task.Run(() => positionReader.Init(credential), cancellationToken);

    public Task<PositionVm> GetDevicePositionAsync(DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
        => positionReader.GetDevicePositionAsync(deviceDto, cancellationToken);

    public Task<IEnumerable<PositionVm>> GetDevicePositionAsync(IEnumerable<DeviceTransporterVm> devices, CancellationToken cancellationToken)
        => positionReader.GetDevicePositionAsync(devices, cancellationToken);

    public Task<IEnumerable<PositionVm>> GetPositionAsync(DateTimeOffset from, DateTimeOffset to, DeviceTransporterVm deviceDto, CancellationToken cancellationToken)
        => positionReader.GetPositionAsync(from, to, deviceDto, cancellationToken);

}
