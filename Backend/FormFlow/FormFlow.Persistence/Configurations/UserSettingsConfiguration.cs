using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormFlow.Persistence.Configurations
{
    public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            builder.ToTable("UserSettings");

            builder.HasKey(us => us.Id);

            builder.Property(us => us.Id)
                .HasColumnName("user_settings_id")
                .ValueGeneratedOnAdd();

            builder.Property(us => us.UserId)
                .IsRequired()
                .HasColumnName("user_id");

            builder.Property(us => us.ColorThemeId)
                .IsRequired()
                .HasColumnName("color_theme_id");

            builder.Property(us => us.LanguageId)
                .IsRequired()
                .HasColumnName("language_id");

            builder.Property(us => us.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(us => us.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasColumnName("updated_at")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(us => us.User)
                .WithOne()
                .HasForeignKey<UserSettings>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(us => us.ColorTheme)
                .WithMany()
                .HasForeignKey(us => us.ColorThemeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(us => us.Language)
                .WithMany()
                .HasForeignKey(us => us.LanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(us => us.UserId)
                .IsUnique()
                .HasDatabaseName("idx_UserSettings_user_Id");
        }
    }
}
