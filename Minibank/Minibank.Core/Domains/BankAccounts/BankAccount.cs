using Minibank.Core.Domains.Currencies;

namespace Minibank.Core.Domains.BankAccounts
{
    public class BankAccount
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Sum { get; set; }
        public Currency? Currency { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateOpening { get; set; }
        public DateTime? DateClosing { get; set; }

        public BankAccount(int id, int userId, Currency? currency, bool isActive, DateTime dateOpening, DateTime? dateClosing, double sum = 0)
        {
            Id = id;
            UserId = userId;
            Sum = sum;
            Currency = currency;
            IsActive = isActive;
            DateOpening = dateOpening;
            DateClosing = dateClosing;
        }
    }
}
