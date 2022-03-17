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


        public BankAccount GetById(int accountId)
        {
            return _bankAccountRepository.GetById(accountId) ?? throw new ValidationException("Ошибка: Такого банковского счёта нет в БД");
        }


        public IEnumerable<BankAccount> GetUserBankAccounts(int userId)
        {
            bool isUserExist = _userRepository.IsUserExist(userId);
            if (!isUserExist)
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }

            return _bankAccountRepository.GetUserAccounts(userId);
        }


        public void Create(CreateBankAccount account)
        {
            if (account.Sum<0)
            {
                throw new ValidationException("Ошибка: Нельзя добавить счёт с отрицательной суммой");
            }

            if (Enum.GetName(typeof(Currency), account.Currency) == null)
            {
                throw new ValidationException("Ошибка: Нельзя создать счёт с такой валютой");
            }

            bool isUserExist = _userRepository.IsUserExist(account.UserId);
            if (!isUserExist)
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }

            _bankAccountRepository.CreateAccount(account);
        }


        public void DeleteById(int accountId)
        {
            bool isBankAccountExist = _bankAccountRepository.IsBankAccountExist(accountId);
            if (!isBankAccountExist)
            {
                throw new ValidationException("Ошибка: Такого банковского счёта нет в БД");
            }

            BankAccount account = _bankAccountRepository.GetById(accountId);
            if (account.Sum != 0)
            {
                throw new ValidationException("Ошибка: На данном банковском счёте ещё остались средства. Такой счёт закрыть нельзя");
            }

            bool isAccountDeleted = _bankAccountRepository.DeleteById(accountId);
            if (!isAccountDeleted)
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

            bool isBankAccountExist = _bankAccountRepository.IsBankAccountExist(fromAccountId);
            if (!isBankAccountExist)
            {
                throw new ValidationException("Ошибка: Банковского счёта отправителя нет в БД");
            }

            isBankAccountExist = _bankAccountRepository.IsBankAccountExist(toAccountId);
            if (!isBankAccountExist)
            {
                throw new ValidationException("Ошибка: Банковского счёта получателя нет в БД");
            }

            bool areUsersSame = _bankAccountRepository.CheckUsersOfAccounts(fromAccountId, toAccountId);

            return areUsersSame ? 0 : Math.Round(sum * 0.02, 2);
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

            var fromAccount = GetById(fromAccountId);
            if (fromAccount == null)
            {
                throw new ValidationException("Ошибка: Банковского счёта отправителя нет в БД");
            }

            if (!fromAccount.IsActive)
            {
                throw new ValidationException("Ошибка: Банковский счёт отправителя закрыт");
            }


            var toAccount = GetById(toAccountId);
            if (toAccount==null)
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

            _bankTransferHistoryRepository.AddBankTransferHistory(new CreateBankTransferHistory(sumFrom, fromAccountId, toAccountId));
        }

        public IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId)
        {
            bool isUserExist = _userRepository.IsUserExist(userId);
            if (!isUserExist)
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }

            return _bankTransferHistoryRepository.GetUserTransferHistory(userId);
        }
    }
}
