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
using Microsoft.Extensions.Logging;
using Moq;
using TrackHubRouter.Application.DevicePositions.Events;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.UnitTests.DevicePositions.Events;

[TestFixture]
public class PositionsRetrievedTests : TestsContext
{
    private static PositionsRetrieved.Notification.EventHandler CreateHandler(
        Mock<TrackHubRouter.Domain.Interfaces.Manager.IPositionWriter> positionWriterMock,
        Mock<TrackHubRouter.Domain.Interfaces.Geofence.IGeofenceWriter> geofenceWriterMock,
        Mock<TrackHubRouter.Domain.Interfaces.Manager.IOperatorSyncRunWriter> syncRunMock,
        Mock<TrackHubRouter.Domain.Interfaces.Manager.IAlertEventWriter> alertMock)
        => new(positionWriterMock.Object, geofenceWriterMock.Object, syncRunMock.Object, alertMock.Object,
            Mock.Of<ILogger<PositionsRetrieved.Notification.EventHandler>>());

    private static PositionsRetrieved.Notification BuildNotification(IEnumerable<PositionVm> positions, AccountSettingsVm account)
    {
        var op = new OperatorVm(Guid.NewGuid(), 1, account.AccountId, null);
        return new PositionsRetrieved.Notification(positions, account, op, DateTimeOffset.UtcNow, "AUTOMATIC", Guid.NewGuid().ToString());
    }

    [Test]
    public async Task EventHandler_CallsPositionWriterAndGeofenceWhenEnabled_AndRecordsSucceededRun()
    {
        var positionWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IPositionWriter>();
        var geofenceWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Geofence.IGeofenceWriter>();
        var syncRunMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IOperatorSyncRunWriter>();
        var alertMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IAlertEventWriter>();

        var handler = CreateHandler(positionWriterMock, geofenceWriterMock, syncRunMock, alertMock);

        var positions = new[] { new PositionVm { TransporterId = Guid.NewGuid(), DeviceDateTime = DateTime.UtcNow, Latitude = 0, Longitude = 0 } };
        var account = new AccountSettingsVm(Guid.NewGuid(), 10, true, true);
        var notification = BuildNotification(positions, account);

        positionWriterMock.Setup(x => x.AddOrUpdatePositionAsync(It.IsAny<IEnumerable<PositionVm>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        geofenceWriterMock.Setup(x => x.ProcessPositionsAsync(It.IsAny<IEnumerable<PositionVm>>(), account.AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TrackHubRouter.Domain.Models.GeofenceProcessingResultVm(0, 1, 0));

        await handler.Handle(notification, CancellationToken.None);

        positionWriterMock.Verify(x => x.AddOrUpdatePositionAsync(
            It.Is<IEnumerable<PositionVm>>(p => p.Single().TransporterId == positions[0].TransporterId),
            It.IsAny<CancellationToken>()), Times.Once);
        geofenceWriterMock.Verify(x => x.ProcessPositionsAsync(It.IsAny<IEnumerable<PositionVm>>(), account.AccountId, It.IsAny<CancellationToken>()), Times.Once);
        syncRunMock.Verify(x => x.RecordAsync(It.Is<OperatorSyncRunDto>(d => d.Result == "SUCCEEDED" && d.PositionsAccepted == 1), It.IsAny<CancellationToken>()), Times.Once);
        alertMock.Verify(x => x.RecordAsync(It.IsAny<AlertEventDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task EventHandler_RecordsFailedRunAndAlert_WhenPositionWriterThrows()
    {
        var positionWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IPositionWriter>();
        var geofenceWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Geofence.IGeofenceWriter>();
        var syncRunMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IOperatorSyncRunWriter>();
        var alertMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IAlertEventWriter>();

        var handler = CreateHandler(positionWriterMock, geofenceWriterMock, syncRunMock, alertMock);

        var positions = new[] { new PositionVm { TransporterId = Guid.NewGuid(), DeviceDateTime = DateTime.UtcNow, Latitude = 0, Longitude = 0 } };
        var account = new AccountSettingsVm(Guid.NewGuid(), 10, true, true);
        var notification = BuildNotification(positions, account);

        positionWriterMock.Setup(x => x.AddOrUpdatePositionAsync(It.IsAny<IEnumerable<PositionVm>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        Assert.DoesNotThrowAsync(async () => await handler.Handle(notification, CancellationToken.None));

        syncRunMock.Verify(x => x.RecordAsync(It.Is<OperatorSyncRunDto>(d => d.Result == "FAILED" && d.ErrorCode == "InvalidOperationException"), It.IsAny<CancellationToken>()), Times.Once);
        alertMock.Verify(x => x.RecordAsync(It.Is<AlertEventDto>(a => a.EventType == "GpsOperatorPositionSyncFailed"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task EventHandler_RecordsRequestedTriggerType()
    {
        var positionWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IPositionWriter>();
        var geofenceWriterMock = new Mock<TrackHubRouter.Domain.Interfaces.Geofence.IGeofenceWriter>();
        var syncRunMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IOperatorSyncRunWriter>();
        var alertMock = new Mock<TrackHubRouter.Domain.Interfaces.Manager.IAlertEventWriter>();

        var handler = CreateHandler(positionWriterMock, geofenceWriterMock, syncRunMock, alertMock);
        var account = new AccountSettingsVm(Guid.NewGuid(), 10, false, false);
        var op = new OperatorVm(Guid.NewGuid(), 1, account.AccountId, null);
        var notification = new PositionsRetrieved.Notification(
            [],
            account,
            op,
            DateTimeOffset.UtcNow,
            "MANUAL",
            "corr-42");

        await handler.Handle(notification, CancellationToken.None);

        syncRunMock.Verify(x => x.RecordAsync(
            It.Is<OperatorSyncRunDto>(d => d.TriggerType == "MANUAL" && d.CorrelationId == "corr-42"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
