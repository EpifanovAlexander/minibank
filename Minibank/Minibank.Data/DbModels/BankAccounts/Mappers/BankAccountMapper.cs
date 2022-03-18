using Minibank.Core.Domains.BankAccounts;

namespace Minibank.Data.DbModels.BankAccounts.Mappers
{
    public class BankAccountMapper
    {
        public static BankAccount ConvertToBankAccount(BankAccountDbModel bankAccount)
        {
            return new BankAccount(bankAccount.Id, bankAccount.UserId, bankAccount.Currency,
                bankAccount.IsActive, bankAccount.DateOpening, bankAccount.DateClosing, Math.Round(bankAccount.Sum, 2));
        }
    }
}
