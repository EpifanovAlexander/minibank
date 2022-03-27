using Minibank.Core.Domains.BankTransferHistories.Repositories;
using Minibank.Core.Domains.BankTransferHistories;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Minibank.Data.DbModels.BankTransferHistories.Repositories
{
    public class BankTransferHistoryRepository : IBankTransferHistoryRepository
    {
        private readonly MinibankContext _context;
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankTransferHistoryRepository(IBankAccountRepository bankAccountRepository, MinibankContext context)
        {
            _bankAccountRepository = bankAccountRepository;
            _context = context;
        }

        public async Task Add(CreateBankTransferHistory history)
        {
            var bankTransferHistoryDbModel = new BankTransferHistoryDbModel
            {
                Id = 0,
                Sum = history.Sum,
                FromAccountId = history.FromAccountId,
                ToAccountId = history.ToAccountId
            };

            await _context.BankTransferHistories.AddAsync(bankTransferHistoryDbModel);
        }

        public async IAsyncEnumerable<BankTransferHistory> GetUserTransferHistory(int userId)
        {
            var userAccounts = _bankAccountRepository.GetUserAccounts(userId);
            var userAccountsId = new List<int>();

            await foreach (var account in userAccounts)
            {
                userAccountsId.Add(account.Id);
            }


            var histories = _context.BankTransferHistories
                .Include(it => it.FromAccount)
                .Include(it => it.ToAccount)
                .Where(history => userAccountsId.Contains(history.FromAccountId))
                .Select(history => new BankTransferHistory(history.Id, history.Sum, history.FromAccountId, history.ToAccountId));

            foreach (var historiy in histories)
            {
                yield return historiy;
            }
        }

    }
}
