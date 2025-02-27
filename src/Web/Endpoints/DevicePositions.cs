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

using TrackHubRouter.Application.DevicePositions.Queries.Get;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Web.Endpoints;

public class DevicePositions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetDevicePositions);
    }

    public async Task<IEnumerable<PositionVm>> GetDevicePositions(ISender sender, [AsParameters] GetPositionsByUserQuery query)
        => await sender.Send(query);

}
