namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        BankAccount? GetById(int accountId);
        IEnumerable<BankAccount> GetUserAccounts(int userId);
        void Create(CreateBankAccount account);
        void Update(BankAccount account);
        void DeleteById(int accountId);
        bool Exists(int id);
        bool IsUserHaveAccounts(int userId);
    }
}
