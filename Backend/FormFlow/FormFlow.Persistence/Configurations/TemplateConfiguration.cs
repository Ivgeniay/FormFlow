using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class TemplateConfiguration : IEntityTypeConfiguration<Template>
    {
        public void Configure(EntityTypeBuilder<Template> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.Templates);

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateId);

            builder.Property(t => t.AuthorId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.AuthorId);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.TemplateNameMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateTitle);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.TemplateDescriptionMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateDescription);

            builder.Property(t => t.ImageUrl)
                .HasMaxLength(DomainConstants.Validation.ImageUrlMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.ImageUrl);

            builder.Property(t => t.AccessType)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName(DomainConstants.Database.ColumnNames.AccessType);

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.Property(t => t.UpdatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UpdatedAt);

            builder.Property(t => t.IsDeleted)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsDeleted)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsDeletedDefault);

            builder.Property(t => t.IsArchived)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsArchived)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsArchivedDefault);

            builder.Property(t => t.IsPublished)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsPublished)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsPublishedDefault);

            builder.Property(t => t.Version)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.Version)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.VersionDefault);

            builder.Property(t => t.IsCurrentVersion)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsCurrentVersion)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsCurrentVersionDefault);

            builder.Property(t => t.BaseTemplateId)
                .HasColumnName(DomainConstants.Database.ColumnNames.BaseTemplateId);

            builder.Property(t => t.PreviousVersionId)
                .HasColumnName(DomainConstants.Database.ColumnNames.PreviousVersionId);

            builder.Ignore(t => t.LikesCount);
            builder.Ignore(t => t.FormsCount);
            builder.Ignore(t => t.CommentsCount);

            builder.HasIndex(t => t.AuthorId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.TemplatesAuthorIndex);

            builder.HasIndex(t => t.BaseTemplateId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.TemplatesBaseTemplateIndex);

            builder.HasIndex(t => new { t.BaseTemplateId, t.Version })
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.TemplatesBaseVersionIndex);

            builder.HasOne(t => t.Author)
                .WithMany(u => u.Templates)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Questions)
                .WithOne(q => q.Template)
                .HasForeignKey(q => q.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Forms)
                .WithOne(f => f.Template)
                .HasForeignKey(f => f.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.Comments)
                .WithOne(c => c.Template)
                .HasForeignKey(c => c.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Likes)
                .WithOne(l => l.Template)
                .HasForeignKey(l => l.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.AllowedUsers)
                .WithOne(tau => tau.Template)
                .HasForeignKey(tau => tau.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
