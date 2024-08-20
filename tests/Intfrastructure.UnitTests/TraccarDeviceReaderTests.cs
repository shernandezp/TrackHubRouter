using Moq;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Models;
using TrackHub.Router.Infrastructure.Traccar.Models;
using FluentAssertions;

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
        var deviceDto = new DeviceOperatorVm { Identifier = 1 };
        var cancellationToken = CancellationToken.None;
        var device = new Device();
        var expectedDeviceVm = new DeviceVm() { DeviceId = Guid.Empty };

        _httpClientServiceMock.Setup(x => x.GetAsync<Device>("api/devices?id=1", null, cancellationToken))
            .ReturnsAsync(device);

        // Act
        var result = await _deviceReader.GetDeviceAsync(deviceDto, cancellationToken);

        // Assert
        expectedDeviceVm.Should().Be(result);
    }

    [Test]
    public async Task GetDevicesAsync_ValidDevices_ReturnsDeviceVms()
    {
        // Arrange
        var devices = new List<DeviceOperatorVm>
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
        expectedDeviceVms.Should().BeEquivalentTo(result);
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
        result.Should().BeEmpty();
    }
}
