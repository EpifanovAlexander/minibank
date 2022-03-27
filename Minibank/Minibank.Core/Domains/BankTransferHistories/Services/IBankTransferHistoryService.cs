namespace Minibank.Core.Domains.BankTransferHistories.Services
{
    public interface IBankTransferHistoryService
    {
        IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId);
    }
}
