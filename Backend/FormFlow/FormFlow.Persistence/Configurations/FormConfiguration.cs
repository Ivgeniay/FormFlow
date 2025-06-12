using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class FormConfiguration : IEntityTypeConfiguration<Form>
    {
        public void Configure(EntityTypeBuilder<Form> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.Forms);

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.FormId);

            builder.Property(f => f.TemplateId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateId);

            builder.Property(f => f.UserId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UserId);

            builder.Property(f => f.AnswersData)
                .IsRequired()
                .HasColumnType(DomainConstants.Database.ColumnTypes.JsonColumnType)
                .HasColumnName(DomainConstants.Database.ColumnNames.AnswersData);

            builder.Property(f => f.SubmittedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.SubmittedAt);

            builder.Property(f => f.UpdatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UpdatedAt);

            builder.Property(f => f.IsDeleted)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsDeleted)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsDeletedDefault);

            builder.Property(f => f.TemplateVersion)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateVersion);

            builder.HasIndex(f => f.TemplateId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.FormsTemplateIndex);

            builder.HasIndex(f => f.UserId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.FormsUserIndex);

            builder.HasIndex(f => new { f.TemplateId, f.UserId })
                .IsUnique()
                .HasDatabaseName(DomainConstants.Database.IndexNames.FormsTemplateUserIndex);

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
