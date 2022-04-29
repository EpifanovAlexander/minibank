using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.BankAccounts.Services;
using Xunit;
using Moq;
using ValidationException = Minibank.Core.Exceptions.ValidationException;
using Minibank.Core.Domains.BankAccounts.Validators;
using Minibank.Core.Domains.Currencies.Services;
using Minibank.Core.Domains.BankTransferHistories.Repositories;
using System;
using Minibank.Core.Domains.Currencies;
using Minibank.Core.Domains.BankTransferHistories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Tests
{
    public class BankAccountServiceTests
    {
        private readonly IBankAccountService _bankAccountService;
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly Mock<IBankAccountRepository> _fakeBankAccountRepository;
        private readonly Mock<ICurrencyConverter> _fakeCurrencyConverter;
        private readonly Mock<IBankTransferHistoryRepository> _fakeBankTransferHistoryRepository;
        private readonly Mock<IUnitOfWork> _fakeUnitOfWork;

        private readonly CreateBankAccountValidator _createBankAccountValidator;

        public BankAccountServiceTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();
            _fakeCurrencyConverter = new Mock<ICurrencyConverter>();
            _fakeBankTransferHistoryRepository = new Mock<IBankTransferHistoryRepository>();
            _fakeUnitOfWork = new Mock<IUnitOfWork>();

            _createBankAccountValidator = new CreateBankAccountValidator(_fakeUserRepository.Object);

            _bankAccountService = new BankAccountService(_fakeBankAccountRepository.Object, _fakeUserRepository.Object,
                _fakeCurrencyConverter.Object, _fakeBankTransferHistoryRepository.Object, _createBankAccountValidator, _fakeUnitOfWork.Object);
        }


        [Fact]
        public async void GetBankAccountById_WithNonExistId_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int accountId = 1;

            _fakeBankAccountRepository
               .Setup(repository => repository.GetById(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetById(accountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Такого банковского счёта нет в БД. Id счёта: {accountId}", exception.Message);
        }


        [Fact]
        public void GetBankAccountById_WithExistId_BankAccountRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int accountId = 1;
            BankAccount account = new();

            _fakeBankAccountRepository
               .Setup(repository => repository.GetById(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(account);

            // ACT
            _bankAccountService.GetById(accountId, cancellationToken);

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.GetById(accountId, cancellationToken), Times.Once);
        }


        [Fact]
        public async void GetBankAccountById_WithExistId_ReturnBankAccount()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 10;
            int accountId = 1;
            int userId = 1;
            DateTime now = DateTime.Now;
            var expectedAccount = new BankAccount(accountId, userId, Currency.RUB, true, now, now, sum);

            _fakeBankAccountRepository
               .Setup(repository => repository.GetById(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(expectedAccount);

            // ACT
            var actualAccount = await _bankAccountService.GetById(accountId, cancellationToken);

            bool areAccountsEqual = expectedAccount.Id == actualAccount.Id
                && expectedAccount.UserId == actualAccount.UserId
                && expectedAccount.Sum == actualAccount.Sum
                && expectedAccount.Currency == actualAccount.Currency
                && expectedAccount.IsActive == actualAccount.IsActive
                && expectedAccount.DateOpening == actualAccount.DateOpening
                && expectedAccount.DateClosing == actualAccount.DateClosing;

            // ASSERT
            Assert.True(areAccountsEqual);
        }


        [Fact]
        public async void GetUserBankAccounts_WithNonExistUserId_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;

            _fakeUserRepository
               .Setup(repository => repository.Exists(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(false);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetUserBankAccounts(userId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Такого пользователя нет в БД. Id пользователя: {userId}", exception.Message);
        }


        [Fact]
        public void GetUserBankAccounts_WithExistUserId_BankAccountRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;

            _fakeUserRepository
               .Setup(repository => repository.Exists(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(true);

            // ACT
            var userAccounts = _bankAccountService.GetUserBankAccounts(userId, cancellationToken);

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.GetUserAccounts(userId, cancellationToken), Times.Once);
        }


        [Fact]
        public async void CreateBankAccount_WithNegativeSum_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;
            double sum = -10;
            var account = new CreateBankAccount(userId, Currency.RUB, sum);

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _bankAccountService.Create(account, cancellationToken));

            // ASSERT
            Assert.Contains("Ошибка: Нельзя добавить счёт с отрицательной суммой", exception.Message);
        }


        [Fact]
        public async void CreateBankAccount_WithInvalidCurrency_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;
            double sum = 10;
            var account = new CreateBankAccount(userId, null, sum);

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _bankAccountService.Create(account, cancellationToken));

            // ASSERT
            Assert.Contains("Ошибка: Нельзя создать счёт с такой валютой", exception.Message);
        }


        [Fact]
        public async void CreateBankAccount_WithNonExistUser_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;
            double sum = 10;
            var account = new CreateBankAccount(userId, Currency.RUB, sum);

            _fakeBankAccountRepository
               .Setup(repository => repository.Exists(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(false);

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _bankAccountService.Create(account, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Такого пользователя нет в БД. Id пользователя: {userId}", exception.Message);
        }


        [Fact]
        public async void CreateBankAccount_WithValidLoginAndEmailAndUser_BankAccountRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;
            double sum = 10;
            var account = new CreateBankAccount(userId, Currency.RUB, sum);

            _fakeUserRepository
              .Setup(repository => repository.Exists(It.IsAny<int>(), cancellationToken))
              .ReturnsAsync(true);

            // ACT
            await _bankAccountService.Create(account, cancellationToken);

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.Create(account, cancellationToken), Times.Once);
        }


        [Fact]
        public async void DeleteBankAccount_WithInvalidId_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int accountId = 1;

            _fakeBankAccountRepository
              .Setup(repository => repository.GetById(It.IsAny<int>(), cancellationToken))
              .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.DeleteById(accountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Такого банковского счёта нет в БД. Id счёта: {accountId}", exception.Message);
        }


        [Fact]
        public async void CreateBankAccount_WithNonZeroSum_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 10;
            int accountId = 1;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(accountId, userId, Currency.RUB, true, now, now, sum);

            _fakeBankAccountRepository
              .Setup(repository => repository.GetById(It.IsAny<int>(), cancellationToken))
              .ReturnsAsync(account);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.DeleteById(accountId, cancellationToken));

            // ASSERT
            Assert.Contains("Ошибка: На данном банковском счёте ещё остались средства. Такой счёт закрыть нельзя", exception.Message);
        }


        [Fact]
        public async void DeleteBankAccount_WithValidIdAndZeroSum_BankAccountRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 0;
            int accountId = 1;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(accountId, userId, Currency.RUB, true, now, now, sum);

            _fakeBankAccountRepository
              .Setup(repository => repository.GetById(It.IsAny<int>(), cancellationToken))
              .ReturnsAsync(account);

            // ACT
            await _bankAccountService.DeleteById(accountId, cancellationToken);

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.DeleteById(accountId, cancellationToken), Times.Once);
        }


        [Fact]
        public async void GetCommission_WithZeroSum_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 0;
            int fromAccountId = 1;
            int toAccountId = 2;

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains("Ошибка: Сумма перевода должна быть больше нуля", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithNonExistFromBankAccount_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Банковского счёта отправителя нет в БД. Id счёта отправителя: {fromAccountId}", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithNonActiveFromBankAccount_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(fromAccountId, userId, Currency.RUB, false, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(account);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Банковский счёт отправителя закрыт. Id счёта отправителя: {fromAccountId}", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithNonExistToBankAccount_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(account);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId, cancellationToken))
             .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Банковского счёта получателя нет в БД. Id счёта получателя: {toAccountId}", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithNonActiveToBankAccount_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, false, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId, cancellationToken))
             .ReturnsAsync(toAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Банковский счёт получателя закрыт. Id счёта получателя: {toAccountId}", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithSingleUserBankAccounts_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId, cancellationToken))
             .ReturnsAsync(toAccount);

            // ACT
            double result = await _bankAccountService.GetCommission(sum, fromAccountId, toAccountId, cancellationToken);

            // ASSERT
            Assert.Equal(0, result);
        }


        [Fact]
        public async void GetCommission_WithDifferentUserBankAccounts_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId+1, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId, cancellationToken))
             .ReturnsAsync(toAccount);

            // ACT
            double result = await _bankAccountService.GetCommission(sum, fromAccountId, toAccountId, cancellationToken);

            // ASSERT
            Assert.Equal(Math.Round(sum * 0.02, 2), result);
        }


        [Fact]
        public async void TransferMoney_WithZeroSum_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 0;
            int fromAccountId = 1;
            int toAccountId = 2;

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains("Ошибка: Сумма перевода должна быть больше нуля", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithSameBankAccountsId_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 1;

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains("Ошибка: указан один и тот же банковский счёт", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithNonExistFromBankAccount_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Банковского счёта отправителя нет в БД. Id счёта отправителя: {fromAccountId}", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithNonActiveFromBankAccount_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(fromAccountId, userId, Currency.RUB, false, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(account);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Банковский счёт отправителя закрыт. Id счёта отправителя: {fromAccountId}", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithNonExistToBankAccount_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(account);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId, cancellationToken))
             .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Банковского счёта получателяя нет в БД. Id счёта получателя: {toAccountId}", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithNonActiveToBankAccount_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, false, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId, cancellationToken))
             .ReturnsAsync(toAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains($"Ошибка: Банковский счёт получателя закрыт. Id счёта получателя: {toAccountId}", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithFromBankAccountSumLessThanTransferSum_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 5;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 1;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId, cancellationToken))
             .ReturnsAsync(toAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken));

            // ASSERT
            Assert.Contains("Ошибка: Недостаточный баланс на счету отправителя", exception.Message);
        }


        [Theory]
        [InlineData(1, 1000, 1, 1000, 100, 1, 900, 1100)]
        [InlineData(1, 1000, 2, 1000, 100, 1, 900, 1098)]
        [InlineData(1, 1000, 2, 1000, 100, 3, 900, 1294)]
        public async void GetCommission_WithValidUserBankAccounts_CheckFromBankAccountSumDecreasedAndToBankSumIncreased
            (int fromUserId, double fromUserSum, int toUserId, double toUserSum, double transferSum, double rate, 
            double expectedFromSum, double expectedToSum)
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = transferSum;
            int fromAccountId = 1;
            int toAccountId = 2;

            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, fromUserId, Currency.RUB, true, now, now, fromUserSum);
            var toAccount = new BankAccount(toAccountId, toUserId, Currency.RUB, true, now, now, toUserSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId, cancellationToken))
             .ReturnsAsync(toAccount);

            double sumWithComission = transferSum - (await _bankAccountService.GetCommission(transferSum, fromAccountId, toAccountId, cancellationToken));

            _fakeCurrencyConverter
             .Setup(converter => converter.Convert(It.IsAny<double>(), fromAccount.Currency, toAccount.Currency, cancellationToken))
             .ReturnsAsync(Math.Round(sumWithComission * rate, 2));

            // ACT
            await _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken);

            // ASSERT
            Assert.Equal(expectedFromSum, fromAccount.Sum);
            Assert.Equal(expectedToSum, toAccount.Sum);
        }


        [Fact]
        public async void GetCommission_WithValidUserBankAccounts_BankAccountRepositoryAndBankTransferHistoryRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId, cancellationToken))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId, cancellationToken))
             .ReturnsAsync(toAccount);

            _fakeCurrencyConverter
             .Setup(converter => converter.Convert(sum, fromAccount.Currency, toAccount.Currency, cancellationToken))
             .ReturnsAsync(1);

            // ACT
            await _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken);

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.Update(fromAccount, cancellationToken), Times.Once);
            _fakeBankAccountRepository.Verify(repository => repository.Update(toAccount, cancellationToken), Times.Once);
            _fakeBankTransferHistoryRepository.Verify(repository => repository.Add(It.IsAny<CreateBankTransferHistory>(), cancellationToken), Times.Once);
        }



    }
}
