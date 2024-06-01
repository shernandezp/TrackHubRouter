using Common.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrackHubRouter.Infrastructure.Configurations;
public sealed class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.Credential, schema: SchemaMetadata.Application);

        //Column names
        builder.Property(x => x.CredentialId).HasColumnName("id");
        builder.Property(x => x.Uri).HasColumnName("uri");
        builder.Property(x => x.Username).HasColumnName("username");
        builder.Property(x => x.Password).HasColumnName("password");
        builder.Property(x => x.Key).HasColumnName("key");
        builder.Property(x => x.Key2).HasColumnName("key2");
        builder.Property(x => x.Salt).HasColumnName("salt");
        builder.Property(x => x.Token).HasColumnName("token");
        builder.Property(x => x.TokenExpiration).HasColumnName("tokenexpiration");
        builder.Property(x => x.RefreshToken).HasColumnName("refreshtoken");
        builder.Property(x => x.OperatorId).HasColumnName("operatorid");

        builder.Property(t => t.Uri)
            .HasColumnType(ColumnMetadata.TextField)
            .IsRequired();

        builder.Property(t => t.Username)
            .HasMaxLength(ColumnMetadata.DefaultFieldLength)
            .IsRequired();

        builder.Property(t => t.Password)
            .HasColumnType(ColumnMetadata.TextField)
            .IsRequired();

        builder.Property(t => t.Key)
            .HasColumnType(ColumnMetadata.TextField)
            .IsRequired();

        builder.Property(t => t.Key2)
            .HasColumnType(ColumnMetadata.TextField)
            .IsRequired();

        builder.Property(t => t.Salt)
            .HasMaxLength(ColumnMetadata.DefaultFieldLength)
            .IsRequired();

        builder.Property(t => t.Token)
            .HasColumnType(ColumnMetadata.TextField)
            .IsRequired();

        builder.Property(t => t.RefreshToken)
            .HasColumnType(ColumnMetadata.TextField)
            .IsRequired();
    }
}
