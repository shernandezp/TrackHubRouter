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

using Common.Application.Attributes;
using Common.Domain.Constants;
using TrackHub.Router.Domain.Constants;
using TrackHub.Router.Domain.Enumerators;
using TrackHub.Router.Domain.Models;

namespace TrackHub.Router.Application.Providers.Queries;

// The capability matrix lets clients distinguish "this GPS provider cannot do it" from a
// TrackHub feature gate BEFORE issuing a request that would fail with
// PROVIDER_CAPABILITY_NOT_SUPPORTED (e.g. disable the provider-history option for a GpsGate
// operator instead of surfacing an error).
[Authorize(Resource = Resources.Positions, Action = Actions.Read)]
[PlatformScoped("Static per-protocol provider capabilities (ProviderCapabilityCatalog): platform reference data describing external GPS provider APIs; no tenant owns a row.")]
public readonly record struct GetProviderCapabilitiesQuery : IRequest<IEnumerable<ProviderCapabilitiesVm>>;

public class GetProviderCapabilitiesQueryHandler : IRequestHandler<GetProviderCapabilitiesQuery, IEnumerable<ProviderCapabilitiesVm>>
{
    public Task<IEnumerable<ProviderCapabilitiesVm>> Handle(GetProviderCapabilitiesQuery request, CancellationToken cancellationToken)
        => Task.FromResult(ProviderCapabilityCatalog.All
            .Select(entry => new ProviderCapabilitiesVm(
                (int)entry.Key,
                entry.Key.ToString(),
                entry.Value.HasFlag(ProviderCapability.RealTimePositions),
                entry.Value.HasFlag(ProviderCapability.PositionHistory),
                entry.Value.HasFlag(ProviderCapability.DeviceCatalog),
                entry.Value.HasFlag(ProviderCapability.ConnectivityPing))));
}
