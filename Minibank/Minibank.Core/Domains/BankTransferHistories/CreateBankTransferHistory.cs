namespace Minibank.Core.Domains.BankTransferHistories
{
    public class CreateBankTransferHistory
    {
        public double Sum { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }

        public CreateBankTransferHistory(double sum, int fromAccountId, int toAccountId)
        {
            Sum = sum;
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
        }
    }
}
