using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;
using static DomainConstants;

namespace FormFlow.Persistence.Configurations
{
    public class TemplateConfiguration : IEntityTypeConfiguration<Template>
    {
        public void Configure(EntityTypeBuilder<Template> builder)
        {
            builder.ToTable(TableNames.Templates);

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName(ColumnNames.TemplateId);

            builder.Property(t => t.AuthorId)
                .IsRequired()
                .HasColumnName(ColumnNames.AuthorId);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(Validation.TemplateNameMaxLength)
                .HasColumnName(ColumnNames.TemplateTitle);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(Validation.TemplateDescriptionMaxLength)
                .HasColumnName(ColumnNames.TemplateDescription);

            builder.Property(t => t.TopicId)
                .IsRequired()
                .HasColumnName("topic_id");

            builder.HasOne<Topic>()
                .WithMany()
                .HasForeignKey(t => t.TopicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.ImageUrl)
                .HasMaxLength(Validation.ImageUrlMaxLength)
                .HasColumnName(ColumnNames.ImageUrl);

            builder.Property(t => t.AccessType)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName(ColumnNames.AccessType);

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.CreatedAt);

            builder.Property(t => t.UpdatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.UpdatedAt);

            builder.Property(t => t.IsDeleted)
                .IsRequired()
                .HasColumnName(ColumnNames.IsDeleted)
                .HasDefaultValue(DefaultValues.IsDeletedDefault);

            builder.Property(t => t.IsArchived)
                .IsRequired()
                .HasColumnName(ColumnNames.IsArchived)
                .HasDefaultValue(DefaultValues.IsArchivedDefault);

            builder.Property(t => t.IsPublished)
                .IsRequired()
                .HasColumnName(ColumnNames.IsPublished)
                .HasDefaultValue(DefaultValues.IsPublishedDefault);

            builder.Property(t => t.Version)
                .IsRequired()
                .HasColumnName(ColumnNames.Version)
                .HasDefaultValue(DefaultValues.VersionDefault);

            builder.Property(t => t.BaseTemplateId)
                .HasColumnName(ColumnNames.BaseTemplateId);

            builder.Property(t => t.PreviousVersionId)
                .HasColumnName(ColumnNames.PreviousVersionId);

            builder.Ignore(t => t.LikesCount);
            builder.Ignore(t => t.FormsCount);
            builder.Ignore(t => t.CommentsCount);

            builder.HasIndex(t => t.AuthorId)
                .HasDatabaseName(IndexNames.TemplatesAuthorIndex);

            builder.HasIndex(t => t.BaseTemplateId)
                .HasDatabaseName(IndexNames.TemplatesBaseTemplateIndex);

            builder.HasIndex(t => new { t.BaseTemplateId, t.Version })
                .IsUnique()
                .HasDatabaseName(IndexNames.TemplatesBaseVersionIndex);

            builder.HasOne(t => t.Author)
                .WithMany(u => u.Templates)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Topic)
                .WithMany(topic => topic.Templates)
                .HasForeignKey(t => t.TopicId)
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
