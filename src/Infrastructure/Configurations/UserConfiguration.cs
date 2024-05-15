using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.User, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.UserId).HasColumnName("id");
        builder.Property(x => x.Username).HasColumnName("username");
        builder.Property(x => x.Active).HasColumnName("active");

        builder.Property(t => t.Username)
            .HasMaxLength(ColumnMetadata.DefaultUserNameLength)
            .IsRequired();
    }
}
