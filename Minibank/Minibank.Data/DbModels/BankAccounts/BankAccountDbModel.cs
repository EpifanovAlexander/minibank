using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minibank.Core.Domains.Currencies;
using Minibank.Data.DbModels.BankTransferHistories;
using Minibank.Data.DbModels.Users;

namespace Minibank.Data.DbModels.BankAccounts
{
    public class BankAccountDbModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Sum { get; set; }
        public Currency Currency { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateOpening { get; set; }
        public DateTime? DateClosing { get; set; }
        public virtual UserDbModel User { get; set; }
        public virtual List<BankTransferHistoryDbModel> FromTransferHistories { get; set; }
        public virtual List<BankTransferHistoryDbModel> ToTransferHistories { get; set; }


        internal class Map : IEntityTypeConfiguration<BankAccountDbModel>
        {
            public void Configure(EntityTypeBuilder<BankAccountDbModel> builder)
            {
                builder.ToTable("bank_accounts");

                builder.Property(it => it.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                builder.Property(it => it.UserId)
                    .HasColumnName("user_id");

                builder.HasOne(it => it.User)
                    .WithMany(it => it.BankAccounts)
                    .HasForeignKey(it => it.UserId);

                builder.Property(it => it.Sum)
                    .HasColumnName("sum");

                builder.Property(it => it.Currency)
                    .HasColumnName("currency");

                builder.Property(it => it.IsActive)
                    .HasColumnName("is_active");

                builder.Property(it => it.DateOpening)
                    .HasColumnName("date_opening");

                builder.Property(it => it.DateClosing)
                    .HasColumnName("date_closing");

                builder.HasKey(it => it.Id).HasName("pk_bank_account_id");
            }
        }
    }
}
