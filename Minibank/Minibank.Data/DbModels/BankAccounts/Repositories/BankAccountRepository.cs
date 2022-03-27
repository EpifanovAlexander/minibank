using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Exceptions;
using Minibank.Data.DbModels.BankAccounts.Mappers;

namespace Minibank.Data.DbModels.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private static readonly List<BankAccountDbModel> _bankAccountStorage = new();

        public bool Exists(int id)
        {
            return _bankAccountStorage.Exists(account => account.Id == id);
        }


        public BankAccount? GetById(int accountId)
        {
            var bankAccount = _bankAccountStorage.FirstOrDefault(account => account.Id == accountId);
            if (bankAccount == null)
            {
                return null;               
            }
            return BankAccountMapper.ToModel(bankAccount);
        }


        public IEnumerable<BankAccount> GetUserAccounts(int userId)
        {
            return _bankAccountStorage
                .Where(account => account.UserId == userId && account.IsActive)
                .Select(account => BankAccountMapper.ToModel(account));
        }


        public void Create(CreateBankAccount account)
        {
            DateTime now = DateTime.Now;
            var bankAccountDbModel = new BankAccountDbModel
            {
                Id = (_bankAccountStorage.Count == 0) ? 0 : _bankAccountStorage.Max(a => a.Id) + 1,
                UserId = account.UserId,
                Sum = account.Sum,
                Currency = account.Currency,
                IsActive = true,
                DateOpening = now,
                DateClosing = new DateTime(now.Year+4, now.Month, now.Day, now.Hour, now.Minute, now.Second)
            };
            _bankAccountStorage.Add(bankAccountDbModel);
        }


        public void Update(BankAccount bankAccount)
        {
            var accountDb = _bankAccountStorage.FirstOrDefault(account => account.Id == bankAccount.Id);
            if (accountDb == null)
            {
                throw new ValidationException($"Банковский счёт не найден. Id счёта: {bankAccount.Id}");
            }

            int accountIndex = _bankAccountStorage.IndexOf(accountDb);
            _bankAccountStorage[accountIndex] = BankAccountMapper.ToDbModel(bankAccount);
        }


        public void DeleteById(int accountId)
        {
            var bankAccountDbModel = _bankAccountStorage.FirstOrDefault(account => account.Id == accountId);
            if (bankAccountDbModel == null)
            {
                throw new ValidationException($"Банковский счёт не найден. Id счёта: {accountId}");
            }

            bankAccountDbModel.IsActive = false;
        }


        public bool IsUserHaveAccounts(int userId)
        {
            return _bankAccountStorage.Any(account => account.UserId == userId && account.IsActive);
        }
    }
}
