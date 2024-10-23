using TrackHubRouter.Domain.Models;

namespace TrackHubRouter.Application.Positions.Events;
public sealed class PositionsRetrieved
{
    public readonly record struct Notification(OperatorVm Operator, IEnumerable<PositionVm> Positions) : INotification
    {
        public class EventHandler(IAccountReader accountReader, IPositionWriter positionWriter) : INotificationHandler<Notification>
        {
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                var settings = await accountReader.GetAccountSettingsAsync(notification.Operator.AccountId, cancellationToken);
                if (!settings.StoreLastPosition)
                {
                    await positionWriter.AddOrUpdatePositionAsync(notification.Positions, cancellationToken);
                }
            }
        }
    }
}
