using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
internal class DeviceGroupConfiguration : IEntityTypeConfiguration<DeviceGroup>
{
    public void Configure(EntityTypeBuilder<DeviceGroup> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.DeviceGroup, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.DeviceId).HasColumnName("deviceid");
        builder.Property(x => x.GroupId).HasColumnName("groupid");

    }
}
