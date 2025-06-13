using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;

namespace FormFlow.Persistence.Configurations
{
    public class FormConfiguration : IEntityTypeConfiguration<Form>
    {
        public void Configure(EntityTypeBuilder<Form> builder)
        {
            builder.ToTable(TableNames.Forms);

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id)
                .HasColumnName(ColumnNames.FormId);

            builder.Property(f => f.TemplateId)
                .IsRequired()
                .HasColumnName(ColumnNames.TemplateId);

            builder.Property(f => f.UserId)
                .IsRequired()
                .HasColumnName(ColumnNames.UserId);

            builder.Property(f => f.AnswersData)
                .IsRequired()
                .HasColumnType(ColumnTypes.JsonColumnType)
                .HasColumnName(ColumnNames.AnswersData);

            builder.Property(f => f.SubmittedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.SubmittedAt);

            builder.Property(f => f.UpdatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.UpdatedAt);

            builder.Property(f => f.IsDeleted)
                .IsRequired()
                .HasColumnName(ColumnNames.IsDeleted)
                .HasDefaultValue(DefaultValues.IsDeletedDefault);

            builder.Property(f => f.TemplateVersion)
                .IsRequired()
                .HasColumnName(ColumnNames.TemplateVersion);

            builder.HasIndex(f => f.TemplateId)
                .HasDatabaseName(IndexNames.FormsTemplateIndex);

            builder.HasIndex(f => f.UserId)
                .HasDatabaseName(IndexNames.FormsUserIndex);

            builder.HasIndex(f => new { f.TemplateId, f.UserId })
                .IsUnique()
                .HasDatabaseName(IndexNames.FormsTemplateUserIndex);

            builder.HasOne(f => f.Template)
                .WithMany(t => t.Forms)
                .HasForeignKey(f => f.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.User)
                .WithMany(u => u.Forms)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
