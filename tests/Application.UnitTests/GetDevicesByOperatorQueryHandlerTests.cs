using Application.UnitTests;
using Common.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using TrackHubRouter.Application.Devices.Queries.GetByOperator;
using TrackHubRouter.Domain.Interfaces.Manager;
using TrackHubRouter.Domain.Interfaces.Operator;
using TrackHubRouter.Domain.Interfaces.Registry;
using TrackHubRouter.Domain.Models;
using TrackHubRouter.Domain.Records;

namespace TrackHubRouter.Application.UnitTests.Devices.Queries.GetByOperator;

[TestFixture]
public class GetDevicesByOperatorQueryHandlerTests : TestsContext
{
    private Mock<IConfiguration> _configurationMock;
    private Mock<IOperatorReader> _operatorReaderMock;
    private Mock<IDeviceRegistry> _deviceRegistryMock;

    private GetDevicesByOperatorQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _configurationMock = new Mock<IConfiguration>();
        _operatorReaderMock = new Mock<IOperatorReader>();
        _deviceRegistryMock = new Mock<IDeviceRegistry>();

        _configurationMock.Setup(x => x["AppSettings:EncryptionKey"]).Returns("4F2C2E66-107F-452A-ACDE-402DFD47B84C");
        _handler = new GetDevicesByOperatorQueryHandler(
            _configurationMock.Object,
            _operatorReaderMock.Object,
            _deviceRegistryMock.Object);
    }

    [Test]
    public async Task Handle_WithValidOperatorId_ReturnsDevices()
    {
        // Arrange
        var operatorId = Guid.NewGuid();
        var @operator = new OperatorVm
        {
            OperatorId = operatorId,
            ProtocolTypeId = (int)ProtocolType.CommandTrack,
            Credential = TestCredentialTokenVm
        };
        var devices = new List<DeviceVm>
        {
            new() { DeviceId = Guid.NewGuid(), Name = "Device 1" },
            new() { DeviceId = Guid.NewGuid(), Name = "Device 2" }
        };

        _operatorReaderMock.Setup(x => x.GetOperatorAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        _deviceRegistryMock.Setup(x => x.GetReader(ProtocolType.CommandTrack))
            .Returns(Mock.Of<IExternalDeviceReader>());

        var readerMock = new Mock<IExternalDeviceReader>();
        readerMock.Setup(x => x.Init(It.IsAny<CredentialTokenDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        readerMock.Setup(x => x.GetDevicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);

        _deviceRegistryMock.Setup(x => x.GetReader(ProtocolType.CommandTrack))
            .Returns(readerMock.Object);

        // Act
        var result = await _handler.Handle(new GetDevicesByOperatorQuery(operatorId), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(devices.Count));
    }

    [Test]
    public void Handle_WithInvalidOperator_ThrowsException()
    {
        // Arrange
        var operatorId = Guid.NewGuid();

        _operatorReaderMock.Setup(x => x.GetOperatorAsync(operatorId, It.IsAny<CancellationToken>()))
            .Throws(new ArgumentException());

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _handler.Handle(new GetDevicesByOperatorQuery(operatorId), CancellationToken.None));
    }

    [Test]
    public async Task Handle_WithNullCredential_ReturnsEmptyDevices()
    {
        // Arrange
        var operatorId = Guid.NewGuid();
        var @operator = new OperatorVm
        {
            OperatorId = operatorId,
            ProtocolTypeId = (int)ProtocolType.CommandTrack,
            Credential = null
        };

        _operatorReaderMock.Setup(x => x.GetOperatorAsync(operatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(@operator);

        // Act
        var result = await _handler.Handle(new GetDevicesByOperatorQuery(operatorId), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
