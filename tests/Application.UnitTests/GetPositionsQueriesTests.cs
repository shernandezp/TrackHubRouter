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
using Microsoft.Extensions.Configuration;
using TrackHubRouter.Application.DevicePositions.Queries.Get;
using TrackHubRouter.Domain.Interfaces.Registry;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Models;
using Common.Domain.Enums;
using Common.Mediator;
using TrackHubRouter.Domain.Records;
using TrackHubRouter.Domain.Interfaces.Operator;
using TrackHubRouter.Application.DevicePositions.Events;

namespace TrackHubRouter.Application.UnitTests.DevicePositions.Queries.Get;

[TestFixture]
public class GetPositionsQueriesTests : TestsContext
{
    private Mock<IConfiguration> _configurationMock = null!;
    private Mock<IPositionRegistry> _positionRegistryMock = null!;
    private Mock<IDeviceTransporterReader> _deviceReaderMock = null!;
    private Mock<IOperatorReader> _operatorReaderMock = null!;
    private Mock<ITransporterPositionReader> _transporterPositionReaderMock = null!;

    [SetUp]
    public void SetUp()
    {
        _configurationMock = new Mock<IConfiguration>();
        _positionRegistryMock = new Mock<IPositionRegistry>();
        _deviceReaderMock = new Mock<IDeviceTransporterReader>();
        _operatorReaderMock = new Mock<IOperatorReader>();
        _transporterPositionReaderMock = new Mock<ITransporterPositionReader>();

        _configurationMock.Setup(x => x["AppSettings:EncryptionKey"]).Returns("4F2C2E66-107F-452A-ACDE-402DFD47B84C");
    }

    [Test]
    public async Task GetPositionsByOperator_WithCredential_PublishesPositions()
    {
        // Arrange
        var publisherMock = new Mock<IPublisher>();
        var readerMock = new Mock<IPositionReader>();

        var operatorId = Guid.NewGuid();
        var operatorVm = new OperatorVm(operatorId, (int)ProtocolType.CommandTrack, Guid.NewGuid(), TestCredentialTokenVm);
        var account = new AccountSettingsVm(Guid.NewGuid(), true, 10, false, false);

        readerMock.Setup(x => x.Init(It.IsAny<CredentialTokenDto>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        readerMock.SetupGet(x => x.Protocol).Returns(ProtocolType.CommandTrack);
        readerMock.Setup(x => x.GetDevicePositionAsync(It.IsAny<IEnumerable<DeviceTransporterVm>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new PositionVm { TransporterId = Guid.NewGuid(), DeviceDateTime = DateTime.UtcNow }]);

        _positionRegistryMock.Setup(x => x.GetReader(It.IsAny<ProtocolType>())).Returns(readerMock.Object);
        _deviceReaderMock.Setup(x => x.GetDeviceTransporterAsync(operatorId, It.IsAny<CancellationToken>())).ReturnsAsync([new DeviceTransporterVm { TransporterId = Guid.NewGuid() }]);

        var handler = new GetPositionsByOperatorQueryHandler(
            publisherMock.Object,
            _configurationMock.Object,
            _positionRegistryMock.Object,
            _deviceReaderMock.Object);

        // Act
        var result = await handler.Handle(new GetPositionsByOperatorQuery(operatorVm, account), CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        publisherMock.Verify(x => x.Publish(It.IsAny<PositionsRetrieved.Notification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetPositionsByOperator_WithNullCredential_DoesNotPublish()
    {
        // Arrange
        var publisherMock = new Mock<IPublisher>();

        var operatorVm = new OperatorVm(Guid.NewGuid(), (int)ProtocolType.CommandTrack, Guid.NewGuid(), null);
        var account = new AccountSettingsVm(Guid.NewGuid(), true, 10, false, false);

        var handler = new GetPositionsByOperatorQueryHandler(
            publisherMock.Object,
            _configurationMock.Object,
            _positionRegistryMock.Object,
            _deviceReaderMock.Object);

        // Act
        var result = await handler.Handle(new GetPositionsByOperatorQuery(operatorVm, account), CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        publisherMock.Verify(x => x.Publish(It.IsAny<PositionsRetrieved.Notification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetPositionByTransporter_ReturnsPosition_WhenCredentialPresent()
    {
        // Arrange
        var readerMock = new Mock<IPositionReader>();
        var operatorId = Guid.NewGuid();
        var operatorVm = new OperatorVm(Guid.NewGuid(), (int)ProtocolType.CommandTrack, Guid.NewGuid(), TestCredentialTokenVm);
        var device = new DeviceTransporterVm { TransporterId = operatorId };

        readerMock.Setup(x => x.Init(It.IsAny<CredentialTokenDto>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        readerMock.SetupGet(x => x.Protocol).Returns(ProtocolType.CommandTrack);
        readerMock.Setup(x => x.GetDevicePositionAsync(device, It.IsAny<CancellationToken>())).ReturnsAsync(new PositionVm { TransporterId = device.TransporterId, DeviceDateTime = DateTime.UtcNow });

        _positionRegistryMock.Setup(x => x.GetReader(It.IsAny<ProtocolType>())).Returns(readerMock.Object);
        _operatorReaderMock.Setup(x => x.GetOperatorByTransporterAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(operatorVm);
        _deviceReaderMock.Setup(x => x.GetDevicesTransporterAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(device);

        var handler = new GetPositionByTransporterQueryHandler(
            _configurationMock.Object,
            _operatorReaderMock.Object,
            _positionRegistryMock.Object,
            _deviceReaderMock.Object);

        // Act
        var result = await handler.Handle(new GetPositionByTransporterQuery(device.TransporterId), CancellationToken.None);

        // Assert
        Assert.That(result.TransporterId, Is.EqualTo(device.TransporterId));
    }

    [Test]
    public async Task GetPositionsByUser_ReturnsMostRecentPositionPerTransporter()
    {
        // Arrange
        var readerMock = new Mock<IPositionReader>();
        var operatorId = Guid.NewGuid();
        var operatorVm = new OperatorVm(Guid.NewGuid(), (int)ProtocolType.CommandTrack, Guid.NewGuid(), TestCredentialTokenVm);
        var device = new DeviceTransporterVm { TransporterId = operatorId };

        readerMock.Setup(x => x.Init(It.IsAny<CredentialTokenDto>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        readerMock.SetupGet(x => x.Protocol).Returns(ProtocolType.CommandTrack);
        readerMock.Setup(x => x.GetDevicePositionAsync(It.IsAny<IEnumerable<DeviceTransporterVm>>(), It.IsAny<CancellationToken>())).ReturnsAsync([new PositionVm { TransporterId = device.TransporterId, DeviceDateTime = DateTime.UtcNow }]);

        _positionRegistryMock.Setup(x => x.GetReaders(It.IsAny<IEnumerable<ProtocolType>>())).Returns([readerMock.Object]);
        _operatorReaderMock.Setup(x => x.GetOperatorsAsync(It.IsAny<CancellationToken>())).ReturnsAsync([operatorVm]);
        _deviceReaderMock.Setup(x => x.GetDevicesByOperatorAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync([device]);

        var handler = new GetPositionsByUserQueryHandler(
            _configurationMock.Object,
            _operatorReaderMock.Object,
            _positionRegistryMock.Object,
            _deviceReaderMock.Object,
            _transporterPositionReaderMock.Object);

        // Act
        var result = await handler.Handle(new GetPositionsByUserQuery(), CancellationToken.None);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }
}
