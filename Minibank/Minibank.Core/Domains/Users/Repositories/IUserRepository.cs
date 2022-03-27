namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetById(int userId);
        IAsyncEnumerable<User> GetAll();
        Task Create(CreateUser user);
        Task Update(User user);
        Task DeleteById(int userId);
        Task<bool> Exists(int id);
    }
}
