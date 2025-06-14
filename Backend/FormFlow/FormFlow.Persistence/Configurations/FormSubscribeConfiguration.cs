using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;

namespace FormFlow.Persistence.Configurations
{
    public class FormSubscribeConfiguration : IEntityTypeConfiguration<FormSubscribe>
    {
        public void Configure(EntityTypeBuilder<FormSubscribe> builder)
        {
            builder.ToTable("FormSubscribes");

            builder.HasKey(fs => fs.Id);
            builder.Property(fs => fs.Id)
                .HasColumnName("form_subscribe_id");

            builder.Property(fs => fs.UserId)
                .IsRequired()
                .HasColumnName(ColumnNames.UserId);

            builder.Property(fs => fs.TemplateId)
                .IsRequired()
                .HasColumnName(ColumnNames.TemplateId);

            builder.Property(fs => fs.CreatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.CreatedAt);

            builder.HasIndex(fs => new { fs.UserId, fs.TemplateId })
                .IsUnique()
                .HasDatabaseName("idx_form_subscribes_user_template");

            builder.HasIndex(fs => fs.UserId)
                .HasDatabaseName("idx_form_subscribes_user");

            builder.HasIndex(fs => fs.TemplateId)
                .HasDatabaseName("idx_form_subscribes_template");

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(fs => fs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Template>()
                .WithMany()
                .HasForeignKey(fs => fs.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
