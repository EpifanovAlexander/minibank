using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;


namespace Minibank.Data.DbModels.Users
{
    public class UserRepository : IUserRepository
    {
        private static List<UserDbModel> _userStorage = new List<UserDbModel>();

        public User Get(int id)
        {
            var userDbModel = _userStorage.FirstOrDefault(_user => _user.Id == id);
            if (userDbModel == null)
            {
                return null;
            }
            return new User(userDbModel.Id, userDbModel.Login, userDbModel.Email);
        }

        public IEnumerable<User> GetAll()
        {
            return _userStorage.Select(_user => new User(_user.Id, _user.Login, _user.Email));
        }

        public void Create(User user)
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
            var userDbModel = _userStorage.FirstOrDefault(_user => _user.Id == user.Id);

            if (userDbModel != null)
            {
                userDbModel.Login = user.Login;
                userDbModel.Email = user.Email;
            }
        }

        public void Delete(int id)
        {
            var userDbModel = _userStorage.FirstOrDefault(_user => _user.Id == id);
            if (userDbModel != null)
            {
                _userStorage.Remove(userDbModel);
            }
        }
    }
}
