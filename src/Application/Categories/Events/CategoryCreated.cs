namespace TrackHubRouter.Application.Categories.Events;

public sealed class CategoryCreated
{
    public class Notification(Guid id) : INotification
    {
        public Guid Id { get; } = id;
    }
}
