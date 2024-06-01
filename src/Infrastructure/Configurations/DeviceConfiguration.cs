using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
public sealed class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.Device, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.DeviceId).HasColumnName("id");
        builder.Property(x => x.Identifier).HasColumnName("identifier");
        builder.Property(x => x.Name).HasColumnName("name");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.DeviceTypeId).HasColumnName("devicetypeid");

        builder.Property(t => t.Name)
            .HasMaxLength(ColumnMetadata.DefaultNameLength)
            .IsRequired();

        builder.Property(t => t.Identifier)
            .HasMaxLength(ColumnMetadata.DefaultFieldLength)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(ColumnMetadata.DefaultDescriptionLength)
            .IsRequired();

        builder
            .HasOne(d => d.Transporter)
            .WithOne(d => d.Device)
            .HasForeignKey<Transporter>(d => d.DeviceId)
            .IsRequired(false);
    }
}
