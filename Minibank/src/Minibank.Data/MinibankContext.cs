using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Minibank.Data.DbModels.BankAccounts;
using Minibank.Data.DbModels.BankTransferHistories;
using Minibank.Data.DbModels.Users;

namespace Minibank.Data
{
    public class MinibankContext : DbContext
    {
        public DbSet<UserDbModel> Users { get; set; }
        public DbSet<BankAccountDbModel> BankAccounts{ get; set; }
        public DbSet<BankTransferHistoryDbModel> BankTransferHistories { get; set; }

        public MinibankContext(DbContextOptions options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MinibankContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }
    }

}
