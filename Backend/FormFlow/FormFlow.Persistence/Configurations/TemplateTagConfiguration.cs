using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;

namespace FormFlow.Persistence.Configurations
{
    public class TemplateTagConfiguration : IEntityTypeConfiguration<TemplateTag>
    {
        public void Configure(EntityTypeBuilder<TemplateTag> builder)
        {
            builder.ToTable(TableNames.TemplateTags);

            builder.HasKey(tt => tt.Id);
            builder.Property(tt => tt.Id)
                .HasColumnName(ColumnNames.TemplateTagId);

            builder.Property(tt => tt.TemplateId)
                .IsRequired()
                .HasColumnName(ColumnNames.TemplateId);

            builder.Property(tt => tt.TagId)
                .IsRequired()
                .HasColumnName(ColumnNames.TagId);

            builder.Property(tt => tt.CreatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.CreatedAt);

            builder.HasIndex(tt => new { tt.TemplateId, tt.TagId })
                .IsUnique()
                .HasDatabaseName(IndexNames.TemplateTagsTemplateTagIndex);

            builder.HasIndex(tt => tt.TagId)
                .HasDatabaseName(IndexNames.TemplateTagsTagIndex);

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
