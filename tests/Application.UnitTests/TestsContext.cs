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

using Common.Domain.Constants;
using Moq;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Models;

namespace Application.UnitTests;

public abstract class TestsContext
{
    protected CredentialTokenVm TestCredentialTokenVm { get; } = new()
    {
        Salt = "yhhidBsASfV6Hh1VLYHD+suDa7cjDgLtgOfKVcOl1i8=",
        Username = "TJeYWbqXtz7bn+j3VolgkVY2LXze37E80K3wC2zNWpSHG6IEuaYEh6UD+2DAdie7XKP3kk3i5pvQc/hDxNwfZQ==",
        Password = "TJeYWbqXtz7bn+j3VolgkVY2LXze37E80K3wC2zNWpSHG6IEuaYEh6UD+2DAdie7XKP3kk3i5pvQc/hDxNwfZQ==",
        Key = "TJeYWbqXtz7bn+j3VolgkVY2LXze37E80K3wC2zNWpSHG6IEuaYEh6UD+2DAdie7XKP3kk3i5pvQc/hDxNwfZQ==",
        Uri = "https://www.example.com/"
    };

    /// <summary>
    /// Creates an <see cref="IAccountReader"/> mock that returns a single GPS-integration-enabled account
    /// for the given accountId. Use when the handler under test expects the gating helper to succeed.
    /// </summary>
    protected static Mock<IAccountReader> AccountReaderForEnabled(params Guid[] accountIds)
    {
        var mock = new Mock<IAccountReader>();
        var accounts = accountIds.Select(id => new AccountSettingsVm(
            id,
            StoreLastPosition: false,
            StoringInterval: 0,
            GeofencingEnabled: false,
            TripManagementEnabled: false,
            GpsIntegrationEnabled: true,
            GpsOperatorHealthEnabled: true,
            GpsPositionHistoryEnabled: false)).ToList();
        mock.Setup(x => x.GetAccountsToSyncAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);
        mock.Setup(x => x.IsFeatureEnabledAsync(
                It.Is<Guid>(id => accountIds.Contains(id)),
                FeatureKeys.GpsIntegration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mock.Setup(x => x.IsFeatureEnabledAsync(
                It.Is<Guid>(id => !accountIds.Contains(id)),
                FeatureKeys.GpsIntegration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return mock;
    }

    /// <summary>
    /// Creates an <see cref="IAccountReader"/> mock that returns visible accounts with GPS provider integration disabled.
    /// Use when validating read-only latest-position fallbacks that must not call live provider integration.
    /// </summary>
    protected static Mock<IAccountReader> AccountReaderForDisabled(params Guid[] accountIds)
    {
        var mock = new Mock<IAccountReader>();
        var accounts = accountIds.Select(id => new AccountSettingsVm(
            id,
            StoreLastPosition: false,
            StoringInterval: 0,
            GeofencingEnabled: false,
            TripManagementEnabled: false,
            GpsIntegrationEnabled: false,
            GpsOperatorHealthEnabled: false,
            GpsPositionHistoryEnabled: false)).ToList();
        mock.Setup(x => x.GetAccountsToSyncAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);
        mock.Setup(x => x.IsFeatureEnabledAsync(
                It.Is<Guid>(id => accountIds.Contains(id)),
                FeatureKeys.GpsIntegration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        return mock;
    }
}
