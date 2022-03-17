namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        User GetById(int userId);
        IEnumerable<User> GetAll();
        void Create(CreateUser user);
        bool Update(User user);
        bool DeleteById(int userId);
        bool IsUserExist(int id);
    }
}
