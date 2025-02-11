namespace TrackHubRouter.Domain.Models;

public struct TripVm
{
    public Guid TripId { get; set; }
    public List<PositionVm> Points { get; set; }
    public double TotalDistance { get; set; }
    public TimeSpan Duration { get; set; }
    public double AverageSpeed { get; set; }
}
