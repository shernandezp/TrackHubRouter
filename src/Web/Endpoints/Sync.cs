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

using Microsoft.AspNetCore.Mvc;
using TrackHubRouter.Application.DevicePositions.Commands.Sync;

namespace TrackHubRouter.Web.Endpoints;

public class Sync : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(TriggerOperatorSync, "trigger");
    }

    public async Task<IResult> TriggerOperatorSync(ISender sender, [FromBody] TriggerOperatorSyncRequest request, CancellationToken cancellationToken)
    {
        if (request.AccountId == Guid.Empty || request.OperatorId == Guid.Empty)
        {
            return Results.BadRequest(new { error = "accountId and operatorId are required." });
        }

        var correlationId = request.CorrelationId ?? Guid.NewGuid().ToString();
        var accepted = await sender.Send(new TriggerOperatorSyncCommand(
            request.AccountId,
            request.OperatorId,
            request.TriggerType ?? "MANUAL",
            correlationId), cancellationToken);

        if (!accepted)
        {
            return Results.Conflict(new { error = "Sync trigger rejected (feature disabled, operator disabled, or unauthorized).", correlationId });
        }

        return Results.Accepted(value: new { correlationId });
    }
}

public sealed record TriggerOperatorSyncRequest(
    Guid AccountId,
    Guid OperatorId,
    string? TriggerType,
    string? CorrelationId);
