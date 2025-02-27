using TrackHubRouter.Application.DevicePositions.Queries.Get;
using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.DevicePositions.Events;

public sealed class OperatorRetrieved
{
    public readonly record struct Notification(OperatorVm Operator) : INotification
    {
        public class EventHandler(ISender sender) : INotificationHandler<Notification>
        {
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
                => await sender.Send(new GetPositionsByOperatorQuery(notification.Operator), cancellationToken);
            
        }
    }
}

