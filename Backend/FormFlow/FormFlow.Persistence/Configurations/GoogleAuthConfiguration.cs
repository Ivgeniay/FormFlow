using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;
using static DomainConstants;

namespace FormFlow.Persistence.Configurations
{
    public class GoogleAuthConfiguration : IEntityTypeConfiguration<GoogleAuth>
    {
        public void Configure(EntityTypeBuilder<GoogleAuth> builder)
        {
            builder.ToTable(TableNames.GoogleAuths);

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id)
                .HasColumnName(ColumnNames.GoogleAuthId);

            builder.Property(g => g.UserId)
                .IsRequired()
                .HasColumnName(ColumnNames.UserId);

            builder.Property(g => g.GoogleId)
                .IsRequired()
                .HasMaxLength(Validation.GoogleIdMaxLength)
                .HasColumnName(ColumnNames.GoogleId);

            builder.Property(g => g.Email)
                .IsRequired()
                .HasMaxLength(Validation.EmailMaxLength)
                .HasColumnName(ColumnNames.Email);

            builder.Property(g => g.RefreshToken)
                .IsRequired()
                .HasMaxLength(Validation.RefreshTokenMaxLength)
                .HasColumnName(ColumnNames.RefreshToken);

            builder.Property(g => g.RefreshTokenExpiresAt)
                .IsRequired()
                .HasColumnName(ColumnNames.RefreshTokenExpiresAt);

            builder.Property(g => g.RefreshTokenRevokedAt)
                .HasColumnName(ColumnNames.RefreshTokenRevokedAt);

            builder.Property(g => g.CreatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.CreatedAt);

            builder.Property(g => g.UpdatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.UpdatedAt);

            builder.HasIndex(g => g.GoogleId)
                .IsUnique()
                .HasDatabaseName(IndexNames.GoogleAuthsGoogleIdIndex);

            builder.HasIndex(g => g.UserId)
                .IsUnique()
                .HasDatabaseName(IndexNames.GoogleAuthsUserIndex);

            builder.HasIndex(g => g.RefreshToken)
                .HasDatabaseName(IndexNames.GoogleAuthsRefreshTokenIndex);

            builder.HasOne(g => g.User)
                .WithOne(u => u.GoogleAuth)
                .HasForeignKey<GoogleAuth>(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
