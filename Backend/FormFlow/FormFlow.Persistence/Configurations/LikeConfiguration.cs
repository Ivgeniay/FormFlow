using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;

namespace FormFlow.Persistence.Configurations
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.ToTable(TableNames.Likes);

            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id)
                .HasColumnName(ColumnNames.LikeId);

            builder.Property(l => l.TemplateId)
                .IsRequired()
                .HasColumnName(ColumnNames.TemplateId);

            builder.Property(l => l.UserId)
                .IsRequired()
                .HasColumnName(ColumnNames.UserId);

            builder.Property(l => l.CreatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.CreatedAt);

            builder.HasIndex(l => new { l.TemplateId, l.UserId })
                .IsUnique()
                .HasDatabaseName(IndexNames.LikesTemplateUserIndex);

            builder.HasIndex(l => l.TemplateId)
                .HasDatabaseName(IndexNames.LikesTemplateIndex);

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
