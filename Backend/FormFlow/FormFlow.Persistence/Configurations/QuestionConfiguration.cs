using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;

namespace FormFlow.Persistence.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable(TableNames.Questions);

            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id)
                .HasColumnName(ColumnNames.QuestionId);

            builder.Property(q => q.TemplateId)
                .IsRequired()
                .HasColumnName(ColumnNames.TemplateId);

            builder.Property(q => q.Order)
                .IsRequired()
                .HasColumnName(ColumnNames.QuestionOrder);

            builder.Property(q => q.ShowInResults)
                .IsRequired()
                .HasColumnName(ColumnNames.ShowInResults)
                .HasDefaultValue(DefaultValues.ShowInResultsDefault);

            builder.Property(q => q.IsRequired)
                .IsRequired()
                .HasColumnName(ColumnNames.IsRequired)
                .HasDefaultValue(DefaultValues.IsRequiredDefault);

            builder.Property(q => q.Data)
                .IsRequired()
                .HasColumnType(ColumnTypes.JsonColumnType)
                .HasColumnName(ColumnNames.QuestionData);

            builder.Property(q => q.CreatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.CreatedAt);

            builder.Property(q => q.UpdatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.UpdatedAt);

            builder.Property(q => q.IsDeleted)
                .IsRequired()
                .HasColumnName(ColumnNames.IsDeleted)
                .HasDefaultValue(DefaultValues.IsDeletedDefault);

            builder.HasIndex(q => q.TemplateId)
                .HasDatabaseName(IndexNames.QuestionsTemplateIndex);

            builder.HasIndex(q => new { q.TemplateId, q.Order })
                .HasDatabaseName(IndexNames.QuestionsTemplateOrderIndex);

            builder.HasOne(q => q.Template)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
