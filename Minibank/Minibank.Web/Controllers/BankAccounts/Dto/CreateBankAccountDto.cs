using Minibank.Core.Domains.Currencies;

namespace Minibank.Web.Controllers.BankAccounts.Dto
{
    public class CreateBankAccountDto
    {
        public int UserId { get; set; }
        public double Sum { get; set; }
        public Currency Currency { get; set; }

        public CreateBankAccountDto(int userId, Currency currency, double sum = 0)
        {
            UserId = userId;
            Sum = sum;
            Currency = currency;
        }
    }
}
