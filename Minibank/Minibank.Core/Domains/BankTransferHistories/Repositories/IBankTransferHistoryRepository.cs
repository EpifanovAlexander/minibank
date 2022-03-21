namespace Minibank.Core.Domains.BankTransferHistories.Repositories
{
    public interface IBankTransferHistoryRepository
    {
        void Add(CreateBankTransferHistory history);
        IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId);
    }
}
