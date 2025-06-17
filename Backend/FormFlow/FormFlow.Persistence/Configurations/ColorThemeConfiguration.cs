using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormFlow.Persistence.Configurations
{
    public class ColorThemeConfiguration : IEntityTypeConfiguration<ColorTheme>
    {
        public void Configure(EntityTypeBuilder<ColorTheme> builder)
        {
            builder.ToTable("ColorThemes");

            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Id)
                .HasColumnName("color_theme_id")
                .ValueGeneratedOnAdd();

            builder.Property(ct => ct.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");

            builder.Property(ct => ct.CssClass)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("css_class");

            builder.Property(ct => ct.ColorVariables)
                .IsRequired()
                .HasMaxLength(300)
                .HasColumnName("color_variables");

            builder.Property(ct => ct.IsDefault)
                .IsRequired()
                .HasColumnName("is_default")
                .HasDefaultValue(false);

            builder.Property(ct => ct.IsActive)
                .IsRequired()
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            builder.Property(ct => ct.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(ct => ct.Name)
                .IsUnique()
                .HasDatabaseName("idx_ColorThemes_Name");

            builder.HasIndex(ct => ct.CssClass)
                .IsUnique()
                .HasDatabaseName("idx_ColorThemes_CssClass");
        }
    }
}
