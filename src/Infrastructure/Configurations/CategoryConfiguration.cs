using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.Category, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.CategoryId).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Type).HasColumnName("type");
        builder.Property(x => x.Active).HasColumnName("active");

        builder.Property(t => t.Name)
            .HasMaxLength(ColumnMetadata.DefaultNameLength)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(ColumnMetadata.DefaultDescriptionLength)
            .IsRequired();
    }
}
