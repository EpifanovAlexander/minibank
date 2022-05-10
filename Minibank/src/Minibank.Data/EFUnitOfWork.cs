using Minibank.Core;

namespace Minibank.Data
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly MinibankContext _context;

        public EfUnitOfWork(MinibankContext context)
        {
            _context = context;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
