using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.Account, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.AccountId).HasColumnName("id");
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

        builder
            .HasMany(e => e.Users)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .IsRequired();

        builder
            .HasMany(e => e.Groups)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .IsRequired();

        builder
            .HasMany(e => e.Operators)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .IsRequired();
    }
}
