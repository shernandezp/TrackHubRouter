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

namespace TrackHub.Router.Infrastructure.ManagerApi;

public class AccountReader(IGraphQLClientFactory graphQLClient) 
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IAccountReader
{
    public async Task<AccountSettingsVm> GetAccountSettingsAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($id: UUID!) {
                        accountSettings(query: { id: $id })
                        {
                            storingInterval
                            storeLastPosition
                            accountId
                        }
                    }",
            Variables = new { id = operatorId }
        };
        return await QueryAsync<AccountSettingsVm>(request, cancellationToken);
    }

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
                    filters = new[]
                    {
                        new
                        {
                            key = "StoreLastPosition",
                            value = true
                        }
                    }
                }
            }
        };
        return await QueryAsync<IEnumerable<AccountSettingsVm>>(request, cancellationToken);
    }
}
