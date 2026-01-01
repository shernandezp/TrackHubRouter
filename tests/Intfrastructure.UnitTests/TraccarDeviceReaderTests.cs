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

using Moq;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;
using TrackHub.Router.Infrastructure.Traccar.Models;
using Common.Domain.Enums;

namespace TrackHub.Router.Infrastructure.Traccar.Tests;

[TestFixture]
public class DeviceReaderTests
{
    private Mock<ICredentialHttpClientFactory> _httpClientFactoryMock;
    private Mock<IHttpClientService> _httpClientServiceMock;
    private DeviceReader _deviceReader;

    [SetUp]
    public void Setup()
    {
        _httpClientFactoryMock = new Mock<ICredentialHttpClientFactory>();
        _httpClientServiceMock = new Mock<IHttpClientService>();
        _deviceReader = new DeviceReader(_httpClientFactoryMock.Object, _httpClientServiceMock.Object);
    }

    [Test]
    public async Task GetDeviceAsync_ValidDevice_ReturnsDeviceVm()
    {
        // Arrange
        var deviceDto = new DeviceTransporterVm { Identifier = 1 };
        var cancellationToken = CancellationToken.None;
        var device = new Device();
        var expectedDeviceVm = new DeviceVm { DeviceId = Guid.Empty, DeviceTypeId = (short)DeviceType.Cellular, TransporterTypeId = (short)TransporterType.Truck };

        _httpClientServiceMock.Setup(x => x.GetAsync<Device>("api/devices?id=1", null, cancellationToken))
            .ReturnsAsync(device);

        // Act
        var result = await _deviceReader.GetDeviceAsync(deviceDto, cancellationToken);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDeviceVm));
    }

    [Test]
    public async Task GetDevicesAsync_ValidDevices_ReturnsDeviceVms()
    {
        // Arrange
        var devices = new List<DeviceTransporterVm>
        {
            new () { Identifier = 1 },
            new () { Identifier = 2 }
        };
        var cancellationToken = CancellationToken.None;
        var resultDevices = new List<Device>();
        var expectedDeviceVms = new List<DeviceVm>();

        _httpClientServiceMock.Setup(x => x.GetAsync<IEnumerable<Device>>("api/devices?id=1,2", null, cancellationToken))
            .ReturnsAsync(resultDevices);

        // Act
        var result = await _deviceReader.GetDevicesAsync(devices, cancellationToken);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDeviceVms));
    }

    [Test]
    public async Task GetDevicesAsync_NoDevices_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var positions = new List<Device>();

        _httpClientServiceMock.Setup(x => x.GetAsync<IEnumerable<Device>>("api/devices?all=true", null, cancellationToken))
            .ReturnsAsync(positions);

        // Act
        var result = await _deviceReader.GetDevicesAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
