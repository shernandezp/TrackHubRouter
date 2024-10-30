using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Positions.Events;

public sealed class ValidateSync
{
    public readonly record struct Notification(Guid AccountId, IEnumerable<PositionVm> Positions) : INotification
    {
        public class EventHandler(IAccountReader accountReader, IPublisher publisher) : INotificationHandler<Notification>
        {
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                var settings = await accountReader.GetAccountSettingsAsync(notification.AccountId, cancellationToken);
                if (!settings.StoreLastPosition)
                {
                    await publisher.Publish(new PositionsRetrieved.Notification(notification.Positions), cancellationToken);
                }
            }
        }
    }
}
