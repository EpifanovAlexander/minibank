namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        BankAccount Get(int accountId);
        IEnumerable<BankAccount> GetUserAccounts(int userId);
        void Create(BankAccount account);
        bool Delete(int accountId);
        double GetCommission(double sum, int fromAccountId, int toAccountId);
        void TransferMoney(double sumFrom, double sumTo, int fromAccountId, int toAccountId);
        bool IsBankAccountExist(int id);
    }
}
