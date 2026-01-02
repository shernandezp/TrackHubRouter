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

// This class represents a device reader that implements the IDeviceReader interface.
// It is responsible for retrieving devices by operator from the GraphQL service.
public class DeviceTransporterReader(IGraphQLClientFactory graphQLClient) 
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), IDeviceTransporterReader
{

    /// <summary>
    /// Retrieves devices by operator asynchronously.
    /// </summary>
    /// <param name="operatorId">The ID of the operator.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of DeviceVm objects representing the devices.</returns>
    public async Task<IEnumerable<DeviceTransporterVm>> GetDevicesByOperatorAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($operatorId: UUID!) {
                    deviceTransporterByUserByOperator(query: { operatorId: $operatorId })
                    {
                        transporterId,
                        identifier,
                        serial,
                        name,
                        transporterType,
                        transporterTypeId
                    }
                }",
            Variables = new { operatorId }
        };
        return await QueryAsync<IEnumerable<DeviceTransporterVm>>(request, cancellationToken);
    }

    public async Task<IEnumerable<DeviceTransporterVm>> GetDeviceTransporterAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
            query($filter: FiltersInput!) {
                deviceTransporterMaster(
                    query: { filter: $filter }
                    ) {
                        transporterId,
                        identifier,
                        serial,
                        name,
                        transporterType,
                        transporterTypeId
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
                            key = "OperatorId",
                            value = operatorId
                        }
                    }
                }
            }
        };
        return await QueryAsync<IEnumerable<DeviceTransporterVm>>(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves device transporter by id asynchronously.
    /// </summary>
    /// <param name="transporterId">The ID of the transporter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A DeviceVm object representing the device.</returns>
    public async Task<DeviceTransporterVm> GetDevicesTransporterAsync(Guid transporterId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($transporterId: UUID!) {
                    deviceTransporterById(query: { transporterId: $transporterId })
                    {
                        transporterId,
                        identifier,
                        serial,
                        name,
                        transporterType,
                        transporterTypeId
                    }
                }",
            Variables = new { transporterId }
        };
        return await QueryAsync<DeviceTransporterVm>(request, cancellationToken);
    }
}
