namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        User GetById(int userId);
        IEnumerable<User> GetAll();
        void Create(CreateUser user);
        void Update(User user);
        void DeleteById(int userId);
        bool IsUserExist(int id);
    }
}
