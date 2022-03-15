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

        public User Get(int id)
        {
            return _userRepository.Get(id) ?? throw new ValidationException("Такого пользователя нет в БД");
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public void Create(User user)
        {
            if (user.Login == null || user.Login.Length > 20)
            {
                throw new ValidationException("Не задан логин или длина более 20 символов");
            }
            _userRepository.Create(user);
        }

        public void Update(User user)
        {
            if (!_userRepository.Update(user))
            {
                throw new ValidationException("Пользователь не обновился");
            }
        }

        public void Delete(int id)
        {
            if (_bankAccountRepository.GetUserAccounts(id).Any())
            {
                throw new ValidationException("У пользователя ещё остались незакрытые счета. Такого пользователя удалить нельзя");
            }
            if (!_userRepository.Delete(id))
            {
                throw new ValidationException("Пользователь не удалился");
            }
        }
    }
}
