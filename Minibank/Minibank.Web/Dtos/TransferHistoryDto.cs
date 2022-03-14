namespace Minibank.Web.Dtos
{
    public class TransferHistoryDto
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }

        public TransferHistoryDto(int id, double sum, int fromAccountId, int toAccountId)
        {
            Id = id;
            Sum = sum;
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
        }
    }
}
