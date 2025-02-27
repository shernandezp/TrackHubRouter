namespace TrackHubRouter.Domain.Models;

public struct TripVm
{
    public Guid TripId { get; set; }
    public List<TripPointVm> Points { get; set; }
    public double TotalDistance { get; set; }
    public TimeSpan Duration { get; set; }
    public double AverageSpeed { get; set; }
    public short Type { get; set; }
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
}
