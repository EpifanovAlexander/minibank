using Microsoft.EntityFrameworkCore;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions;

namespace Minibank.Data.DbModels.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MinibankContext _context;

        public UserRepository(MinibankContext context)
        {
            _context = context;
        }


        public async Task<bool> Exists(int id, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(user => user.Id == id, cancellationToken);
        }


        public async Task<User?> GetById(int id, CancellationToken cancellationToken)
        {
            var userDbModel = await _context.Users
                .Include(it => it.BankAccounts)
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

            if (userDbModel == null)
            {
                return null;
            }
            return new User(userDbModel.Id, userDbModel.Login, userDbModel.Email);
        }


        public async Task<List<User>> GetAll(CancellationToken cancellationToken)
        {
           return await _context.Users
                .AsNoTracking()
                .Select(user => new User(user.Id, user.Login, user.Email))
                .ToListAsync(cancellationToken);
        }


        public async Task Create(CreateUser user, CancellationToken cancellationToken)
        {
            var userDbModel = new UserDbModel
            {
                Id = 0,
                Login = user.Login,
                Email = user.Email
            };

            await _context.Users.AddAsync(userDbModel, cancellationToken);
        }


        public async Task Update(User user, CancellationToken cancellationToken)
        {
            var userDbModel = await _context.Users.FirstOrDefaultAsync(it => it.Id == user.Id, cancellationToken);
            if (userDbModel == null)
            {
                throw new ValidationException($"Пользователь не найден. Id пользователя: {user.Id}");
            }

            userDbModel.Login = user.Login;
            userDbModel.Email = user.Email;
            _context.Entry(userDbModel).State = EntityState.Modified;
        }


        public async Task DeleteById(int id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
            if (user==null)
            {
                throw new ValidationException($"Пользователь не найден. Id пользователя: {id}");
            }

            _context.Users.Remove(user);
        }
    }
}
