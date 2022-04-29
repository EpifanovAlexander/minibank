using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;
using Minibank.Core.Domains.Users.Validators;
using Xunit;
using Moq;
using ValidationException = Minibank.Core.Exceptions.ValidationException;
using System.Threading;

namespace Minibank.Core.Tests
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly Mock<IBankAccountRepository> _fakeBankAccountRepository;
        private readonly Mock<IUnitOfWork> _fakeUnitOfWork;

        private readonly UserValidator _userValidator = new();
        private readonly CreateUserValidator _createUserValidator = new();


        public UserServiceTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();
            _fakeUnitOfWork = new Mock<IUnitOfWork>();

            _userService = new UserService(_fakeBankAccountRepository.Object, _fakeUserRepository.Object,
                _userValidator, _createUserValidator, _fakeUnitOfWork.Object);
        }


        [Fact]
        public void GetAllUsers_UserRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;

            // ACT
            _userService.GetAll(cancellationToken);

            // ASSERT
            _fakeUserRepository.Verify(repository => repository.GetAll(cancellationToken), Times.Once);
        }


        [Fact]
        public async void GetUserById_WithNonExistId_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;
            User user = null;

            _fakeUserRepository
               .Setup(repository => repository.GetById(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(user);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.GetById(userId, cancellationToken));

            // ASSERT
            Assert.Contains($"Такого пользователя нет в БД. Id пользователя: {userId}", exception.Message);
        }


        [Fact]
        public void GetUserById_WithExistId_UserRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;
            var user = new User(userId, "login", "email");

            _fakeUserRepository
               .Setup(repository => repository.GetById(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(user);

            // ACT
            _userService.GetById(userId, cancellationToken);

            // ASSERT
            _fakeUserRepository.Verify(repository => repository.GetById(userId, cancellationToken), Times.Once);
        }


        [Fact]
        public async void GetUserById_WithExistId_ReturnUser()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;
            var expectedUser = new User(userId, "login", "email");

            _fakeUserRepository
                .Setup(repository => repository.GetById(It.IsAny<int>(), cancellationToken))
                .ReturnsAsync(expectedUser);

            // ACT
            var actualUser = await _userService.GetById(userId, cancellationToken);
            bool areUsersEqual = actualUser.Id == expectedUser.Id
                                    && actualUser.Login == expectedUser.Login
                                    && actualUser.Email == expectedUser.Email;

            // ASSERT
            Assert.True(areUsersEqual);
        }


        [Fact]
        public async void CreateUser_WithLoginsLengthMore20Symbols_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            var user = new CreateUser("very_very_very_long_login", "");

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userService.Create(user, cancellationToken));

            // ASSERT
            Assert.Contains("Длина логина должна быть не больше 20 символов", exception.Message);
        }


        [Fact]
        public async void CreateUser_WithNullLogin_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            var user = new CreateUser("", "");

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userService.Create(user, cancellationToken));

            // ASSERT
            Assert.Contains("Логин не должен быть пустым", exception.Message);
        }


        [Fact]
        public async void CreateUser_WithValidLoginAndEmail_UserRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            var user = new CreateUser("login", "email");

            // ACT
            await _userService.Create(user, cancellationToken);

            // ASSERT
            _fakeUserRepository.Verify(repository => repository.Create(user, cancellationToken), Times.Once);
        }


        [Fact]
        public async void UpdateUser_WithLoginsLengthMore20Symbols_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            var user = new User(1, "very_very_very_long_login", "");

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userService.Update(user, cancellationToken));

            // ASSERT
            Assert.Contains("Длина логина должна быть не больше 20 символов", exception.Message);
        }


        [Fact]
        public async void UpdateUser_WithNullLogin_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            var user = new User(1, "", "");

            // ACT
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userService.Update(user, cancellationToken));

            // ASSERT
            Assert.Contains("Логин не должен быть пустым", exception.Message);
        }


        [Fact]
        public async void UpdateUser_WithValidLoginAndEmail_UserRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            var user = new User(1, "login", "email");

            // ACT
            await _userService.Update(user, cancellationToken);

            // ASSERT
            _fakeUserRepository.Verify(repository => repository.Update(user, cancellationToken), Times.Once);
        }


        [Fact]
        public async void DeleteUser_WithNonExistId_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;

            _fakeUserRepository
               .Setup(repository => repository.Exists(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(false);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.DeleteById(userId, cancellationToken));

            // ASSERT
            Assert.Contains($"Такого пользователя нет в БД. Id пользователя: {userId}", exception.Message);
        }


        [Fact]
        public async void DeleteUser_WithActiveBankAccounts_ShouldThrowException()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;

            _fakeUserRepository
               .Setup(repository => repository.Exists(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(true);

            _fakeBankAccountRepository
               .Setup(repository => repository.IsUserHaveAccounts(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(true);

            // ACT
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.DeleteById(userId, cancellationToken));

            // ASSERT
            Assert.Contains($"У пользователя ещё остались незакрытые счета. Такого пользователя удалить нельзя. Id пользователя: {userId}", exception.Message);
        }


        [Fact]
        public async void DeleteUser_WithoutActiveBankAccounts_UserRepositoryCalled()
        {
            // ARRANGE
            CancellationToken cancellationToken = default;
            int userId = 1;

            _fakeUserRepository
               .Setup(repository => repository.Exists(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(true);

            _fakeBankAccountRepository
               .Setup(repository => repository.IsUserHaveAccounts(It.IsAny<int>(), cancellationToken))
               .ReturnsAsync(false);

            // ACT
            await _userService.DeleteById(userId, cancellationToken);

            // ASSERT
            _fakeUserRepository.Verify(repository => repository.DeleteById(It.IsAny<int>(), cancellationToken), Times.Once);
        }
        

    }
}