namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        BankAccount GetById(int accountId);
        IEnumerable<BankAccount> GetUserAccounts(int userId);
        void CreateAccount(CreateBankAccount account);
        void DeleteById(int accountId);
        bool CheckUsersOfAccounts(int fromAccountId, int toAccountId);
        void TransferMoney(double sumFrom, double sumTo, int fromAccountId, int toAccountId);
        bool IsBankAccountExist(int id);
        bool IsUserHaveAccounts(int userId);
    }
}
