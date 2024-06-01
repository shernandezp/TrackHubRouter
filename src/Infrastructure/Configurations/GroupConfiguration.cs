using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
public sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.Group, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.GroupId).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.IsMaster).HasColumnName("ismaster");
        builder.Property(x => x.Active).HasColumnName("active");
        builder.Property(x => x.AccountId).HasColumnName("accountid");

        builder.Property(t => t.Name)
            .HasMaxLength(ColumnMetadata.DefaultNameLength)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(ColumnMetadata.DefaultDescriptionLength)
            .IsRequired();

        builder
            .HasMany(e => e.Users)
            .WithMany(e => e.Groups)
            .UsingEntity<UserGroup>();

        builder
            .HasMany(e => e.Devices)
            .WithMany(e => e.Groups)
            .UsingEntity<DeviceGroup>();
    }
}
