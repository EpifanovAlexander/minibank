namespace Minibank.Core.Domains.BankTransferHistories.Repositories
{
    public interface IBankTransferHistoryRepository
    {
        void AddBankTransferHistory(CreateBankTransferHistory history);
        IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId);
    }
}
