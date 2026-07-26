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

using TrackHub.Router.Domain.Enumerators;
using TrackHub.Router.Domain.Exceptions;

namespace TrackHub.Router.Domain.Constants;

/// <summary>
/// The single place that declares what each GPS provider's API supports. A capability
/// listed here means the provider implements it against its external API; a missing
/// capability is a PROVIDER limitation, surfaced to clients as
/// <see cref="ProviderCapabilityNotSupportedException"/> (GraphQL code
/// PROVIDER_CAPABILITY_NOT_SUPPORTED) — never confused with TrackHub feature gating
/// (FEATURE_DISABLED) or a missing provider assembly (ProtocolNotSupportedException).
/// </summary>
/// <remarks>
/// Kept in the Domain so both the Application handlers (request-time enforcement) and the
/// registration path (startup cross-check against the reader classes actually present in the
/// provider assembly — see <c>ProtocolRegistrationExtensions</c>) read the same declaration.
/// When adding a provider, declaring its entry here is the fifth alignment point of
/// <c>docs/adding-a-provider.md</c>.
/// </remarks>
public static class ProviderCapabilityCatalog
{
    private const ProviderCapability Full =
        ProviderCapability.RealTimePositions
        | ProviderCapability.PositionHistory
        | ProviderCapability.DeviceCatalog
        | ProviderCapability.ConnectivityPing;

    private static readonly IReadOnlyDictionary<ProtocolType, ProviderCapability> Catalog =
        new Dictionary<ProtocolType, ProviderCapability>
        {
            [ProtocolType.CommandTrack] = Full,
            [ProtocolType.Traccar] = Full,
            [ProtocolType.Flespi] = Full,
            [ProtocolType.GeoTab] = Full,
            // GpsGate's REST surface exposes no usable track-history endpoint; history stays
            // unavailable until GpsGate ships one (PositionReader.GetPositionAsync is a
            // capability-guarded stub).
            [ProtocolType.GpsGate] = Full & ~ProviderCapability.PositionHistory,
            [ProtocolType.Navixy] = Full,
            [ProtocolType.Samsara] = Full,
            [ProtocolType.Wialon] = Full,
            [ProtocolType.Protrack] = Full,
            // Reserved in the enum with no provider assembly (docs/adding-a-provider.md §2).
            [ProtocolType.Mettax] = ProviderCapability.None,
        };

    public static ProviderCapability Get(ProtocolType protocol)
        => Catalog.TryGetValue(protocol, out var capabilities) ? capabilities : ProviderCapability.None;

    public static bool Supports(ProtocolType protocol, ProviderCapability capability)
        => (Get(protocol) & capability) == capability;

    /// <summary>Throws the client-facing provider-limitation error when the capability is missing.</summary>
    public static void EnsureSupports(ProtocolType protocol, ProviderCapability capability)
    {
        if (!Supports(protocol, capability))
        {
            throw new ProviderCapabilityNotSupportedException(protocol, capability);
        }
    }

    /// <summary>Every declared protocol with its capabilities, for the capability-matrix query.</summary>
    public static IEnumerable<KeyValuePair<ProtocolType, ProviderCapability>> All => Catalog;
}
