namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount?> GetById(int accountId);
        IAsyncEnumerable<BankAccount> GetUserAccounts(int userId);
        Task Create(CreateBankAccount account);
        Task Update(BankAccount account);
        Task DeleteById(int accountId);
        Task<bool> Exists(int id);
        Task<bool> IsUserHaveAccounts(int userId);
    }
}
