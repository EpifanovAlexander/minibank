using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Exceptions;
using Minibank.Data.DbModels.BankAccounts.Mappers;

namespace Minibank.Data.DbModels.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private static List<BankAccountDbModel> _bankAccountStorage = new List<BankAccountDbModel>();

        public bool Exists(int id)
        {
            return _bankAccountStorage.Exists(account => account.Id == id);
        }


        public BankAccount GetById(int accountId)
        {
            if (Exists(accountId))
            {
                var bankAccount = _bankAccountStorage.FirstOrDefault(_account => _account.Id == accountId);
                return BankAccountMapper.ToModel(bankAccount);
            }
            return null;
        }


        public IEnumerable<BankAccount> GetUserAccounts(int userId)
        {
            return _bankAccountStorage
                .Where(_account => _account.UserId == userId && _account.IsActive)
                .Select(_account => BankAccountMapper.ToModel(_account));
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


        public void Update(BankAccount account)
        {
            var accountDb = _bankAccountStorage.FirstOrDefault(_account => _account.Id == account.Id);
            if (accountDb == null)
            {
                throw new ValidationException($"Банковский счёт не найден. Id: {account.Id}");
            }

            int accountIndex = _bankAccountStorage.IndexOf(accountDb);
            _bankAccountStorage[accountIndex] = BankAccountMapper.ToDbModel(account);
        }


        public void DeleteById(int accountId)
        {
            var bankAccountDbModel = _bankAccountStorage.FirstOrDefault(_account => _account.Id == accountId);
            if (bankAccountDbModel == null)
            {
                throw new ValidationException($"Банковский счёт не найден. Id: {accountId}");
            }

            bankAccountDbModel.IsActive = false;
        }


        public bool IsUserHaveAccounts(int userId)
        {
            return _bankAccountStorage
                .Where(_account => _account.UserId == userId && _account.IsActive)
                .Any();
        }
    }
}
