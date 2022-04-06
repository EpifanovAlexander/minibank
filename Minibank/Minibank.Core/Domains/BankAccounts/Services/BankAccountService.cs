using FluentValidation;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankTransferHistories;
using Minibank.Core.Domains.BankTransferHistories.Repositories;
using Minibank.Core.Domains.Currencies.Services;
using Minibank.Core.Domains.Users.Repositories;
using ValidationException = Minibank.Core.Exceptions.ValidationException;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IBankTransferHistoryRepository _bankTransferHistoryRepository;
        private readonly IValidator<CreateBankAccount> _createBankAccountValidator;
        private readonly IUnitOfWork _unitOfWork;

        public BankAccountService(IBankAccountRepository bankAccountRepository, IUserRepository userRepository,
            ICurrencyConverter currencyConverter, IBankTransferHistoryRepository bankTransferHistoryRepository,
            IValidator<CreateBankAccount> createBankAccountValidator, IUnitOfWork unitOfWork)
        {
            _bankAccountRepository = bankAccountRepository;
            _userRepository = userRepository;
            _currencyConverter = currencyConverter;
            _bankTransferHistoryRepository = bankTransferHistoryRepository;
            _createBankAccountValidator = createBankAccountValidator;
            _unitOfWork = unitOfWork;
        }


        public async Task<BankAccount> GetById(int accountId, CancellationToken cancellationToken)
        {
            var bankAccount = await _bankAccountRepository.GetById(accountId, cancellationToken);

            return bankAccount
                ?? throw new ValidationException($"Ошибка: Такого банковского счёта нет в БД. Id счёта: {accountId}");
        }


        public async Task<List<BankAccount>> GetUserBankAccounts(int userId, CancellationToken cancellationToken)
        {
            bool isUserExist = await _userRepository.Exists(userId, cancellationToken);
            if (!isUserExist)
            {
                throw new ValidationException($"Ошибка: Такого пользователя нет в БД. Id пользователя: {userId}");
            }

            return await _bankAccountRepository.GetUserAccounts(userId, cancellationToken);
        }


        public async Task Create(CreateBankAccount account, CancellationToken cancellationToken)
        {
            _createBankAccountValidator.ValidateAndThrow(account);

            await _bankAccountRepository.Create(account, cancellationToken);
            await _unitOfWork.SaveChanges();
        }


        public async Task DeleteById(int accountId, CancellationToken cancellationToken)
        {
            BankAccount? account = await _bankAccountRepository.GetById(accountId, cancellationToken);
            if (account == null)
            {
                throw new ValidationException($"Ошибка: Такого банковского счёта нет в БД. Id счёта: {accountId}");
            }

            if (account.Sum != 0)
            {
                throw new ValidationException("Ошибка: На данном банковском счёте ещё остались средства. Такой счёт закрыть нельзя");
            }

            await _bankAccountRepository.DeleteById(accountId, cancellationToken);
            await _unitOfWork.SaveChanges();
        }


        public async Task<double> GetCommission(double sum, int fromAccountId, int toAccountId, CancellationToken cancellationToken)
        {
            if (sum <= 0)
            {
                throw new ValidationException("Ошибка: Сумма перевода должна быть больше нуля");
            }

            var formAccount = await _bankAccountRepository.GetById(fromAccountId, cancellationToken);
            if (formAccount == null)
            {
                throw new ValidationException($"Ошибка: Банковского счёта отправителя нет в БД. Id счёта отправителя: {fromAccountId}");
            }
            if (!formAccount.IsActive)
            {
                throw new ValidationException($"Ошибка: Банковский счёт отправителя закрыт. Id счёта отправителя: {fromAccountId}");
            }

            var toAccount = await _bankAccountRepository.GetById(toAccountId, cancellationToken);
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


        public async Task TransferMoney(double sum, int fromAccountId, int toAccountId, CancellationToken cancellationToken)
        {
            if (sum <= 0)
            {
                throw new ValidationException("Ошибка: Сумма перевода должна быть больше нуля");
            }

            if (fromAccountId == toAccountId)
            {
                throw new ValidationException("Ошибка: указан один и тот же банковский счёт");
            }

            var fromAccount = await _bankAccountRepository.GetById(fromAccountId, cancellationToken);
            if (fromAccount == null)
            {
                throw new ValidationException($"Ошибка: Банковского счёта отправителя нет в БД. Id счёта отправителя: {fromAccountId}");
            }

            if (!fromAccount.IsActive)
            {
                throw new ValidationException($"Ошибка: Банковский счёт отправителя закрыт. Id счёта отправителя: {fromAccountId}");
            }


            var toAccount = await _bankAccountRepository.GetById(toAccountId, cancellationToken);
            if (toAccount == null)
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
            double sumTo = await _currencyConverter.Convert(sumWithComission, fromAccount.Currency, toAccount.Currency, cancellationToken);

            fromAccount.Sum = Math.Round(fromAccount.Sum - sum, 2);
            toAccount.Sum = Math.Round(toAccount.Sum + sumTo, 2);

            await _bankAccountRepository.Update(fromAccount, cancellationToken);
            await _bankAccountRepository.Update(toAccount, cancellationToken);

            await _bankTransferHistoryRepository.Add(new CreateBankTransferHistory(sum, fromAccountId, toAccountId), cancellationToken);
            await _unitOfWork.SaveChanges();
        }

    }
}
