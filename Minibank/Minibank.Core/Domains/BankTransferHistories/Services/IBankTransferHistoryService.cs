namespace Minibank.Core.Domains.BankTransferHistories.Services
{
    public interface IBankTransferHistoryService
    {
        IAsyncEnumerable<BankTransferHistory> GetUserTransferHistory(int userId);
    }
}
