using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormFlow.Persistence.Configurations
{
    public class TopicsConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.ToTable("Topics");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("topic_id");

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("topic_name");

            builder
                .HasIndex(t => t.Name)
                .HasDatabaseName("idx_topic_name")
                .IsUnique();
        }
    }
}
