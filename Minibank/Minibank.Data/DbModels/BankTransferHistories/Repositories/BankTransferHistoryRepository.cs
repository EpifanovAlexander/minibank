using Minibank.Core.Domains.BankTransferHistories.Repositories;
using Minibank.Core.Domains.BankTransferHistories;
using Minibank.Core.Domains.BankAccounts.Repositories;

namespace Minibank.Data.DbModels.BankTransferHistories.Repositories
{
    public class BankTransferHistoryRepository : IBankTransferHistoryRepository
    {
        private static List<BankTransferHistoryDbModel> _transferHistoryStorage = new List<BankTransferHistoryDbModel>();
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankTransferHistoryRepository(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public void Add(CreateBankTransferHistory history)
        {
            var bankTransferHistoryDbModel = new BankTransferHistoryDbModel
            {
                Id = (_transferHistoryStorage.Count == 0) ? 0 : _transferHistoryStorage.Max(b => b.Id) + 1,
                Sum = history.Sum,
                FromAccountId = history.FromAccountId,
                ToAccountId = history.ToAccountId
            };
            _transferHistoryStorage.Add(bankTransferHistoryDbModel);
        }

        public IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId)
        {
            return _transferHistoryStorage
               .Where(history => _bankAccountRepository.GetUserAccounts(userId).Any(account => account.Id == history.FromAccountId))
               .Select(history => new BankTransferHistory(history.Id, history.Sum, history.FromAccountId, history.ToAccountId));
        }
    }
}
