using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;

namespace FormFlow.Persistence.Configurations
{
    public class EmailPasswordAuthConfiguration : IEntityTypeConfiguration<EmailPasswordAuth>
    {
        public void Configure(EntityTypeBuilder<EmailPasswordAuth> builder)
        {
            builder.ToTable(TableNames.EmailPasswordAuths);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasColumnName(ColumnNames.EmailPasswordAuthId);

            builder.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName(ColumnNames.UserId);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.EmailMaxLength)
                .HasColumnName(ColumnNames.Email);

            builder.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.PasswordHashMaxLength)
                .HasColumnName(ColumnNames.PasswordHash);

            builder.Property(e => e.RefreshToken)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.RefreshTokenMaxLength)
                .HasColumnName(ColumnNames.RefreshToken)
                .HasDefaultValue(string.Empty);

            builder.Property(e => e.RefreshTokenExpiresAt)
                .IsRequired()
                .HasColumnName(ColumnNames.RefreshTokenExpiresAt);

            builder.Property(e => e.RefreshTokenRevokedAt)
                .HasColumnName(ColumnNames.RefreshTokenRevokedAt);

            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName(IndexNames.EmailPasswordAuthsEmailIndex);

            builder.HasIndex(e => e.UserId)
                .IsUnique()
                .HasDatabaseName(IndexNames.EmailPasswordAuthsUserIndex);

            builder.HasIndex(e => e.RefreshToken)
                .HasDatabaseName(IndexNames.EmailPasswordRefreshTokenIndex);

            builder.HasOne(e => e.User)
                .WithOne(u => u.EmailAuth)
                .HasForeignKey<EmailPasswordAuth>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
   
    }
}
