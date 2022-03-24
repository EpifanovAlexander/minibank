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
            return _bankAccountRepository.GetById(accountId)
                ?? throw new ValidationException($"Ошибка: Такого банковского счёта нет в БД. Id счёта: {accountId}");
        }


        public IEnumerable<BankAccount> GetUserBankAccounts(int userId)
        {
            bool isUserExist = _userRepository.Exists(userId);
            if (!isUserExist)
            {
                throw new ValidationException($"Ошибка: Такого пользователя нет в БД. Id пользователя: {userId}");
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

            bool isUserExist = _userRepository.Exists(account.UserId);
            if (!isUserExist)
            {
                throw new ValidationException($"Ошибка: Такого пользователя нет в БД. Id пользователя: {account.UserId}");
            }

            _bankAccountRepository.Create(account);
        }


        public void DeleteById(int accountId)
        {
            BankAccount? account = _bankAccountRepository.GetById(accountId);
            if (account == null)
            {
                throw new ValidationException($"Ошибка: Такого банковского счёта нет в БД. Id счёта: {accountId}");
            }

            if (account.Sum != 0)
            {
                throw new ValidationException("Ошибка: На данном банковском счёте ещё остались средства. Такой счёт закрыть нельзя");
            }

            _bankAccountRepository.DeleteById(accountId);
        }


        public double GetCommission(double sum, int fromAccountId, int toAccountId)
        {
            if (sum<=0)
            {
                throw new ValidationException("Ошибка: Сумма перевода должна быть больше нуля");
            }

            var formAccount = _bankAccountRepository.GetById(fromAccountId);
            if (formAccount==null)
            {
                throw new ValidationException($"Ошибка: Банковского счёта отправителя нет в БД. Id счёта отправителя: {fromAccountId}");
            }
            if (!formAccount.IsActive)
            {
                throw new ValidationException($"Ошибка: Банковский счёт отправителя закрыт. Id счёта отправителя: {fromAccountId}");
            }

            var toAccount = _bankAccountRepository.GetById(toAccountId);
            if (toAccount == null)
            {
                throw new ValidationException($"Ошибка: Банковского счёта получателя нет в БД. Id счёта получателя: {toAccountId}");
            }
            if (!toAccount.IsActive)
            {
                throw new ValidationException($"Ошибка: Банковский счёт получателя закрыт. Id счёта получателя: {toAccountId}");
            }

            return (formAccount.UserId == toAccount.UserId) ? 0 : Math.Round(sum * 0.02, 2);
        }


        private double GetCommission(double sum, BankAccount fromAccount, BankAccount toAccount)
        {
            return (fromAccount.UserId == toAccount.UserId) ? 0 : Math.Round(sum * 0.02, 2);
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

            var fromAccount = _bankAccountRepository.GetById(fromAccountId);
            if (fromAccount == null)
            {
                throw new ValidationException($"Ошибка: Банковского счёта отправителя нет в БД. Id счёта отправителя: {fromAccountId}");
            }

            if (!fromAccount.IsActive)
            {
                throw new ValidationException($"Ошибка: Банковский счёт отправителя закрыт. Id счёта отправителя: {fromAccountId}");
            }


            var toAccount = _bankAccountRepository.GetById(toAccountId);
            if (toAccount==null)
            {
                throw new ValidationException($"Ошибка: Банковского счёта получателяя нет в БД. Id счёта получателя: {toAccountId}");
            }

            if (!toAccount.IsActive)
            {
                throw new ValidationException($"Ошибка: Банковский счёт получателя закрыт. Id счёта получателя: {toAccountId}");
            }


            if (fromAccount.Sum < sum)
            {
                throw new ValidationException("Ошибка: Недостаточный баланс на счету отправителя");
            }

            double sumWithComission = sum - GetCommission(sum, fromAccount, toAccount);
            double sumTo = _currencyConverter.Convert(sumWithComission, fromAccount.Currency, toAccount.Currency);

            fromAccount.Sum = Math.Round(fromAccount.Sum - sum, 2);
            toAccount.Sum = Math.Round(toAccount.Sum + sumTo, 2);

            _bankAccountRepository.Update(fromAccount);
            _bankAccountRepository.Update(toAccount);

            _bankTransferHistoryRepository.Add(new CreateBankTransferHistory(sum, fromAccountId, toAccountId));
        }

    }
}
