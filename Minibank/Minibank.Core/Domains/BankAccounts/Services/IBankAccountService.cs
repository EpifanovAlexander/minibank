namespace Minibank.Core.Domains.BankAccounts.Services
{
    public interface IBankAccountService
    {
        Task<BankAccount> GetById(int accountId);
        IAsyncEnumerable<BankAccount> GetUserBankAccounts(int userId);
        Task Create(CreateBankAccount account);
        Task DeleteById(int accountId);
        Task<double> GetCommission(double sum, int fromAccountId, int toAccountId);
        Task TransferMoney(double sum, int fromAccountId, int toAccountId);
    }
}
