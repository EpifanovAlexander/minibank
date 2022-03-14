using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankTransferHistories;
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

        public BankAccountService(IBankAccountRepository bankAccountRepository, IUserRepository userRepository, ICurrencyConverter currencyConverter)
        {
            _bankAccountRepository = bankAccountRepository;
            _userRepository = userRepository;
            _currencyConverter = currencyConverter;
        }


        public BankAccount Get(int accountId)
        {
            return _bankAccountRepository.Get(accountId) ?? throw new ValidationException("Ошибка: Такого банковского счёта нет в БД");
        }


        public IEnumerable<BankAccount> GetUserBankAccounts(int userId)
        {
            if (_userRepository.Get(userId) == null)
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }
            return _bankAccountRepository.GetUserAccounts(userId);
        }


        public void Create(BankAccount account)
        {
            if (_userRepository.Get(account.UserId)==null)
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }
            if (account.Sum<0)
            {
                throw new ValidationException("Ошибка: Нельзя добавить счёт с отрицательной суммой");
            }
            if (Enum.GetName(typeof(Currency), account.Currency) == null)
            {
                throw new ValidationException("Ошибка: Нельзя создать счёт с такой валютой");
            }
            _bankAccountRepository.Create(account);
        }


        public void Delete(int accountId)
        {
            BankAccount account = _bankAccountRepository.Get(accountId);
            if (account == null)
            {
                throw new ValidationException("Ошибка: Такого банковского счёта нет в БД");
            }
            if (account.Sum != 0)
            {
                throw new ValidationException("Ошибка: На данном банковском счёте ещё остались средства. Такой счёт закрыть нельзя");
            }
            _bankAccountRepository.Delete(accountId);
        }


        public double GetCommission(double sum, int fromAccountId, int toAccountId)
        {
            Get(fromAccountId);
            Get(toAccountId);
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

            var fromAccount = Get(fromAccountId);
            if (fromAccount == null)
            {
                throw new ValidationException("Ошибка: Банковского счёта отправителя нет в БД");
            }
            if (!fromAccount.IsActive)
            {
                throw new ValidationException("Ошибка: Банковский счёт отправителя закрыт");
            }


            var toAccount = Get(toAccountId);
            if (toAccount == null)
            {
                throw new ValidationException("Ошибка: Банковского счёта получателяя нет в БД");
            }
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
        }

        public IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId)
        {
            if (_userRepository.Get(userId) == null)
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }
            return _bankAccountRepository.GetUserTransferHistory(userId);
        }
    }
}
