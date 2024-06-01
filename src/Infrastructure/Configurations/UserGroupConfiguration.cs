using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
internal class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.UserGroup, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.UserId).HasColumnName("userid");
        builder.Property(x => x.GroupId).HasColumnName("groupid");

    }
}
