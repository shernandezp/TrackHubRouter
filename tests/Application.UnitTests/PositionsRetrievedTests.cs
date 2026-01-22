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

using Application.UnitTests;
using Moq;
using TrackHubRouter.Application.DevicePositions.Events;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.UnitTests.DevicePositions.Events;

[TestFixture]
public class PositionsRetrievedTests : TestsContext
{
    [Test]
    public async Task EventHandler_CallsPositionWriterAndGeofenceWhenEnabled()
    {
        // Arrange
        var positionWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IPositionWriter>();
        var geofenceWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Geofence.IGeofenceWriter>();

        var handler = new PositionsRetrieved.Notification.EventHandler(positionWriterMock.Object, geofenceWriterMock.Object);

        var positions = new[] { new PositionVm { DeviceDateTime = DateTime.UtcNow, Latitude = 0, Longitude = 0 } };
        var account = new AccountSettingsVm(Guid.NewGuid(), true, 10, true, true);
        var notification = new PositionsRetrieved.Notification(positions, account);

        geofenceWriterMock.Setup(x => x.ProcessPositionsAsync(It.IsAny<IEnumerable<PositionVm>>(), account.AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TrackHubRouter.Domain.Models.GeofenceProcessingResultVm(0, 1, 0));

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        positionWriterMock.Verify(x => x.AddOrUpdatePositionAsync(positions, It.IsAny<CancellationToken>()), Times.Once);
        geofenceWriterMock.Verify(x => x.ProcessPositionsAsync(It.IsAny<IEnumerable<PositionVm>>(), account.AccountId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task EventHandler_DoesNotThrow_WhenPositionWriterThrows()
    {
        // Arrange
        var positionWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IPositionWriter>();
        var geofenceWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Geofence.IGeofenceWriter>();

        var handler = new PositionsRetrieved.Notification.EventHandler(positionWriterMock.Object, geofenceWriterMock.Object);

        var positions = new[] { new PositionVm { DeviceDateTime = DateTime.UtcNow, Latitude = 0, Longitude = 0 } };
        var account = new AccountSettingsVm(Guid.NewGuid(), true, 10, true, true);
        var notification = new PositionsRetrieved.Notification(positions, account);

        positionWriterMock.Setup(x => x.AddOrUpdatePositionAsync(It.IsAny<IEnumerable<PositionVm>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("test"));

        // Act & Assert: should not throw
        Assert.DoesNotThrowAsync(async () => await handler.Handle(notification, CancellationToken.None));
    }
}
