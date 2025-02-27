using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.DevicePositions.Events;

public sealed class PositionsRetrieved
{
    public readonly record struct Notification(IEnumerable<PositionVm> Positions) : INotification
    {
        public class EventHandler(IPositionWriter positionWriter) : INotificationHandler<Notification>
        {
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                try
                {
                    await positionWriter.AddOrUpdatePositionAsync(notification.Positions, cancellationToken);
                }
                catch
                {
                    //TODO: Log the exception
                }
            }
        }
    }
}
