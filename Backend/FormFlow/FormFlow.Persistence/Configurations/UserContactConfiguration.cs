using FormFlow.Domain.Models.General;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Persistence.Configurations
{
    public class UserContactConfiguration : IEntityTypeConfiguration<UserContact>
    {
        public void Configure(EntityTypeBuilder<UserContact> builder)
        {
            builder.ToTable(DomainConstants.Database.TableNames.UserContacts);

            builder.HasKey(uc => uc.Id);
            builder.Property(uc => uc.Id)
                .HasColumnName(DomainConstants.Database.ColumnNames.UserContactId);

            builder.Property(uc => uc.UserId)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UserId);

            builder.Property(uc => uc.Type)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnName(DomainConstants.Database.ColumnNames.ContactType);

            builder.Property(uc => uc.Value)
                .IsRequired()
                .HasMaxLength(DomainConstants.Validation.ContactValueMaxLength)
                .HasColumnName(DomainConstants.Database.ColumnNames.ContactValue);

            builder.Property(uc => uc.IsPrimary)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.IsPrimary)
                .HasDefaultValue(DomainConstants.Database.DefaultValues.IsPrimaryDefault);

            builder.Property(uc => uc.CreatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.CreatedAt);

            builder.Property(uc => uc.UpdatedAt)
                .IsRequired()
                .HasColumnName(DomainConstants.Database.ColumnNames.UpdatedAt);

            builder.HasIndex(uc => uc.UserId)
                .HasDatabaseName(DomainConstants.Database.IndexNames.UserContactsUserIndex);

            builder.HasIndex(uc => new { uc.UserId, uc.Type, uc.IsPrimary })
                .HasDatabaseName(DomainConstants.Database.IndexNames.UserContactsUserTypePrimaryIndex);

            builder.HasOne(uc => uc.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
