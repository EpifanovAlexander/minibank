namespace Minibank.Data.DbModels.BankTransferHistories
{
    public class BankTransferHistoryDbModel
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
    }
}
