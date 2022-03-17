using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;

namespace Minibank.Data.DbModels.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private static List<BankAccountDbModel> _bankAccountStorage = new List<BankAccountDbModel>();


        private BankAccount ConvertToBankAccount(BankAccountDbModel bankAccount)
        {
            return new BankAccount(bankAccount.Id, bankAccount.UserId, bankAccount.Currency, 
                bankAccount.IsActive, bankAccount.DateOpening, bankAccount.DateClosing, Math.Round(bankAccount.Sum,2));
        }


        public bool IsBankAccountExist(int id)
        {
            return _bankAccountStorage.Exists(account => account.Id == id);
        }


        public BankAccount GetById(int accountId)
        {
            if (IsBankAccountExist(accountId))
            {
                var bankAccount = _bankAccountStorage.FirstOrDefault(_account => _account.Id == accountId);
                return ConvertToBankAccount(bankAccount);
            }
            return null;
        }


        public IEnumerable<BankAccount> GetUserAccounts(int userId)
        {
            return _bankAccountStorage
                .Where(_account => _account.UserId == userId && _account.IsActive)
                .Select(_account => ConvertToBankAccount(_account));
        }


        public void CreateAccount(CreateBankAccount account)
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


        public bool DeleteById(int accountId)
        {
            if (IsBankAccountExist(accountId))
            {
                var bankAccountDbModel = _bankAccountStorage.FirstOrDefault(_account => _account.Id == accountId);
                bankAccountDbModel.IsActive = false;
                return true;
            }
            return false;
        }


        public bool CheckUsersOfAccounts(int fromAccountId, int toAccountId)
        {

            return _bankAccountStorage
                .Where(_account => _account.Id == fromAccountId &&
                _account.UserId == _bankAccountStorage.FirstOrDefault(_toAccount => _toAccount.Id == toAccountId).UserId)
                .Any();
        }


        public void TransferMoney(double sumFrom, double sumTo, int fromAccountId, int toAccountId)
        {
            _bankAccountStorage.FirstOrDefault(_account => _account.Id == fromAccountId).Sum -= sumFrom;
            _bankAccountStorage.FirstOrDefault(_account => _account.Id == toAccountId).Sum += sumTo;
        }


        public bool IsUserHaveAccounts(int userId)
        {
            return _bankAccountStorage
                .Where(_account => _account.UserId == userId && _account.IsActive)
                .Any();
        }
    }
}
