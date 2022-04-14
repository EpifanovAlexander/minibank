using FluentValidation;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using ValidationException = Minibank.Core.Exceptions.ValidationException;

namespace Minibank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<User> _userValidator;
        private readonly IValidator<CreateUser> _createUserValidator;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IBankAccountRepository bankAccountRepository, IUserRepository userRepository,
            IValidator<User> userValidator, IValidator<CreateUser> createUserValidator, IUnitOfWork unitOfWork)
        {
            _bankAccountRepository = bankAccountRepository;
            _userRepository = userRepository;
            _userValidator = userValidator;
            _createUserValidator = createUserValidator;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> GetById(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetById(id, cancellationToken);

            return user
                ?? throw new ValidationException($"Такого пользователя нет в БД. Id пользователя: {id}");
        }

        public Task<List<User>> GetAll(CancellationToken cancellationToken)
        {
            return _userRepository.GetAll(cancellationToken);
        }

        public async Task Create(CreateUser user, CancellationToken cancellationToken)
        {
            _createUserValidator.ValidateAndThrow(user);

            await _userRepository.Create(user, cancellationToken);
            await _unitOfWork.SaveChanges();
        }

        public async Task Update(User user, CancellationToken cancellationToken)
        {
            _userValidator.ValidateAndThrow(user);

            await _userRepository.Update(user, cancellationToken);
            await _unitOfWork.SaveChanges();
        }

        public async Task DeleteById(int userId, CancellationToken cancellationToken)
        {
            var isUserExist = await _userRepository.Exists(userId, cancellationToken);
            if (!isUserExist)
            {
                throw new ValidationException($"Такого пользователя нет в БД. Id пользователя: {userId}");
            }

            var isUserHaveAccounts = await _bankAccountRepository.IsUserHaveAccounts(userId, cancellationToken);
            if (isUserHaveAccounts)
            {
                throw new ValidationException($"У пользователя ещё остались незакрытые счета. Такого пользователя удалить нельзя. Id пользователя: {userId}");
            }

            await _userRepository.DeleteById(userId, cancellationToken);
            await _unitOfWork.SaveChanges();
        }
    }
}
