using Microsoft.EntityFrameworkCore;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Exceptions;
using Minibank.Data.DbModels.BankAccounts.Mappers;

namespace Minibank.Data.DbModels.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly MinibankContext _context;

        public BankAccountRepository(MinibankContext context)
        {
            _context = context;
        }


        public async Task<bool> Exists(int id)
        {
            return await _context.BankAccounts.AnyAsync(account => account.Id == id); 
        }


        public async Task<BankAccount?> GetById(int accountId)
        {
            var bankAccountDbModel = await _context.BankAccounts
                .Include(it => it.User)
                .Include(it => it.FromTransferHistories)
                .Include(it => it.ToTransferHistories)
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.Id == accountId);

            if (bankAccountDbModel == null)
            {
                return null;               
            }
            return BankAccountMapper.ToModel(bankAccountDbModel);
        }


        public async IAsyncEnumerable<BankAccount> GetUserAccounts(int userId)
        {
            var accounts = await _context.BankAccounts
                 .Where(account => account.UserId == userId && account.IsActive)
                 .Include(it => it.User)
                 .Include(it => it.FromTransferHistories)
                 .Include(it => it.ToTransferHistories)
                 .AsNoTracking()
                 .ToListAsync();

            foreach (var account in accounts)
            {
                yield return BankAccountMapper.ToModel(account);
            }
        }


        public async Task Create(CreateBankAccount account)
        {
            DateTime now = DateTime.Now;
            var bankAccountDbModel = new BankAccountDbModel
            {
                Id = 0,
                UserId = account.UserId,
                Sum = account.Sum,
                Currency = account.Currency,
                IsActive = true,
                DateOpening = now,
                DateClosing = new DateTime(now.Year + 4, now.Month, now.Day, now.Hour, now.Minute, now.Second)
            };

            await _context.BankAccounts.AddAsync(bankAccountDbModel);
        }


        public async Task Update(BankAccount bankAccount)
        {
            var bankAccountDbModel = await _context.BankAccounts.AsNoTracking()
                .FirstOrDefaultAsync(it => it.Id == bankAccount.Id);

            if (bankAccountDbModel == null)
            {
                throw new ValidationException($"Банковский счёт не найден. Id счёта: {bankAccount.Id}");
            }

            bankAccountDbModel = BankAccountMapper.ToDbModel(bankAccount);
            _context.Entry(bankAccountDbModel).State = EntityState.Modified;
        }


        public async Task DeleteById(int accountId)
        {
            var bankAccountDbModel = await _context.BankAccounts
                .FirstOrDefaultAsync(it => it.Id == accountId);

            if (bankAccountDbModel == null)
            {
                throw new ValidationException($"Банковский счёт не найден. Id счёта: {accountId}");
            }

            bankAccountDbModel.IsActive = false;
            _context.Entry(bankAccountDbModel).State = EntityState.Modified;
        }


        public async Task<bool> IsUserHaveAccounts(int userId)
        {
            return await _context.BankAccounts.AnyAsync(account => account.UserId == userId && account.IsActive);
        }
    }
}
