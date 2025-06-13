using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;
using static DomainConstants;

namespace FormFlow.Persistence.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable(TableNames.Comments);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName(ColumnNames.CommentId);

            builder.Property(c => c.TemplateId)
                .IsRequired()
                .HasColumnName(ColumnNames.TemplateId);

            builder.Property(c => c.UserId)
                .IsRequired()
                .HasColumnName(ColumnNames.UserId);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(Validation.CommentMaxLength)
                .HasColumnName(ColumnNames.CommentContent);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.CreatedAt);

            builder.Property(c => c.UpdatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.UpdatedAt);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasColumnName(ColumnNames.IsDeleted)
                .HasDefaultValue(DefaultValues.IsDeletedDefault);

            builder.HasIndex(c => c.TemplateId)
                .HasDatabaseName(IndexNames.CommentsTemplateIndex);

            builder.HasOne(c => c.Template)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
