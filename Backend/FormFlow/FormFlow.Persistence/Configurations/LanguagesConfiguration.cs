using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormFlow.Persistence.Configurations
{
    public class LanguagesConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages");
            
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .HasColumnName("language_id")
                .ValueGeneratedOnAdd();

            builder.Property(l => l.Code)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("code");

            builder.Property(l => l.ShortCode)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("short_code");

            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");

            builder.Property(l => l.IconURL)
                .HasMaxLength(255)
                .HasColumnName("icon_url");

            builder.Property(l => l.Region)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("region");

            builder.Property(l => l.IsDefault)
                .IsRequired()
                .HasColumnName("is_default")
                .HasDefaultValue(false);

            builder.Property(l => l.IsActive)
                .IsRequired()
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            builder.Property(l => l.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(l => l.Code)
                .IsUnique()
                .HasDatabaseName("idx_Languages_Code");

            builder.HasIndex(l => l.Name)
                .IsUnique()
                .HasDatabaseName("idx_Languages_Name");

        }
    }
}
