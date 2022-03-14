namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        User Get(int userId);
        IEnumerable<User> GetAll();
        void Create(User user);
        void Update(User user);
        void Delete(int userId);
    }
}
