using Minibank.Core.Domains.Currencies;

namespace Minibank.Web.Controllers.BankAccounts.Dto
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Sum { get; set; }
        public Currency? Currency { get; set; }

        public BankAccountDto(int id, int userId, Currency? currency, double sum = 0)
        {
            Id = id;
            UserId = userId;
            Sum = sum;
            Currency = currency;
        }
    }
}
