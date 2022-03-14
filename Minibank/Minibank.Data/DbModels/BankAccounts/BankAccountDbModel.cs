using Minibank.Core.Domains.Currencies;

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
        public DateTime DateClosing { get; set; }
    }
}
