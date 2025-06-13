using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static DomainConstants.Database;
using static DomainConstants;

namespace FormFlow.Persistence.Configurations
{
    public class UserContactConfiguration : IEntityTypeConfiguration<UserContact>
    {
        public void Configure(EntityTypeBuilder<UserContact> builder)
        {
            builder.ToTable(TableNames.UserContacts);

            builder.HasKey(uc => uc.Id);
            builder.Property(uc => uc.Id)
                .HasColumnName(ColumnNames.UserContactId);

            builder.Property(uc => uc.UserId)
                .IsRequired()
                .HasColumnName(ColumnNames.UserId);

            builder.Property(uc => uc.Type)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName(ColumnNames.ContactType);

            builder.Property(uc => uc.Value)
                .IsRequired()
                .HasMaxLength(Validation.ContactValueMaxLength)
                .HasColumnName(ColumnNames.ContactValue);

            builder.Property(uc => uc.IsPrimary)
                .IsRequired()
                .HasColumnName(ColumnNames.IsPrimary)
                .HasDefaultValue(DefaultValues.IsPrimaryDefault);

            builder.Property(uc => uc.CreatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.CreatedAt);

            builder.Property(uc => uc.UpdatedAt)
                .IsRequired()
                .HasColumnName(ColumnNames.UpdatedAt);

            builder.HasIndex(uc => uc.UserId)
                .HasDatabaseName(IndexNames.UserContactsUserIndex);

            builder.HasIndex(uc => new { uc.UserId, uc.Type, uc.IsPrimary })
                .HasDatabaseName(IndexNames.UserContactsUserTypePrimaryIndex);

            builder.HasOne(uc => uc.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
