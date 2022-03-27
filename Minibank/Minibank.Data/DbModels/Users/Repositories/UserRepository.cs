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


        public async Task<bool> Exists(int id)
        {
            return await _context.Users.AnyAsync(user => user.Id == id);
        }


        public async Task<User?> GetById(int id)
        {
            var userDbModel = await _context.Users
                .Include(it => it.BankAccounts)
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Id == id);

            if (userDbModel == null)
            {
                return null;
            }
            return new User(userDbModel.Id, userDbModel.Login, userDbModel.Email);
        }


        public async IAsyncEnumerable<User> GetAll()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();

            foreach (var user in users)
            {
                yield return new User(user.Id, user.Login, user.Email);
            }
        }


        public async Task Create(CreateUser user)
        {
            var userDbModel = new UserDbModel
            {
                Id = 0,
                Login = user.Login,
                Email = user.Email
            };

            await _context.Users.AddAsync(userDbModel);
        }


        public async Task Update(User user)
        {
            var userDbModel = await _context.Users.FirstOrDefaultAsync(it => it.Id == user.Id);
            if (userDbModel == null)
            {
                throw new ValidationException($"Пользователь не найден. Id пользователя: {user.Id}");
            }

            userDbModel.Login = user.Login;
            userDbModel.Email = user.Email;
            _context.Entry(userDbModel).State = EntityState.Modified;
        }


        public async Task DeleteById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
            if (user==null)
            {
                throw new ValidationException($"Пользователь не найден. Id пользователя: {id}");
            }

            _context.Users.Remove(user);
        }
    }
}
