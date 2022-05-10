using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minibank.Data.DbModels.BankAccounts;

namespace Minibank.Data.DbModels.Users
{
    public class UserDbModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public virtual List<BankAccountDbModel> BankAccounts { get; set; }


        internal class Map : IEntityTypeConfiguration<UserDbModel>
        {
            public void Configure(EntityTypeBuilder<UserDbModel> builder)
            {
                builder.ToTable("user_table");

                builder.Property(it => it.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                builder.Property(it => it.Login)
                    .HasColumnName("login");

                builder.Property(it => it.Email)
                    .HasColumnName("email");

                builder.HasKey(it => it.Id).HasName("pk_user_id");
            }
        }
    }
}
