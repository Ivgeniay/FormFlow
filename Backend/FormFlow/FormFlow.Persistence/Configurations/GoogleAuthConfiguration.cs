using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class GoogleAuthConfiguration : IEntityTypeConfiguration<GoogleAuth>
    {
        public void Configure(EntityTypeBuilder<GoogleAuth> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.GoogleAuths);

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.GoogleAuthId);

            builder.Property(g => g.UserId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UserId);

            builder.Property(g => g.GoogleId)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.GoogleIdMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.GoogleId);

            builder.Property(g => g.Email)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.EmailMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.Email);

            builder.Property(g => g.RefreshToken)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.RefreshTokenMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.RefreshToken);

            builder.Property(g => g.TokenExpiry)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.TokenExpiry);

            builder.Property(g => g.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.Property(g => g.UpdatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UpdatedAt);

            builder.HasIndex(g => g.GoogleId)
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.GoogleAuthsGoogleIdIndex);

            builder.HasIndex(g => g.UserId)
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.GoogleAuthsUserIndex);

            builder.HasOne(g => g.User)
                .WithOne(u => u.GoogleAuth)
                .HasForeignKey<GoogleAuth>(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
