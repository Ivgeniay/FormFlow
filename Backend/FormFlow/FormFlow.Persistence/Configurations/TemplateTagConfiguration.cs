using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class TemplateTagConfiguration : IEntityTypeConfiguration<TemplateTag>
    {
        public void Configure(EntityTypeBuilder<TemplateTag> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.TemplateTags);

            builder.HasKey(tt => tt.Id);
            builder.Property(tt => tt.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateTagId);

            builder.Property(tt => tt.TemplateId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateId);

            builder.Property(tt => tt.TagId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.TagId);

            builder.Property(tt => tt.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.HasIndex(tt => new { tt.TemplateId, tt.TagId })
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.TemplateTagsTemplateTagIndex);

            builder.HasIndex(tt => tt.TagId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.TemplateTagsTagIndex);

            builder.HasOne(tt => tt.Template)
                .WithMany(t => t.Tags)
                .HasForeignKey(tt => tt.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tt => tt.Tag)
                .WithMany(t => t.Templates)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
