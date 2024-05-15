namespace TrackHubRouter.Infrastructure.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
