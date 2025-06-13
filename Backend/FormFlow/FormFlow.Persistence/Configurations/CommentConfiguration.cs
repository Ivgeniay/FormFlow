using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.Comments);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.CommentId);

            builder.Property(c => c.TemplateId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateId);

            builder.Property(c => c.UserId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UserId);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.CommentMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.CommentContent);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.Property(c => c.UpdatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UpdatedAt);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsDeleted)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsDeletedDefault);

            builder.HasIndex(c => c.TemplateId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.CommentsTemplateIndex);

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
