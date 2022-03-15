namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        User Get(int userId);
        IEnumerable<User> GetAll();
        void Create(User user);
        bool Update(User user);
        bool Delete(int userId);
        bool IsUserExist(int id);
    }
}
