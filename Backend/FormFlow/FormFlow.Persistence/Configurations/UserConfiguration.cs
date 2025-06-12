using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.Users);

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.UserId);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.UserNameMaxLength);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.Property(u => u.UpdatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UpdatedAt);

            builder.Property(u => u.IsBlocked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(u => u.UserName)
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.UsersuserNameIndex);

            builder.HasOne(u => u.EmailAuth)
                .WithOne(e => e.User)
                .HasForeignKey<EmailPasswordAuth>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.GoogleAuth)
                .WithOne(g => g.User)
                .HasForeignKey<GoogleAuth>(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Contacts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Templates)
                .WithOne(t => t.Author)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Forms)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Likes)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
