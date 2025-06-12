using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.Likes);

            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.LikeId);

            builder.Property(l => l.TemplateId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateId);

            builder.Property(l => l.UserId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UserId);

            builder.Property(l => l.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.Property(l => l.IsDeleted)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsDeleted)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsDeletedDefault);

            builder.HasIndex(l => new { l.TemplateId, l.UserId })
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.LikesTemplateUserIndex);

            builder.HasIndex(l => l.TemplateId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.LikesTemplateIndex);

            builder.HasOne(l => l.Template)
                .WithMany(t => t.Likes)
                .HasForeignKey(l => l.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
