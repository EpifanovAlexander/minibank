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
            int accountId = 1;

            _fakeBankAccountRepository
               .Setup(repository => repository.GetById(It.IsAny<int>()))
               .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetById(accountId));

            // ASSERT
            Assert.Contains($"Ошибка: Такого банковского счёта нет в БД. Id счёта: {accountId}", exception.Message);
        }


        [Fact]
        public void GetBankAccountById_WithExistId_BankAccountRepositoryCalled()
        {
            // ARRANGE
            int accountId = 1;
            BankAccount account = new();

            _fakeBankAccountRepository
               .Setup(repository => repository.GetById(It.IsAny<int>()))
               .ReturnsAsync(account);

            // ACT
            _bankAccountService.GetById(accountId);

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.GetById(accountId), Times.Once);
        }


        [Fact]
        public async void GetBankAccountById_WithExistId_ReturnBankAccount()
        {
            // ARRANGE
            double sum = 10;
            int accountId = 1;
            int userId = 1;
            DateTime now = DateTime.Now;
            var expectedAccount = new BankAccount(accountId, userId, Currency.RUB, true, now, now, sum);

            _fakeBankAccountRepository
               .Setup(repository => repository.GetById(It.IsAny<int>()))
               .ReturnsAsync(expectedAccount);

            // ACT
            var actualAccount = await _bankAccountService.GetById(accountId);

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
        public void GetUserBankAccounts_WithNonExistUserId_ShouldThrowException()
        {
            // ARRANGE
            int userId = 1;

            _fakeUserRepository
               .Setup(repository => repository.Exists(It.IsAny<int>()))
               .ReturnsAsync(false);

            try
            {
                // ACT
                _bankAccountService.GetUserBankAccounts(userId);
            }
            catch (Exception exception)
            {
                // ASSERT
                Assert.Equal($"Ошибка: Такого пользователя нет в БД. Id пользователя: {userId}", exception.Message);
            }
        }


        [Fact]
        public async void GetUserBankAccounts_WithExistUserId_BankAccountRepositoryCalled()
        {
            // ARRANGE
            int userId = 1;

            async IAsyncEnumerable<BankAccount> accounts()
            {
                yield return new BankAccount();
            }

            _fakeUserRepository
               .Setup(repository => repository.Exists(It.IsAny<int>()))
               .ReturnsAsync(true);

            _fakeBankAccountRepository
               .Setup(repository => repository.GetUserAccounts(It.IsAny<int>()))
               .Returns(accounts);

            // ACT
            var userAccounts = _bankAccountService.GetUserBankAccounts(userId);
            await foreach (var account in userAccounts) { }

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.GetUserAccounts(userId), Times.Once);
        }


        [Fact]
        public async void CreateBankAccount_WithNegativeSum_ShouldThrowException()
        {
            // ARRANGE
            int userId = 1;
            double sum = -10;
            var account = new CreateBankAccount(userId, Currency.RUB, sum);

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _bankAccountService.Create(account));

            // ASSERT
            Assert.Contains("Ошибка: Нельзя добавить счёт с отрицательной суммой", exception.Message);
        }


        [Fact]
        public async void CreateBankAccount_WithInvalidCurrency_ShouldThrowException()
        {
            // ARRANGE
            int userId = 1;
            double sum = 10;
            var account = new CreateBankAccount(userId, null, sum);

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _bankAccountService.Create(account));

            // ASSERT
            Assert.Contains("Ошибка: Нельзя создать счёт с такой валютой", exception.Message);
        }


        [Fact]
        public async void CreateBankAccount_WithNonExistUser_ShouldThrowException()
        {
            // ARRANGE
            int userId = 1;
            double sum = 10;
            var account = new CreateBankAccount(userId, Currency.RUB, sum);

            _fakeBankAccountRepository
               .Setup(repository => repository.Exists(It.IsAny<int>()))
               .ReturnsAsync(false);

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _bankAccountService.Create(account));

            // ASSERT
            Assert.Contains($"Ошибка: Такого пользователя нет в БД. Id пользователя: {userId}", exception.Message);
        }


        [Fact]
        public async void CreateBankAccount_WithValidLoginAndEmailAndUser_BankAccountRepositoryCalled()
        {
            // ARRANGE
            int userId = 1;
            double sum = 10;
            var account = new CreateBankAccount(userId, Currency.RUB, sum);

            _fakeUserRepository
              .Setup(repository => repository.Exists(It.IsAny<int>()))
              .ReturnsAsync(true);

            // ACT
            await _bankAccountService.Create(account);

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.Create(account), Times.Once);
        }


        [Fact]
        public async void DeleteBankAccount_WithInvalidId_ShouldThrowException()
        {
            // ARRANGE
            int accountId = 1;

            _fakeBankAccountRepository
              .Setup(repository => repository.GetById(It.IsAny<int>()))
              .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.DeleteById(accountId));

            // ASSERT
            Assert.Contains($"Ошибка: Такого банковского счёта нет в БД. Id счёта: {accountId}", exception.Message);
        }


        [Fact]
        public async void CreateBankAccount_WithNonZeroSum_ShouldThrowException()
        {
            // ARRANGE
            double sum = 10;
            int accountId = 1;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(accountId, userId, Currency.RUB, true, now, now, sum);

            _fakeBankAccountRepository
              .Setup(repository => repository.GetById(It.IsAny<int>()))
              .ReturnsAsync(account);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.DeleteById(accountId));

            // ASSERT
            Assert.Contains("Ошибка: На данном банковском счёте ещё остались средства. Такой счёт закрыть нельзя", exception.Message);
        }


        [Fact]
        public async void DeleteBankAccount_WithValidIdAndZeroSum_BankAccountRepositoryCalled()
        {
            // ARRANGE
            double sum = 0;
            int accountId = 1;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(accountId, userId, Currency.RUB, true, now, now, sum);

            _fakeBankAccountRepository
              .Setup(repository => repository.GetById(It.IsAny<int>()))
              .ReturnsAsync(account);

            // ACT
            await _bankAccountService.DeleteById(accountId);

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.DeleteById(accountId), Times.Once);
        }


        [Fact]
        public async void GetCommission_WithZeroSum_ShouldThrowException()
        {
            // ARRANGE
            double sum = 0;
            int fromAccountId = 1;
            int toAccountId = 2;

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains("Ошибка: Сумма перевода должна быть больше нуля", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithNonExistFromBankAccount_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains($"Ошибка: Банковского счёта отправителя нет в БД. Id счёта отправителя: {fromAccountId}", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithNonActiveFromBankAccount_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(fromAccountId, userId, Currency.RUB, false, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(account);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains($"Ошибка: Банковский счёт отправителя закрыт. Id счёта отправителя: {fromAccountId}", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithNonExistToBankAccount_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(account);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId))
             .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains($"Ошибка: Банковского счёта получателя нет в БД. Id счёта получателя: {toAccountId}", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithNonActiveToBankAccount_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, false, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId))
             .ReturnsAsync(toAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains($"Ошибка: Банковский счёт получателя закрыт. Id счёта получателя: {toAccountId}", exception.Message);
        }


        [Fact]
        public async void GetCommission_WithSingleUserBankAccounts_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId))
             .ReturnsAsync(toAccount);

            // ACT
            double result = await _bankAccountService.GetCommission(sum, fromAccountId, toAccountId);

            // ASSERT
            Assert.Equal(0, result);
        }


        [Fact]
        public async void GetCommission_WithDifferentUserBankAccounts_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId+1, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId))
             .ReturnsAsync(toAccount);

            // ACT
            double result = await _bankAccountService.GetCommission(sum, fromAccountId, toAccountId);

            // ASSERT
            Assert.Equal(Math.Round(sum * 0.02, 2), result);
        }


        [Fact]
        public async void TransferMoney_WithZeroSum_ShouldThrowException()
        {
            // ARRANGE
            double sum = 0;
            int fromAccountId = 1;
            int toAccountId = 2;

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains("Ошибка: Сумма перевода должна быть больше нуля", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithSameBankAccountsId_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 1;

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains("Ошибка: указан один и тот же банковский счёт", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithNonExistFromBankAccount_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains($"Ошибка: Банковского счёта отправителя нет в БД. Id счёта отправителя: {fromAccountId}", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithNonActiveFromBankAccount_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(fromAccountId, userId, Currency.RUB, false, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(account);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains($"Ошибка: Банковский счёт отправителя закрыт. Id счёта отправителя: {fromAccountId}", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithNonExistToBankAccount_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var account = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(account);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId))
             .ReturnsAsync(null as BankAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains($"Ошибка: Банковского счёта получателяя нет в БД. Id счёта получателя: {toAccountId}", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithNonActiveToBankAccount_ShouldThrowException()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, false, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId))
             .ReturnsAsync(toAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId));

            // ASSERT
            Assert.Contains($"Ошибка: Банковский счёт получателя закрыт. Id счёта получателя: {toAccountId}", exception.Message);
        }


        [Fact]
        public async void TransferMoney_WithFromBankAccountSumLessThanTransferSum_ShouldThrowException()
        {
            // ARRANGE
            double sum = 5;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 1;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId))
             .ReturnsAsync(toAccount);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId));

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
            double sum = transferSum;
            int fromAccountId = 1;
            int toAccountId = 2;

            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, fromUserId, Currency.RUB, true, now, now, fromUserSum);
            var toAccount = new BankAccount(toAccountId, toUserId, Currency.RUB, true, now, now, toUserSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId))
             .ReturnsAsync(toAccount);

            double sumWithComission = transferSum - (await _bankAccountService.GetCommission(transferSum, fromAccountId, toAccountId));

            _fakeCurrencyConverter
             .Setup(converter => converter.Convert(It.IsAny<double>(), fromAccount.Currency, toAccount.Currency))
             .ReturnsAsync(Math.Round(sumWithComission * rate, 2));

            // ACT
            await _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId);

            // ASSERT
            Assert.Equal(expectedFromSum, fromAccount.Sum);
            Assert.Equal(expectedToSum, toAccount.Sum);
        }


        [Fact]
        public async void GetCommission_WithValidUserBankAccounts_BankAccountRepositoryAndBankTransferHistoryRepositoryCalled()
        {
            // ARRANGE
            double sum = 1;
            int fromAccountId = 1;
            int toAccountId = 2;

            double accountSum = 10;
            int userId = 1;
            DateTime now = DateTime.Now;
            var fromAccount = new BankAccount(fromAccountId, userId, Currency.RUB, true, now, now, accountSum);
            var toAccount = new BankAccount(toAccountId, userId, Currency.RUB, true, now, now, accountSum);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(fromAccountId))
             .ReturnsAsync(fromAccount);

            _fakeBankAccountRepository
             .Setup(repository => repository.GetById(toAccountId))
             .ReturnsAsync(toAccount);

            _fakeCurrencyConverter
             .Setup(converter => converter.Convert(sum, fromAccount.Currency, toAccount.Currency))
             .ReturnsAsync(1);

            // ACT
            await _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId);

            // ASSERT
            _fakeBankAccountRepository.Verify(repository => repository.Update(fromAccount), Times.Once);
            _fakeBankAccountRepository.Verify(repository => repository.Update(toAccount), Times.Once);
            _fakeBankTransferHistoryRepository.Verify(repository => repository.Add(It.IsAny<CreateBankTransferHistory>()), Times.Once);
        }



    }
}
