using Minibank.Core.Domains.Currencies;

namespace Minibank.Core.Domains.BankAccounts
{
    public class CreateBankAccount
    {
        public int UserId { get; set; }
        public double Sum { get; set; }
        public Currency Currency { get; set; }

        public CreateBankAccount(int userId, Currency currency, double sum = 0)
        {
            UserId = userId;
            Sum = sum;
            Currency = currency;
        }
    }
}