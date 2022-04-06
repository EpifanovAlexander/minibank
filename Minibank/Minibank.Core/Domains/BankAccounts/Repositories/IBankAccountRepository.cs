namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount?> GetById(int accountId, CancellationToken cancellationToken);
        Task<List<BankAccount>> GetUserAccounts(int userId, CancellationToken cancellationToken);
        Task Create(CreateBankAccount account, CancellationToken cancellationToken);
        Task Update(BankAccount account, CancellationToken cancellationToken);
        Task DeleteById(int accountId, CancellationToken cancellationToken);
        Task<bool> Exists(int id, CancellationToken cancellationToken);
        Task<bool> IsUserHaveAccounts(int userId, CancellationToken cancellationToken);
    }
}
