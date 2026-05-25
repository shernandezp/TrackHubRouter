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

using Common.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TrackHubRouter.Application.DevicePositions.Commands.Sync;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Interfaces.Operator;
using TrackHubRouter.Domain.Interfaces.Registry;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Records;

namespace Application.UnitTests;

[TestFixture]
public class SyncOperatorDevicesCommandHandlerTests : TestsContext
{
    private Mock<IConfiguration> _configurationMock = null!;
    private Mock<IDeviceRegistry> _deviceRegistryMock = null!;
    private Mock<IDeviceSyncWriter> _deviceSyncWriterMock = null!;
    private Mock<IOperatorSyncRunWriter> _syncRunWriterMock = null!;
    private Mock<IAlertEventWriter> _alertWriterMock = null!;
    private Mock<IExternalDeviceReader> _readerMock = null!;

    [SetUp]
    public void SetUp()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(x => x["AppSettings:EncryptionKey"]).Returns("4F2C2E66-107F-452A-ACDE-402DFD47B84C");

        _deviceRegistryMock = new Mock<IDeviceRegistry>();
        _deviceSyncWriterMock = new Mock<IDeviceSyncWriter>();
        _syncRunWriterMock = new Mock<IOperatorSyncRunWriter>();
        _alertWriterMock = new Mock<IAlertEventWriter>();
        _readerMock = new Mock<IExternalDeviceReader>();
        _readerMock.SetupGet(r => r.Protocol).Returns(ProtocolType.CommandTrack);
        _readerMock.Setup(r => r.Init(It.IsAny<CredentialTokenDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _deviceRegistryMock.Setup(r => r.GetReader(It.IsAny<ProtocolType>())).Returns(_readerMock.Object);
    }

    private SyncOperatorDevicesCommandHandler CreateHandler() => new(
        _configurationMock.Object,
        _deviceRegistryMock.Object,
        _deviceSyncWriterMock.Object,
        _syncRunWriterMock.Object,
        _alertWriterMock.Object,
        Mock.Of<ILogger<SyncOperatorDevicesCommandHandler>>());

    private static OperatorVm OperatorWith(CredentialTokenVm? credential, Guid? accountId = null) =>
        new(Guid.NewGuid(), (int)ProtocolType.CommandTrack, accountId ?? Guid.NewGuid(), credential);

    private static AccountSettingsVm EnabledAccount(Guid accountId) =>
        new(accountId, false, 0, false, false, GpsIntegrationEnabled: true);

    [Test]
    public async Task Handle_NoCredential_ReturnsFalseAndRecordsNothing()
    {
        var op = OperatorWith(null);
        var account = EnabledAccount(op.AccountId);

        var result = await CreateHandler().Handle(
            new SyncOperatorDevicesCommand(op, account, "MANUAL"), CancellationToken.None);

        Assert.That(result, Is.False);
        _syncRunWriterMock.Verify(w => w.RecordAsync(It.IsAny<OperatorSyncRunDto>(), It.IsAny<CancellationToken>()), Times.Never);
        _alertWriterMock.Verify(w => w.RecordAsync(It.IsAny<AlertEventDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_Success_RecordsSucceededRunAndPushesDevices()
    {
        var op = OperatorWith(TestCredentialTokenVm);
        var account = EnabledAccount(op.AccountId);
        var devices = new[]
        {
            new DeviceVm(Guid.NewGuid(), 1, "S1", "Device 1", 0, 0, "Dev1", "hash", "ACTIVE"),
            new DeviceVm(Guid.NewGuid(), 2, "S2", "Device 2", 0, 0, "Dev2", "hash2", "ACTIVE")
        };
        _readerMock.Setup(r => r.GetDevicesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(devices);

        var result = await CreateHandler().Handle(
            new SyncOperatorDevicesCommand(op, account, "AUTOMATIC", "corr-1"), CancellationToken.None);

        Assert.That(result, Is.True);
        _deviceSyncWriterMock.Verify(w => w.SynchronizeAsync(
            account.AccountId,
            op.OperatorId,
            It.Is<IEnumerable<SynchronizedDeviceDto>>(d => d.Count() == 2),
            "corr-1",
            It.IsAny<CancellationToken>()), Times.Once);
        _syncRunWriterMock.Verify(w => w.RecordAsync(
            It.Is<OperatorSyncRunDto>(r => r.Result == "SUCCEEDED"
                                            && r.DevicesSeen == 2
                                            && r.CorrelationId == "corr-1"
                                            && r.TriggerType == "AUTOMATIC"),
            It.IsAny<CancellationToken>()), Times.Once);
        _alertWriterMock.Verify(w => w.RecordAsync(It.IsAny<AlertEventDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_AdapterFailure_RecordsFailedRunAndRaisesAlert()
    {
        var op = OperatorWith(TestCredentialTokenVm);
        var account = EnabledAccount(op.AccountId);
        _readerMock.Setup(r => r.GetDevicesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var result = await CreateHandler().Handle(
            new SyncOperatorDevicesCommand(op, account, "MANUAL"), CancellationToken.None);

        Assert.That(result, Is.False);
        _deviceSyncWriterMock.Verify(w => w.SynchronizeAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<SynchronizedDeviceDto>>(),
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _syncRunWriterMock.Verify(w => w.RecordAsync(
            It.Is<OperatorSyncRunDto>(r => r.Result == "FAILED"
                                            && r.ErrorCode == "InvalidOperationException"
                                            && r.ErrorMessage == "boom"),
            It.IsAny<CancellationToken>()), Times.Once);
        _alertWriterMock.Verify(w => w.RecordAsync(
            It.Is<AlertEventDto>(a => a.EventType == "GpsOperatorDeviceSyncFailed"
                                       && a.Severity == "Warning"
                                       && a.DeduplicationKey.StartsWith($"device-sync-failed:{op.OperatorId}")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_NoDevicesReturned_RecordsSucceededRunAndSkipsUpsert()
    {
        var op = OperatorWith(TestCredentialTokenVm);
        var account = EnabledAccount(op.AccountId);
        _readerMock.Setup(r => r.GetDevicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await CreateHandler().Handle(
            new SyncOperatorDevicesCommand(op, account, "AUTOMATIC"), CancellationToken.None);

        Assert.That(result, Is.True);
        _deviceSyncWriterMock.Verify(w => w.SynchronizeAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<SynchronizedDeviceDto>>(),
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _syncRunWriterMock.Verify(w => w.RecordAsync(
            It.Is<OperatorSyncRunDto>(r => r.Result == "SUCCEEDED" && r.DevicesSeen == 0),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
