namespace TrackHubRouter.Application.Categories.Events;

public sealed class CategoryUpdated
{
    public class Notification(Guid id) : INotification
    {
        public Guid Id { get; } = id;
    }
}
