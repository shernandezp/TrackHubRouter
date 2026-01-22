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
using Common.Mediator;
using TrackHubRouter.Application.DevicePositions.Commands.Sync;
using TrackHubRouter.Application.DevicePositions.Events;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.UnitTests.DevicePositions.Commands.Sync;

[TestFixture]
public class SyncPositionCommandHandlerTests : TestsContext
{
    private Mock<IAccountReader> _accountReaderMock = null!;
    private Mock<IOperatorReader> _operatorReaderMock = null!;
    private Mock<IExecutionIntervalManager> _intervalManagerMock = null!;
    private Mock<IPublisher> _publisherMock = null!;

    private UpdateTransporterCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _accountReaderMock = new Mock<IAccountReader>();
        _operatorReaderMock = new Mock<IOperatorReader>();
        _intervalManagerMock = new Mock<IExecutionIntervalManager>();
        _publisherMock = new Mock<IPublisher>();

        _handler = new UpdateTransporterCommandHandler(
            _accountReaderMock.Object,
            _operatorReaderMock.Object,
            _intervalManagerMock.Object,
            _publisherMock.Object);
    }

    [Test]
    public async Task Handle_NoAccounts_ReturnsTrueAndDoesNotPublish()
    {
        // Arrange
        _accountReaderMock.Setup(x => x.GetAccountsToSyncAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<AccountSettingsVm>());

        // Act
        var result = await _handler.Handle(new SyncPositionCommand(), CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        _publisherMock.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        _intervalManagerMock.Verify(x => x.UpdateLastExecutionTime(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task Handle_AccountShouldExecute_PublishesForEachOperatorAndUpdatesInterval()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new AccountSettingsVm(accountId, true, 10, false, false);

        var operatorId1 = Guid.NewGuid();
        var operatorId2 = Guid.NewGuid();

        var operators = new[]
        {
            new OperatorVm(operatorId1, 1, accountId, null),
            new OperatorVm(operatorId2, 1, accountId, null)
        };

        var operatorCredential1 = new OperatorVm(operatorId1, 1, accountId, null);
        var operatorCredential2 = new OperatorVm(operatorId2, 1, accountId, null);

        _accountReaderMock.Setup(x => x.GetAccountsToSyncAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([account]);

        _intervalManagerMock.Setup(x => x.ShouldExecuteTask(account)).Returns(true);

        _operatorReaderMock.Setup(x => x.GetOperatorsByAccountsAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operators);

        _operatorReaderMock.Setup(x => x.GetOperatorAsync(operatorId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operatorCredential1);
        _operatorReaderMock.Setup(x => x.GetOperatorAsync(operatorId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(operatorCredential2);

        // Act
        var result = await _handler.Handle(new SyncPositionCommand(), CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        _publisherMock.Verify(x => x.Publish(It.IsAny<OperatorRetrieved.Notification>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _intervalManagerMock.Verify(x => x.UpdateLastExecutionTime(accountId), Times.Once);
    }

    [Test]
    public async Task Handle_AccountShouldNotExecute_DoesNotPublishOrUpdate()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new AccountSettingsVm(accountId, true, 10, false, false);

        _accountReaderMock.Setup(x => x.GetAccountsToSyncAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([account]);

        _intervalManagerMock.Setup(x => x.ShouldExecuteTask(account)).Returns(false);

        // Act
        var result = await _handler.Handle(new SyncPositionCommand(), CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        _publisherMock.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        _intervalManagerMock.Verify(x => x.UpdateLastExecutionTime(It.IsAny<Guid>()), Times.Never);
    }
}
