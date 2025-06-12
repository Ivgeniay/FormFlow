using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.Questions);

            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.QuestionId);

            builder.Property(q => q.TemplateId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.TemplateId);

            builder.Property(q => q.Order)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.QuestionOrder);

            builder.Property(q => q.ShowInResults)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.ShowInResults)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.ShowInResultsDefault);

            builder.Property(q => q.IsRequired)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsRequired)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsRequiredDefault);

            builder.Property(q => q.Data)
                .IsRequired()
                .HasColumnType(DomainConstants.Database.ColumnTypes.JsonColumnType)
                .HasColumnName(DomainConstants.Database.ColumnNames.QuestionData);

            builder.Property(q => q.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.Property(q => q.UpdatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UpdatedAt);

            builder.Property(q => q.IsDeleted)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsDeleted)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsDeletedDefault);

            builder.HasIndex(q => q.TemplateId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.QuestionsTemplateIndex);

            builder.HasIndex(q => new { q.TemplateId, q.Order })
                .HasDatabaseName(DomainConstants.Database.IndexNames.QuestionsTemplateOrderIndex);

            builder.HasOne(q => q.Template)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
