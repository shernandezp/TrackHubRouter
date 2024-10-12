using Moq;
using TrackHubRouter.Domain.Interfaces;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHub.Router.Infrastructure.CommandTrack.Models;
using TrackHubRouter.Domain.Models;
using FluentAssertions;

namespace TrackHub.Router.Infrastructure.CommandTrack.Tests;

[TestFixture]
public class DeviceReaderTests
{
    private Mock<ICredentialHttpClientFactory> _httpClientFactoryMock;
    private Mock<IHttpClientService> _httpClientServiceMock;
    private Mock<ICredentialWriter> _credentialWriterMock;
    private DeviceReader _deviceReader;
    private IDictionary<string, string>? _header;

    [SetUp]
    public void Setup()
    {
        _httpClientFactoryMock = new Mock<ICredentialHttpClientFactory>();
        _httpClientServiceMock = new Mock<IHttpClientService>();
        _credentialWriterMock = new Mock<ICredentialWriter>();
        _deviceReader = new DeviceReader(_httpClientFactoryMock.Object, _httpClientServiceMock.Object, _credentialWriterMock.Object);
        _header = new Dictionary<string, string> { { "", "" } };
    }

    [Test]
    public async Task GetDeviceAsync_WithValidDeviceDto_ReturnsDeviceVm()
    {
        // Arrange
        var deviceDto = new DeviceTransporterVm { Identifier = 1 };
        var devicePosition = new DevicePosition();
        var expectedDeviceVm = new DeviceVm { DeviceId = Guid.Empty };

        _httpClientServiceMock.Setup(x => x.GetAsync<DevicePosition>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(devicePosition);

        // Act
        var result = await _deviceReader.GetDeviceAsync(deviceDto, CancellationToken.None);

        // Assert
        expectedDeviceVm.Should().Be(result);
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
        expectedDeviceVms.Should().BeEquivalentTo(result);
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
        result.Should().BeEmpty();
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
        result.Should().BeEmpty();
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
        result.Should().BeEmpty();
    }
}
