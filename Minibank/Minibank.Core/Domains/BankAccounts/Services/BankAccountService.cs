using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankTransferHistories;
using Minibank.Core.Domains.BankTransferHistories.Repositories;
using Minibank.Core.Domains.Currencies;
using Minibank.Core.Domains.Currencies.Services;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IBankTransferHistoryRepository _bankTransferHistoryRepository;

        public BankAccountService(IBankAccountRepository bankAccountRepository, IUserRepository userRepository, 
            ICurrencyConverter currencyConverter, IBankTransferHistoryRepository bankTransferHistoryRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _userRepository = userRepository;
            _currencyConverter = currencyConverter;
            _bankTransferHistoryRepository = bankTransferHistoryRepository;
        }


        public BankAccount Get(int accountId)
        {
            return _bankAccountRepository.Get(accountId) ?? throw new ValidationException("Ошибка: Такого банковского счёта нет в БД");
        }


        public IEnumerable<BankAccount> GetUserBankAccounts(int userId)
        {
            if (!_userRepository.IsUserExist(userId))
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }
            return _bankAccountRepository.GetUserAccounts(userId);
        }


        public void Create(BankAccount account)
        {
            if (account.Sum<0)
            {
                throw new ValidationException("Ошибка: Нельзя добавить счёт с отрицательной суммой");
            }
            if (Enum.GetName(typeof(Currency), account.Currency) == null)
            {
                throw new ValidationException("Ошибка: Нельзя создать счёт с такой валютой");
            }
            if (!_userRepository.IsUserExist(account.UserId))
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }
            _bankAccountRepository.Create(account);
        }


        public void Delete(int accountId)
        {
            if (!_bankAccountRepository.IsBankAccountExist(accountId))
            {
                throw new ValidationException("Ошибка: Такого банковского счёта нет в БД");
            }
            BankAccount account = _bankAccountRepository.Get(accountId);
            if (account.Sum != 0)
            {
                throw new ValidationException("Ошибка: На данном банковском счёте ещё остались средства. Такой счёт закрыть нельзя");
            }
            if (!_bankAccountRepository.Delete(accountId))
            {
                throw new ValidationException("Ошибка: Банковский счёт не удалился");
            }
        }


        public double GetCommission(double sum, int fromAccountId, int toAccountId)
        {
            if (sum<=0)
            {
                throw new ValidationException("Ошибка: Сумма перевода должна быть больше нуля");
            }
            if (!_bankAccountRepository.IsBankAccountExist(fromAccountId))
            {
                throw new ValidationException("Ошибка: Банковского счёта отправителя нет в БД");
            }
            if (!_bankAccountRepository.IsBankAccountExist(toAccountId))
            {
                throw new ValidationException("Ошибка: Банковского счёта получателя нет в БД");
            }
            return _bankAccountRepository.GetCommission(sum, fromAccountId, toAccountId);
        }


        public void TransferMoney(double sum, int fromAccountId, int toAccountId)
        {
            if (sum<=0)
            {
                throw new ValidationException("Ошибка: Сумма перевода должна быть больше нуля");
            }

            if (fromAccountId == toAccountId)
            {
                throw new ValidationException("Ошибка: указан один и тот же банковский счёт");
            }

            
            if (!_bankAccountRepository.IsBankAccountExist(fromAccountId))
            {
                throw new ValidationException("Ошибка: Банковского счёта отправителя нет в БД");
            }
            var fromAccount = Get(fromAccountId);
            if (!fromAccount.IsActive)
            {
                throw new ValidationException("Ошибка: Банковский счёт отправителя закрыт");
            }

         
            if (!_bankAccountRepository.IsBankAccountExist(toAccountId))
            {
                throw new ValidationException("Ошибка: Банковского счёта получателяя нет в БД");
            }
            var toAccount = Get(toAccountId);
            if (!toAccount.IsActive)
            {
                throw new ValidationException("Ошибка: Банковский счёт получателя закрыт");
            }


            double sumFrom = sum + GetCommission(sum, fromAccountId, toAccountId);
            if (fromAccount.Sum < sumFrom)
            {
                throw new ValidationException("Ошибка: Недостаточный баланс на счету отправителя");
            }

            double sumTo = _currencyConverter.Convert(sum, fromAccount.Currency, toAccount.Currency);
            _bankAccountRepository.TransferMoney(sumFrom, sumTo, fromAccountId, toAccountId);
            _bankTransferHistoryRepository.AddBankTransferHistory(new BankTransferHistory(-1, sumFrom, fromAccountId, toAccountId));
        }

        public IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId)
        {
            if (!_userRepository.IsUserExist(userId))
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }
            return _bankTransferHistoryRepository.GetUserTransferHistory(userId);
        }
    }
}
