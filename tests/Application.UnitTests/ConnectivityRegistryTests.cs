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

using Microsoft.Extensions.DependencyInjection;
using Moq;
using Common.Domain.Enums;
using TrackHubRouter.Application.PingOperator;
using TrackHubRouter.Domain.Interfaces;

namespace TrackHubRouter.Application.UnitTests.PingOperator.Registry;

[TestFixture]
public class ConnectivityRegistryTests
{
    [Test]
    public void GetTester_ReturnsMatchingTester()
    {
        // Arrange
        var tester1 = new Mock<IConnectivityTester>();
        tester1.SetupGet(t => t.Protocol).Returns(ProtocolType.CommandTrack);

        var tester2 = new Mock<IConnectivityTester>();
        tester2.SetupGet(t => t.Protocol).Returns(ProtocolType.Samsara);

        var services = new ServiceCollection();
        services.AddScoped(_ => tester1.Object);
        services.AddScoped(_ => tester2.Object);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var registry = new ConnectivityRegistry(scopeFactory);

        // Act
        var result = registry.GetTester(ProtocolType.Samsara);

        // Assert
        Assert.That(result.Protocol, Is.EqualTo(ProtocolType.Samsara));
    }

    [Test]
    public void GetTester_ReturnsFirstMatching_WhenMultipleRegistered()
    {
        // Arrange
        var first = new Mock<IConnectivityTester>();
        first.SetupGet(t => t.Protocol).Returns(ProtocolType.GpsGate);

        var second = new Mock<IConnectivityTester>();
        second.SetupGet(t => t.Protocol).Returns(ProtocolType.GpsGate);

        var services = new ServiceCollection();
        services.AddScoped(_ => first.Object);
        services.AddScoped(_ => second.Object);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var registry = new ConnectivityRegistry(scopeFactory);

        // Act
        var result = registry.GetTester(ProtocolType.GpsGate);

        // Assert
        Assert.That(result, Is.EqualTo(first.Object));
    }

    [Test]
    public void GetTester_NoMatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var tester = new Mock<IConnectivityTester>();
        tester.SetupGet(t => t.Protocol).Returns(ProtocolType.CommandTrack);

        var services = new ServiceCollection();
        services.AddScoped(_ => tester.Object);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var registry = new ConnectivityRegistry(scopeFactory);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => registry.GetTester(ProtocolType.Samsara));
    }
}
