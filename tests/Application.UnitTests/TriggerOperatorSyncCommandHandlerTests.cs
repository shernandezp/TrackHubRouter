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
using Common.Mediator;
using Microsoft.Extensions.Logging;
using Moq;
using TrackHubRouter.Application.DevicePositions.Commands.Sync;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Models;

namespace Application.UnitTests;

[TestFixture]
public class TriggerOperatorSyncCommandHandlerTests : TestsContext
{
    private Mock<IAccountReader> _accountReaderMock = null!;
    private Mock<IOperatorReader> _operatorReaderMock = null!;
    private Mock<ISender> _senderMock = null!;

    [SetUp]
    public void SetUp()
    {
        _accountReaderMock = new Mock<IAccountReader>();
        _operatorReaderMock = new Mock<IOperatorReader>();
        _senderMock = new Mock<ISender>();
    }

    private TriggerOperatorSyncCommandHandler CreateHandler() => new(
        _accountReaderMock.Object,
        _operatorReaderMock.Object,
        _senderMock.Object,
        Mock.Of<ILogger<TriggerOperatorSyncCommandHandler>>());

    private void SetupAccounts(params AccountSettingsVm[] accounts)
    {
        _accountReaderMock.Setup(x => x.GetAccountsToSyncAsync(It.IsAny<CancellationToken>())).ReturnsAsync(accounts);
        _accountReaderMock.Setup(x => x.GetAccountToSyncAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccountSettingsVm?)null);

        foreach (var account in accounts)
        {
            _accountReaderMock.Setup(x => x.GetAccountToSyncAsync(account.AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);
        }
    }

    private void SetupOperator(OperatorVm op) =>
        _operatorReaderMock.Setup(x => x.GetOperatorAsync(op.OperatorId, It.IsAny<CancellationToken>())).ReturnsAsync(op);

    [Test]
    public async Task Handle_UnknownAccount_ReturnsFalseAndDoesNotDispatch()
    {
        var accountId = Guid.NewGuid();
        var op = new OperatorVm(Guid.NewGuid(), (int)ProtocolType.CommandTrack, accountId, TestCredentialTokenVm);
        SetupAccounts(); // empty list
        SetupOperator(op);

        var result = await CreateHandler().Handle(
            new TriggerOperatorSyncCommand(accountId, op.OperatorId), CancellationToken.None);

        Assert.That(result, Is.False);
        _senderMock.Verify(s => s.Send(It.IsAny<SyncOperatorDevicesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_GpsIntegrationDisabled_ReturnsFalse()
    {
        var accountId = Guid.NewGuid();
        var op = new OperatorVm(Guid.NewGuid(), (int)ProtocolType.CommandTrack, accountId, TestCredentialTokenVm);
        SetupAccounts(new AccountSettingsVm(accountId, false, 0, false, false, GpsIntegrationEnabled: false));
        SetupOperator(op);

        var result = await CreateHandler().Handle(
            new TriggerOperatorSyncCommand(accountId, op.OperatorId), CancellationToken.None);

        Assert.That(result, Is.False);
        _senderMock.Verify(s => s.Send(It.IsAny<SyncOperatorDevicesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_CrossAccountOperator_ReturnsFalse()
    {
        var accountId = Guid.NewGuid();
        var otherAccount = Guid.NewGuid();
        SetupAccounts(new AccountSettingsVm(accountId, false, 0, false, false, GpsIntegrationEnabled: true));
        var op = new OperatorVm(Guid.NewGuid(), (int)ProtocolType.CommandTrack, otherAccount, TestCredentialTokenVm);
        SetupOperator(op);

        var result = await CreateHandler().Handle(
            new TriggerOperatorSyncCommand(accountId, op.OperatorId), CancellationToken.None);

        Assert.That(result, Is.False);
        _senderMock.Verify(s => s.Send(It.IsAny<SyncOperatorDevicesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_OperatorDisabled_ReturnsFalse()
    {
        var accountId = Guid.NewGuid();
        SetupAccounts(new AccountSettingsVm(accountId, false, 0, false, false, GpsIntegrationEnabled: true));
        var op = new OperatorVm(Guid.NewGuid(), (int)ProtocolType.CommandTrack, accountId, TestCredentialTokenVm,
            Enabled: false);
        SetupOperator(op);

        var result = await CreateHandler().Handle(
            new TriggerOperatorSyncCommand(accountId, op.OperatorId), CancellationToken.None);

        Assert.That(result, Is.False);
        _senderMock.Verify(s => s.Send(It.IsAny<SyncOperatorDevicesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_HappyPath_DispatchesSyncOperatorDevicesCommand()
    {
        var accountId = Guid.NewGuid();
        var account = new AccountSettingsVm(accountId, false, 0, false, false, GpsIntegrationEnabled: true);
        SetupAccounts(account);
        var op = new OperatorVm(Guid.NewGuid(), (int)ProtocolType.CommandTrack, accountId, TestCredentialTokenVm);
        SetupOperator(op);
        _senderMock.Setup(s => s.Send(It.IsAny<SyncOperatorDevicesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await CreateHandler().Handle(
            new TriggerOperatorSyncCommand(accountId, op.OperatorId, "MANUAL", "corr-42"), CancellationToken.None);

        Assert.That(result, Is.True);
        _accountReaderMock.Verify(x => x.GetAccountsToSyncAsync(It.IsAny<CancellationToken>()), Times.Never);
        _senderMock.Verify(s => s.Send(
            It.Is<SyncOperatorDevicesCommand>(c =>
                c.Operator.OperatorId == op.OperatorId
                && c.Account.AccountId == accountId
                && c.TriggerType == "MANUAL"
                && c.CorrelationId == "corr-42"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_DownstreamSyncReturnsFalse_ReturnsFalse()
    {
        var accountId = Guid.NewGuid();
        var account = new AccountSettingsVm(accountId, false, 0, false, false, GpsIntegrationEnabled: true);
        SetupAccounts(account);
        var op = new OperatorVm(Guid.NewGuid(), (int)ProtocolType.CommandTrack, accountId, TestCredentialTokenVm);
        SetupOperator(op);
        _senderMock.Setup(s => s.Send(It.IsAny<SyncOperatorDevicesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await CreateHandler().Handle(
            new TriggerOperatorSyncCommand(accountId, op.OperatorId, "MANUAL", "corr-42"), CancellationToken.None);

        Assert.That(result, Is.False);
    }
}
