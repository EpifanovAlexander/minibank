using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;


namespace Minibank.Data.DbModels.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static List<UserDbModel> _userStorage = new List<UserDbModel>();

        public bool IsUserExist(int id)
        {
            return _userStorage.Exists(_user => _user.Id == id);
        }

        public User GetById(int id)
        {
            if (IsUserExist(id))
            {
                var userDbModel = _userStorage.FirstOrDefault(_user => _user.Id == id);
                return new User(userDbModel.Id, userDbModel.Login, userDbModel.Email);
            }
            return null;
        }

        public IEnumerable<User> GetAll()
        {
            return _userStorage.Select(_user => new User(_user.Id, _user.Login, _user.Email));
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

        public bool Update(User user)
        {
            if (IsUserExist(user.Id))
            {
                var userDbModel = _userStorage.FirstOrDefault(_user => _user.Id == user.Id);
                userDbModel.Login = user.Login;
                userDbModel.Email = user.Email;
                return true;
            }
            return false;
        }

        public bool DeleteById(int id)
        {
            if (IsUserExist(id))
            {
                _userStorage.Remove(_userStorage.FirstOrDefault(_user => _user.Id == id));
                return true;
            }
            return false;
        }
    }
}
