using Minibank.Core.Domains.BankTransferHistories;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public interface IBankAccountService
    {
        BankAccount GetById(int accountId);
        IEnumerable<BankAccount> GetUserBankAccounts(int userId);
        void Create(CreateBankAccount account);
        void DeleteById(int accountId);
        double GetCommission(double sum, int fromAccountId, int toAccountId);
        void TransferMoney(double sum, int fromAccountId, int toAccountId);
        IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId);
    }
}
