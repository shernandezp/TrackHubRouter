namespace TrackHubRouter.Infrastructure.Entities;

public sealed class DevicePosition
{
    private Device? _device;

    public int DevicePositionId { get; set; }
    public Guid DeviceId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Altitude { get; set; }
    public DateTime DeviceDateTime { get; set; }
    public int Speed { get; set; }
    public int Course { get; set; }
    public int? EventId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Attributes { get; set; } //json with ignition, mileage, Hobbs Meter, temperature, etc.

    public Device Device
    {
        get => _device ?? throw new InvalidOperationException("Device is not loaded");
        set => _device = value;
    }

}
