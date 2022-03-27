namespace Minibank.Core.Domains.BankTransferHistories.Repositories
{
    public interface IBankTransferHistoryRepository
    {
        Task Add(CreateBankTransferHistory history);
        IAsyncEnumerable<BankTransferHistory> GetUserTransferHistory(int userId);
    }
}
