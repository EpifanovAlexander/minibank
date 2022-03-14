namespace Minibank.Core.Domains.Users.Services
{
    public interface IUserService
    {
        User Get(int userId);
        IEnumerable<User> GetAll();
        void Create(User user);
        void Update(User user);
        void Delete(int userId);
    }
}
