using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
public sealed class OperatorConfiguration : IEntityTypeConfiguration<Operator>
{
    public void Configure(EntityTypeBuilder<Operator> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.Operator, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.OperatorId).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.PhoneNumber).HasColumnName("phonenumber");
        builder.Property(x => x.EmailAddress).HasColumnName("emailaddress");
        builder.Property(x => x.Address).HasColumnName("address");
        builder.Property(x => x.ContactName).HasColumnName("contactname");
        builder.Property(x => x.ProtocolType).HasColumnName("protocoltype");
        builder.Property(x => x.AccountId).HasColumnName("accountid");

        builder.Property(t => t.Name)
            .HasMaxLength(ColumnMetadata.DefaultNameLength)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(ColumnMetadata.DefaultDescriptionLength)
            .IsRequired();

        builder.Property(t => t.PhoneNumber)
            .HasMaxLength(ColumnMetadata.DefaultPhoneNumberLength)
            .IsRequired();

        builder.Property(t => t.EmailAddress)
            .HasMaxLength(ColumnMetadata.DefaultEmailLength)
            .IsRequired();

        builder.Property(t => t.Address)
            .HasMaxLength(ColumnMetadata.DefaultAddressLength)
            .IsRequired();

        builder.Property(t => t.ContactName)
            .HasMaxLength(ColumnMetadata.DefaultFieldLength)
            .IsRequired();

        builder
            .HasOne(d => d.Credential)
            .WithOne(d => d.Operator)
            .HasForeignKey<Credential>(d => d.OperatorId)
            .IsRequired(false);
    }
}
