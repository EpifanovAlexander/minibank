using Minibank.Core.Domains.BankAccounts;

namespace Minibank.Data.DbModels.BankAccounts.Mappers
{
    public class BankAccountMapper
    {
        public static BankAccount ToModel(BankAccountDbModel account)
        {
            return new BankAccount(account.Id, account.UserId, account.Currency,
                account.IsActive, account.DateOpening, account.DateClosing, account.Sum);
        }

        public static BankAccountDbModel ToDbModel(BankAccount account)
        {
            var bankAccountDbModel = new BankAccountDbModel
            {
                Id = account.Id,
                UserId = account.UserId,
                Sum = account.Sum,
                Currency = account.Currency,
                IsActive = account.IsActive,
                DateOpening = account.DateOpening,
                DateClosing = account.DateClosing
            };
            return bankAccountDbModel;
        }
    }
}
