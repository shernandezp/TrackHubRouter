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

using Microsoft.Extensions.DependencyInjection;
using Moq;
using Common.Domain.Enums;
using TrackHubRouter.Application.Devices.Registry;
using TrackHubRouter.Domain.Interfaces.Operator;

namespace TrackHubRouter.Application.UnitTests.Devices.Registry;

[TestFixture]
public class DeviceRegistryTests
{
    [Test]
    public void GetReaders_ReturnsMatchingReaders()
    {
        // Arrange
        var reader1 = new Mock<IExternalDeviceReader>();
        reader1.SetupGet(r => r.Protocol).Returns(ProtocolType.CommandTrack);

        var reader2 = new Mock<IExternalDeviceReader>();
        reader2.SetupGet(r => r.Protocol).Returns(ProtocolType.Samsara);

        var services = new ServiceCollection();
        services.AddScoped(_ => reader1.Object);
        services.AddScoped(_ => reader2.Object);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var registry = new DeviceRegistry(scopeFactory);

        // Act
        var results = registry.GetReaders([ProtocolType.CommandTrack]).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results.First().Protocol, Is.EqualTo(ProtocolType.CommandTrack));
    }

    [Test]
    public void GetReader_ReturnsFirstMatching()
    {
        // Arrange
        var reader1 = new Mock<IExternalDeviceReader>();
        reader1.SetupGet(r => r.Protocol).Returns(ProtocolType.GpsGate);

        var reader2 = new Mock<IExternalDeviceReader>();
        reader2.SetupGet(r => r.Protocol).Returns(ProtocolType.Samsara);

        var services = new ServiceCollection();
        services.AddScoped(_ => reader1.Object);
        services.AddScoped(_ => reader2.Object);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var registry = new DeviceRegistry(scopeFactory);

        // Act
        var result = registry.GetReader(ProtocolType.Samsara);

        // Assert
        Assert.That(result.Protocol, Is.EqualTo(ProtocolType.Samsara));
    }

    [Test]
    public void GetReaders_NoMatch_ReturnsEmpty()
    {
        // Arrange
        var reader = new Mock<IExternalDeviceReader>();
        reader.SetupGet(r => r.Protocol).Returns(ProtocolType.GpsGate);

        var services = new ServiceCollection();
        services.AddScoped(_ => reader.Object);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var registry = new DeviceRegistry(scopeFactory);

        // Act
        var results = registry.GetReaders([ProtocolType.CommandTrack]).ToList();

        // Assert
        Assert.That(results, Is.Empty);
    }

    [Test]
    public void GetReader_NoMatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var reader = new Mock<IExternalDeviceReader>();
        reader.SetupGet(r => r.Protocol).Returns(ProtocolType.GpsGate);

        var services = new ServiceCollection();
        services.AddScoped(_ => reader.Object);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var registry = new DeviceRegistry(scopeFactory);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => registry.GetReader(ProtocolType.CommandTrack));
    }
}
