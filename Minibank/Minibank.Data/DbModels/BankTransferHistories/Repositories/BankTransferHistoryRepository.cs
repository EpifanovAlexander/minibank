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

        public async Task Add(CreateBankTransferHistory history, CancellationToken cancellationToken)
        {
            var bankTransferHistoryDbModel = new BankTransferHistoryDbModel
            {
                Id = 0,
                Sum = history.Sum,
                FromAccountId = history.FromAccountId,
                ToAccountId = history.ToAccountId
            };

            await _context.BankTransferHistories.AddAsync(bankTransferHistoryDbModel, cancellationToken);
        }

        public async Task<List<BankTransferHistory>> GetUserTransferHistory(int userId, CancellationToken cancellationToken)
        {
            var userAccounts = await _bankAccountRepository.GetUserAccounts(userId, cancellationToken);
            var userAccountsId = new List<int>();

            foreach (var account in userAccounts)
            {
                userAccountsId.Add(account.Id);
            }

            return await _context.BankTransferHistories
                .Include(it => it.FromAccount)
                .Include(it => it.ToAccount)
                .Where(history => userAccountsId.Contains(history.FromAccountId))
                .Select(history => new BankTransferHistory(history.Id, history.Sum, history.FromAccountId, history.ToAccountId))
                .ToListAsync(cancellationToken);
        }

    }
}
