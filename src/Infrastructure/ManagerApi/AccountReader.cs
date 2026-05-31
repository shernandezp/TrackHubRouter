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

namespace TrackHub.Router.Infrastructure.ManagerApi;

public class AccountReader(IGraphQLClientFactory graphQLClient) 
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IAccountReader
{

    public async Task<IEnumerable<AccountSettingsVm>> GetAccountsToSyncAsync(CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($filter: FiltersInput!) {
                    accountSettingsMaster(
                        query: { filter: $filter }
                      ) {
                            accountId
                            storeLastPosition
                            storingInterval
                       }
                }",
            Variables = new
            {
                filter = new
                {
                    filters = Array.Empty<object>()
                }
            }
        };
        var accounts = await QueryAsync<IEnumerable<AccountSettingsVm>>(request, cancellationToken);
        var accountTasks = accounts.Select(account => AddAccountFeaturesAsync(account, cancellationToken));
        return await Task.WhenAll(accountTasks);
    }

    public async Task<AccountSettingsVm?> GetAccountToSyncAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($id: UUID!) {
                    accountSettings(query: { id: $id }) {
                        accountId
                        storeLastPosition
                        storingInterval
                   }
                }",
            Variables = new
            {
                id = accountId
            }
        };

        var account = await QueryAsync<AccountSettingsVm>(request, cancellationToken);
        return account.AccountId == Guid.Empty
            ? null
            : await AddAccountFeaturesAsync(account, cancellationToken);
    }

    public async Task<bool> IsFeatureEnabledAsync(Guid accountId, string featureKey, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($accountId: UUID!, $featureKey: String!) {
                    validateFeatureEnabled(query: { accountId: $accountId, featureKey: $featureKey })
                }",
            Variables = new
            {
                accountId,
                featureKey
            }
        };

        return await QueryAsync<bool>(request, cancellationToken);
    }

    private async Task<AccountSettingsVm> AddAccountFeaturesAsync(AccountSettingsVm account, CancellationToken cancellationToken)
    {
        var features = await GetAccountFeaturesAsync(account.AccountId, cancellationToken);
        return new AccountSettingsVm(
            account.AccountId,
            account.StoreLastPosition,
            account.StoringInterval,
            IsFeatureEnabled(features, FeatureKeys.Geofencing),
            IsFeatureEnabled(features, FeatureKeys.TripManagement),
            IsFeatureEnabled(features, FeatureKeys.GpsIntegration),
            IsFeatureEnabled(features, FeatureKeys.GpsOperatorHealth),
            IsFeatureEnabled(features, FeatureKeys.GpsPositionHistory));
    }

    private async Task<IReadOnlyCollection<AccountFeatureStateVm>> GetAccountFeaturesAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($accountId: UUID!) {
                    accountFeatures(query: { accountId: $accountId }) {
                        featureKey
                        enabled
                        effectiveFrom
                        effectiveTo
                    }
                }",
            Variables = new
            {
                accountId
            }
        };

        return await QueryAsync<IReadOnlyCollection<AccountFeatureStateVm>>(request, cancellationToken);
    }

    private static bool IsFeatureEnabled(IEnumerable<AccountFeatureStateVm> features, string featureKey)
    {
        var now = DateTimeOffset.UtcNow;
        return features.Any(feature =>
            feature.FeatureKey == featureKey
            && feature.Enabled
            && (!feature.EffectiveFrom.HasValue || feature.EffectiveFrom <= now)
            && (!feature.EffectiveTo.HasValue || feature.EffectiveTo >= now));
    }

    private readonly record struct AccountFeatureStateVm(
        string FeatureKey,
        bool Enabled,
        DateTimeOffset? EffectiveFrom,
        DateTimeOffset? EffectiveTo);
}
