using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class EmailPasswordAuthConfiguration : IEntityTypeConfiguration<EmailPasswordAuth>
    {
        public void Configure(EntityTypeBuilder<EmailPasswordAuth> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.EmailPasswordAuths);

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.EmailPasswordAuthId);

            builder.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UserId);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.EmailMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.Email);

            builder.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.PasswordHashMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.PasswordHash);

            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.EmailPasswordAuthsEmailIndex);

            builder.HasIndex(e => e.UserId)
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.EmailPasswordAuthsUserIndex);

            builder.HasOne(e => e.User)
                .WithOne(u => u.EmailAuth)
                .HasForeignKey<EmailPasswordAuth>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
