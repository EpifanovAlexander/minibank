namespace Minibank.Core.Domains.Users.Services
{
    public interface IUserService
    {
        User GetById(int userId);
        IEnumerable<User> GetAll();
        void Create(CreateUser user);
        void Update(User user);
        void DeleteById(int userId);
    }
}
