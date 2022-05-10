using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minibank.Data.DbModels.BankAccounts;


namespace Minibank.Data.DbModels.BankTransferHistories
{
    public class BankTransferHistoryDbModel
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public virtual BankAccountDbModel FromAccount { get; set; }
        public virtual BankAccountDbModel ToAccount { get; set; }


        internal class Map : IEntityTypeConfiguration<BankTransferHistoryDbModel>
        {
            public void Configure(EntityTypeBuilder<BankTransferHistoryDbModel> builder)
            {
                builder.ToTable("transfer_history");

                builder.Property(it => it.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                builder.Property(it => it.Sum)
                    .HasColumnName("sum");

                builder.Property(it => it.FromAccountId)
                    .HasColumnName("from_id");

                builder.Property(it => it.ToAccountId)
                    .HasColumnName("to_id");

                builder.HasOne(it => it.FromAccount)
                    .WithMany(it => it.FromTransferHistories)
                    .HasForeignKey(it => it.FromAccountId);

                builder.HasOne(it => it.ToAccount)
                    .WithMany(it => it.ToTransferHistories)
                    .HasForeignKey(it => it.ToAccountId);

                builder.HasKey(it => it.Id).HasName("pk_transfer_history_id");
            }
        }
    }
}
