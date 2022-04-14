namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetById(int userId, CancellationToken cancellationToken);
        Task<List<User>> GetAll(CancellationToken cancellationToken);
        Task Create(CreateUser user, CancellationToken cancellationToken);
        Task Update(User user, CancellationToken cancellationToken);
        Task DeleteById(int userId, CancellationToken cancellationToken);
        Task<bool> Exists(int id, CancellationToken cancellationToken);
    }
}
