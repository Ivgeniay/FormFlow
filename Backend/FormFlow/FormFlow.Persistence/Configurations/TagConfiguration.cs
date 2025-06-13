using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.Tags);

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.TagId);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.TagNameMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.TagName);

            builder.Property(t => t.UsageCount)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UsageCount)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.UsageCountDefault);

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.Property(t => t.UpdatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UpdatedAt);

            builder.HasIndex(t => t.Name)
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.TagsNameIndex);
        }
    }
}
