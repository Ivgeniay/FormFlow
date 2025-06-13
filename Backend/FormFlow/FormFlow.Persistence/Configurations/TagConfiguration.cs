using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;
using static DomainConstants;

namespace FormFlow.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable(TableNames.Tags);

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName(ColumnNames.TagId);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(Validation.TagNameMaxLength)
                .HasColumnName(ColumnNames.TagName);

            builder.Property(t => t.UsageCount)
                .IsRequired()
                .HasColumnName(ColumnNames.UsageCount)
                .HasDefaultValue(DefaultValues.UsageCountDefault);

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.CreatedAt);

            builder.Property(t => t.UpdatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.UpdatedAt);

            builder.HasIndex(t => t.Name)
                .IsUnique()
                .HasDatabaseName(IndexNames.TagsNameIndex);
        }
    }
}
