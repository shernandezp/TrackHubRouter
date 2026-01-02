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

namespace ManagerApi;

// This class represents the implementation of the IOperatorReader interface
// It is responsible for reading operator data from the GraphQL service
public class OperatorReader(IGraphQLClientFactory graphQLClient) 
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IOperatorReader
{

    /// <summary>
    /// Retrieves a list of operators associated with the current user 
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed</param>
    /// <returns>A collection of OperatorVm objects representing the operators</returns>
    public async Task<IEnumerable<OperatorVm>> GetOperatorsAsync(CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query {
                        operatorsByUser
                        {
                            operatorId
                            protocolTypeId
                            accountId
                            credential {
                                credentialId
                                uri
                                username
                                password
                                salt
                                key
                                key2
                                token
                                tokenExpiration
                                refreshToken
                                refreshTokenExpiration
                            }
                        }
                    }"
        };
        return await QueryAsync<IEnumerable<OperatorVm>>(request, cancellationToken);
    }

    public async Task<OperatorVm> GetOperatorByTransporterAsync(Guid transporterId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($transporterId: UUID!) {
                        operatorByTransporter(query: { transporterId: $transporterId })
                        {
                            operatorId
                            protocolTypeId
                            accountId
                            credential {
                                credentialId
                                uri
                                username
                                password
                                salt
                                key
                                key2
                                token
                                tokenExpiration
                                refreshToken
                                refreshTokenExpiration
                            }
                        }
                    }",
            Variables = new { transporterId }
        };
        return await QueryAsync<OperatorVm>(request, cancellationToken);
    }

    public async Task<OperatorVm> GetOperatorAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                    query($id: UUID!) {
                        operator(query: { id: $id })
                        {
                            operatorId
                            protocolTypeId
                            accountId
                            credential {
                                credentialId
                                uri
                                username
                                password
                                salt
                                key
                                key2
                                token
                                tokenExpiration
                                refreshToken
                                refreshTokenExpiration
                            }
                        }
                    }",
            Variables = new { id = operatorId }
        };
        return await QueryAsync<OperatorVm>(request, cancellationToken);
    }

    public async Task<IEnumerable<OperatorVm>> GetOperatorsByAccountsAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
            query($filter: FiltersInput!) {
                operatorsMaster(
                    query: { filter: $filter }
                    ) {
                        operatorId
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
                        key = "AccountId",
                        value = accountId
                    }
                }
                }
            }
        };
        return await QueryAsync<IEnumerable<OperatorVm>>(request, cancellationToken);
    }
}
