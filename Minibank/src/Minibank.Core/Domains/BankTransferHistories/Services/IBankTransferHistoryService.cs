namespace Minibank.Core.Domains.BankTransferHistories.Services
{
    public interface IBankTransferHistoryService
    {
        Task<List<BankTransferHistory>> GetUserTransferHistory(int userId, CancellationToken cancellationToken);
    }
}
