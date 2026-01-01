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

using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.DevicePositions.Events;

public sealed class ValidateSync
{
    public readonly record struct Notification(Guid AccountId, IEnumerable<PositionVm> Positions) : INotification
    {
        public class EventHandler(IAccountReader accountReader, IPublisher publisher) : INotificationHandler<Notification>
        {
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                var settings = await accountReader.GetAccountSettingsAsync(notification.AccountId, cancellationToken);
                if (!settings.StoreLastPosition)
                {
                    await publisher.Publish(new PositionsRetrieved.Notification(notification.Positions), cancellationToken);
                }
            }
        }
    }
}
