using Minibank.Core.Domains.Currencies;

namespace Minibank.Core.Domains.BankTransferHistories
{
    public class BankTransferHistory
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }

        public BankTransferHistory(int id, double sum, int fromAccountId, int toAccountId)
        {
            Id = id;
            Sum = sum;
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
        }
    }
}
