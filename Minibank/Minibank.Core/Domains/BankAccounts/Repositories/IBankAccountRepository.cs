using Minibank.Core.Domains.BankTransferHistories;

namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        BankAccount Get(int accountId);
        IEnumerable<BankAccount> GetUserAccounts(int userId);
        void Create(BankAccount account);
        void Delete(int accountId);
        double GetCommission(double sum, int fromAccountId, int toAccountId);
        void TransferMoney(double sumFrom, double sumTo, int fromAccountId, int toAccountId);
        IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId);
    }
}
