using Minibank.Core.Domains.BankTransferHistories;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public interface IBankAccountService
    {
        BankAccount Get(int accountId);
        IEnumerable<BankAccount> GetUserBankAccounts(int userId);
        void Create(BankAccount account);
        void Delete(int accountId);
        double GetCommission(double sum, int fromAccountId, int toAccountId);
        void TransferMoney(double sum, int fromAccountId, int toAccountId);
        IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId);
    }
}
