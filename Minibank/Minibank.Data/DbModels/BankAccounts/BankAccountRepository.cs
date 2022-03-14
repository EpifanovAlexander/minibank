using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankTransferHistories;

namespace Minibank.Data.DbModels.BankAccounts
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private static List<BankAccountDbModel> _bankAccountStorage = new List<BankAccountDbModel>();
        private static List<BankTransferHistoryDbModel> _transferHistoryStorage = new List<BankTransferHistoryDbModel>();

        private BankAccount ConvertToBankAccount(BankAccountDbModel bankAccount)
        {
            return new BankAccount(bankAccount.Id, bankAccount.UserId, bankAccount.Currency, 
                bankAccount.IsActive, bankAccount.DateOpening, bankAccount.DateClosing, bankAccount.Sum);
        }


        public BankAccount Get(int accountId)
        {
            var bankAccount = _bankAccountStorage.FirstOrDefault(_account => _account.Id == accountId);
            if (bankAccount == null)
            {
                return null;
            }
            return ConvertToBankAccount(bankAccount);
        }


        public IEnumerable<BankAccount> GetUserAccounts(int userId)
        {
            return _bankAccountStorage
                .Where(_account => _account.UserId == userId)
                .Where(_account => _account.IsActive)
                .Select(_account => ConvertToBankAccount(_account));
        }


        public void Create(BankAccount account)
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


        public void Delete(int accountId)
        {
            var bankAccountDbModel = _bankAccountStorage.FirstOrDefault(_account => _account.Id == accountId);
            if (bankAccountDbModel != null)
            {
                bankAccountDbModel.IsActive = false;
            }
        }


        public double GetCommission(double sum, int fromAccountId, int toAccountId)
        {
            if (_bankAccountStorage
                .Where(_account => _account.Id == fromAccountId &&
                _account.UserId == _bankAccountStorage.FirstOrDefault(_toAccount => _toAccount.Id == toAccountId).UserId)
                .Count() == 0)
            {
                return Math.Round(sum * 0.02, 2);
            }
            return 0;

            //if (Get(fromAccountId).UserId == Get(toAccountId).UserId)
            //{
            //    return 0;
            //}
        }


        public void TransferMoney(double sumFrom, double sumTo, int fromAccountId, int toAccountId)
        {
            _bankAccountStorage.FirstOrDefault(_account => _account.Id == fromAccountId).Sum -= Math.Round(sumFrom,2);
            _bankAccountStorage.FirstOrDefault(_account => _account.Id == toAccountId).Sum += Math.Round(sumTo,2);

            var bankTransferHistoryDbModel = new BankTransferHistoryDbModel
            {
                Id = (_transferHistoryStorage.Count == 0) ? 0 : _transferHistoryStorage.Max(b => b.Id) + 1,
                Sum = sumFrom,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId
            };
            _transferHistoryStorage.Add(bankTransferHistoryDbModel);
        }


        public IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId)
        {
            return _transferHistoryStorage
               .Where(history => GetUserAccounts(userId).Any(account => account.Id==history.FromAccountId))
               .Select(history => new BankTransferHistory(history.Id, history.Sum, history.FromAccountId, history.ToAccountId));
        }
    }
}
