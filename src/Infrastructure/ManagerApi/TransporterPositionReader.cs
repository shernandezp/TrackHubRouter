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

public class TransporterPositionReader(IGraphQLClientFactory graphQLClient)
    : GraphQLService(graphQLClient.CreateClient(Clients.Manager)), ITransporterPositionReader
{

    public async Task<IEnumerable<PositionVm>> GetTransporterPositionAsync(Guid operatorId, CancellationToken cancellationToken)
    {
        var request = new GraphQLRequest
        {
            Query = @"
                query($operatorId: UUID!) {
                    transporterPositionByOperator(query: { operatorId: $operatorId })
                    {
                        transporterId
                        transporterType
                        state
                        speed
                        longitude
                        latitude
                        geometryId
                        eventId
                        deviceName
                        deviceDateTime
                        course
                        country
                        city
                        attributes {
                            temperature
                            satellites
                            mileage
                            ignition
                            hourmeter
                        }
                        altitude
                        address
                    }
                }",
            Variables = new { operatorId }
        };
        return await QueryAsync<IEnumerable<PositionVm>>(request, cancellationToken);
    }
}
