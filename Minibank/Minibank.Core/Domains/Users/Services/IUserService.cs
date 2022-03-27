namespace Minibank.Core.Domains.Users.Services
{
    public interface IUserService
    {
        Task<User> GetById(int userId);
        IAsyncEnumerable<User> GetAll();
        Task Create(CreateUser user);
        Task Update(User user);
        Task DeleteById(int userId);
    }
}
