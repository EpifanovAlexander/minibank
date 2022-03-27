
namespace Minibank.Core
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveChanges();
    }
}
