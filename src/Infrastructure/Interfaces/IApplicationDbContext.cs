namespace TrackHubRouter.Infrastructure.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<Credential> Credentials { get; set; }
    DbSet<Device> Devices { get; set; }
    DbSet<DeviceGroup> DeviceGroups { get; set; }
    DbSet<Group> Groups { get; set; }
    DbSet<Operator> Operators { get; set; }
    DbSet<Transporter> Transporters { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<UserGroup> UserGroups { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
