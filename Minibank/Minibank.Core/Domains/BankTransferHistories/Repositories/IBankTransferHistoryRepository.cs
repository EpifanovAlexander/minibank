namespace Minibank.Core.Domains.BankTransferHistories.Repositories
{
    public interface IBankTransferHistoryRepository
    {
        void AddBankTransferHistory(BankTransferHistory history);
        IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId);
    }
}
