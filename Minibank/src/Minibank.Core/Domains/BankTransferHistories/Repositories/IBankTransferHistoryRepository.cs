namespace Minibank.Core.Domains.BankTransferHistories.Repositories
{
    public interface IBankTransferHistoryRepository
    {
        Task Add(CreateBankTransferHistory history, CancellationToken cancellationToken);
        Task<List<BankTransferHistory>> GetUserTransferHistory(int userId, CancellationToken cancellationToken);
    }
}
