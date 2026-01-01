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
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHub.Router.Infrastructure.CommandTrack.Models;
using TrackHubRouter.Domain.Models;
using Common.Domain.Enums;

namespace TrackHub.Router.Infrastructure.CommandTrack.Tests;

[TestFixture]
public class DeviceReaderTests
{
    private Mock<ICredentialHttpClientFactory> _httpClientFactoryMock;
    private Mock<IHttpClientService> _httpClientServiceMock;
    private Mock<ICredentialWriter> _credentialWriterMock;
    private DeviceReader _deviceReader;

    [SetUp]
    public void Setup()
    {
        _httpClientFactoryMock = new Mock<ICredentialHttpClientFactory>();
        _httpClientServiceMock = new Mock<IHttpClientService>();
        _credentialWriterMock = new Mock<ICredentialWriter>();
        _deviceReader = new DeviceReader(_httpClientFactoryMock.Object, _httpClientServiceMock.Object, _credentialWriterMock.Object);
    }

    [Test]
    public async Task GetDeviceAsync_WithValidDeviceDto_ReturnsDeviceVm()
    {
        // Arrange
        var deviceDto = new DeviceTransporterVm { Identifier = 1 };
        var devicePosition = new DevicePosition();
        var expectedDeviceVm = new DeviceVm { DeviceId = Guid.Empty, DeviceTypeId = (short)DeviceType.Cellular, TransporterTypeId = (short)TransporterType.Truck };

        _httpClientServiceMock.Setup(x => x.GetAsync<DevicePosition>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(devicePosition);

        // Act
        var result = await _deviceReader.GetDeviceAsync(deviceDto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDeviceVm));
    }

    [Test]
    public async Task GetDevicesAsync_WithValidDevices_ReturnsListOfDeviceVm()
    {
        // Arrange
        var devices = new List<DeviceTransporterVm> { new () { Identifier = 1 }, new () { Identifier = 2 } };
        var devicePositions = new List<DevicePosition>();
        var expectedDeviceVms = new List<DeviceVm>();

        _httpClientServiceMock.Setup(x => x.GetAsync<IEnumerable<DevicePosition>>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(devicePositions);

        // Act
        var result = await _deviceReader.GetDevicesAsync(devices, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDeviceVms));
    }

    [Test]
    public async Task GetDevicesAsync_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var devices = new List<DeviceTransporterVm> { new () { Identifier = 1 }, new () { Identifier = 2 } };
        var emptyDevices = new List<DevicePosition>();

        _httpClientServiceMock.Setup(x => x.GetAsync<IEnumerable<DevicePosition>>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyDevices);

        // Act
        var result = await _deviceReader.GetDevicesAsync(devices, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetDevicesAsync_WithNoDevices_ReturnsEmptyList()
    {
        // Arrange
        var devices = new List<DeviceTransporterVm>();
        var devicePositions = new List<DevicePosition> ();

        _httpClientServiceMock.Setup(x => x.GetAsync<IEnumerable<DevicePosition>>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(devicePositions);

        // Act
        var result = await _deviceReader.GetDevicesAsync(devices, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetDevicesAsync_WithNoPositions_ReturnsEmptyList()
    {
        // Arrange
        var emptyDevices = new List<DevicePosition>();
        _httpClientServiceMock.Setup(x => x.GetAsync<IEnumerable<DevicePosition>>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyDevices);

        // Act
        var result = await _deviceReader.GetDevicesAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
