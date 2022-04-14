using Minibank.Core.Domains.BankTransferHistories.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions;

namespace Minibank.Core.Domains.BankTransferHistories.Services
{
    public class BankTransferHistoryService : IBankTransferHistoryService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankTransferHistoryRepository _bankTransferHistoryRepository;

        public BankTransferHistoryService(IUserRepository userRepository, IBankTransferHistoryRepository bankTransferHistoryRepository)
        {
            _userRepository = userRepository;
            _bankTransferHistoryRepository = bankTransferHistoryRepository;
        }

        public async Task<List<BankTransferHistory>> GetUserTransferHistory(int userId, CancellationToken cancellationToken)
        {
            bool isUserExist = await _userRepository.Exists(userId, cancellationToken);
            if (!isUserExist)
            {
                throw new ValidationException($"Ошибка: Такого пользователя нет в БД. Id пользователя: {userId}");
            }

            return await _bankTransferHistoryRepository.GetUserTransferHistory(userId, cancellationToken);
        }
    }
}
