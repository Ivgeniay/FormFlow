using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormFlow.Persistence.Configurations
{
    public class ApiTokenConfiguration : IEntityTypeConfiguration<ApiToken>
    {
        public void Configure(EntityTypeBuilder<ApiToken> builder)
        {
            builder.ToTable("ApiTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TokenHash)
                .IsUnique()
                .HasDatabaseName("IX_ApiTokens_TokenHash");

            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_ApiTokens_UserId");

            builder.HasIndex(x => new { x.UserId, x.IsActive })
                .HasDatabaseName("IX_ApiTokens_UserId_IsActive");
        }
    }
}