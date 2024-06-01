using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
public sealed class TransporterConfiguration : IEntityTypeConfiguration<Transporter>
{
    public void Configure(EntityTypeBuilder<Transporter> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.Transporter, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.TransporterId).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name");
        builder.Property(x => x.TransporterTypeId).HasColumnName("transportertypeid");
        builder.Property(x => x.Icon).HasColumnName("icon");
        builder.Property(x => x.DeviceId).HasColumnName("deviceid");

        builder.Property(t => t.Name)
            .HasMaxLength(ColumnMetadata.DefaultNameLength)
            .IsRequired();
    }
}
