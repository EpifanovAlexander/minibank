using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions;

namespace Minibank.Data.DbModels.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static readonly List<UserDbModel> _userStorage = new();

        public bool Exists(int id)
        {
            return _userStorage.Exists(user => user.Id == id);
        }

        public User? GetById(int id)
        {
            var userDbModel = _userStorage.FirstOrDefault(user => user.Id == id);
            if (userDbModel == null)
            {
                return null;
            }
            return new User(userDbModel.Id, userDbModel.Login, userDbModel.Email);
        }

        public IEnumerable<User> GetAll()
        {
            return _userStorage.Select(user => new User(user.Id, user.Login, user.Email));
        }

        public void Create(CreateUser user)
        {
            var userDbModel = new UserDbModel
            {
                Id = (_userStorage.Count==0) ? 0 : _userStorage.Max(u => u.Id)+1,
                Login = user.Login,
                Email = user.Email
            };
            _userStorage.Add(userDbModel);
        }

        public void Update(User user)
        {
            var userDbModel = _userStorage.FirstOrDefault(userInStorage => userInStorage.Id == user.Id);
            if (userDbModel == null)
            {
                throw new ValidationException($"Пользователь не найден. Id пользователя: {user.Id}");
            }

            userDbModel.Login = user.Login;
            userDbModel.Email = user.Email;
        }

        public void DeleteById(int id)
        {
            if (!Exists(id))
            {
                throw new ValidationException($"Пользователь не найден. Id пользователя: {id}");
            }
            
            _userStorage.Remove(_userStorage.First(user => user.Id == id));
        }
    }
}
