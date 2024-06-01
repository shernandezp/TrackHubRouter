namespace TrackHubRouter.Infrastructure.Entities;
public sealed class DeviceGroup
{
    private Device? _device;
    private User? _user;

    public required Guid DeviceId { get; set; }
    public required long GroupId { get; set; }

    public Device Device
    {
        get => _device ?? throw new InvalidOperationException("Device is not loaded");
        set => _device = value;
    }
    public User User
    {
        get => _user ?? throw new InvalidOperationException("User is not loaded");
        set => _user = value;
    }
}
