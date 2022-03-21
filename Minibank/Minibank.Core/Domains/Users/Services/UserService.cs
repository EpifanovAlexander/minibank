using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions;

namespace Minibank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUserRepository _userRepository;

        public UserService(IBankAccountRepository bankAccountRepository, IUserRepository userRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _userRepository = userRepository;
        }

        public User GetById(int id)
        {
            return _userRepository.GetById(id)
                ?? throw new ValidationException($"Такого пользователя нет в БД. Id пользователя: {id}");
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public void Create(CreateUser user)
        {
            if (string.IsNullOrEmpty(user.Login) || user.Login.Length > 20)
            {
                throw new ValidationException("Не задан логин или длина более 20 символов");
            }

            _userRepository.Create(user);
        }

        public void Update(User user)
        {
            if (string.IsNullOrEmpty(user.Login) || user.Login.Length > 20)
            {
                throw new ValidationException("Не задан логин или длина более 20 символов");
            }

            _userRepository.Update(user);
        }

        public void DeleteById(int userId)
        {
            if (_bankAccountRepository.IsUserHaveAccounts(userId))
            {
                throw new ValidationException("У пользователя ещё остались незакрытые счета. Такого пользователя удалить нельзя");
            }

            _userRepository.DeleteById(userId);
        }
    }
}
