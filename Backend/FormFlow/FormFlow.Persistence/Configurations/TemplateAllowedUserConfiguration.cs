using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class TemplateAllowedUserConfiguration : IEntityTypeConfiguration<TemplateAllowedUser>
    {
        public void Configure(EntityTypeBuilder<TemplateAllowedUser> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.TemplateAllowedUsers);

            builder.HasKey(tau => tau.Id);
            builder.Property(tau => tau.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateAllowedUserId);

            builder.Property(tau => tau.TemplateId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateId);

            builder.Property(tau => tau.UserId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UserId);

            builder.Property(tau => tau.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.HasIndex(tau => new { tau.TemplateId, tau.UserId })
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.TemplateAllowedUsersTemplateUserIndex);

            builder.HasIndex(tau => tau.TemplateId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.TemplateAllowedUsersTemplateIndex);

            builder.HasOne(tau => tau.Template)
                .WithMany(t => t.AllowedUsers)
                .HasForeignKey(tau => tau.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tau => tau.User)
                .WithMany()
                .HasForeignKey(tau => tau.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
