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

using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Gating;

/// <summary>
/// Helpers for gating Router operations on the per-account GPS integration feature flags.
/// </summary>
public static class GpsFeatureGate
{
    public const string GpsIntegrationKey = "gps.integration";

    /// <summary>
    /// Returns the account ids from the supplied visible accounts that have provider integration enabled.
    /// </summary>
    public static async Task<HashSet<Guid>> GetProviderIntegrationEnabledAccountIdsAsync(
        IAccountReader accountReader,
        IEnumerable<Guid> accountIds,
        CancellationToken cancellationToken)
    {
        var enabled = new HashSet<Guid>();
        foreach (var accountId in accountIds.Where(id => id != Guid.Empty).Distinct())
        {
            if (await accountReader.IsFeatureEnabledAsync(accountId, GpsIntegrationKey, cancellationToken))
            {
                enabled.Add(accountId);
            }
        }

        return enabled;
    }

    /// <summary>
    /// Returns true when map reads should contact the provider directly because background sync is not enabled.
    /// </summary>
    public static bool CanReadProviderOnDemand(
        OperatorVm @operator,
        IReadOnlySet<Guid> enabledAccountIds)
        => @operator.Enabled && !enabledAccountIds.Contains(@operator.AccountId);
}
