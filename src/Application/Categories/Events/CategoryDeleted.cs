namespace TrackHubRouter.Application.Categories.Events;

public sealed class CategoryDeleted
{
    public class Notification(Guid id) : INotification
    {
        public Guid Id { get; } = id;
    }
}
