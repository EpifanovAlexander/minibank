namespace Minibank.Core.Domains.BankAccounts.Services
{
    public interface IBankAccountService
    {
        Task<BankAccount> GetById(int accountId, CancellationToken cancellationToken);
        Task<List<BankAccount>> GetUserBankAccounts(int userId, CancellationToken cancellationToken);
        Task Create(CreateBankAccount account, CancellationToken cancellationToken);
        Task DeleteById(int accountId, CancellationToken cancellationToken);
        Task<double> GetCommission(double sum, int fromAccountId, int toAccountId, CancellationToken cancellationToken);
        Task TransferMoney(double sum, int fromAccountId, int toAccountId, CancellationToken cancellationToken);
    }
}
