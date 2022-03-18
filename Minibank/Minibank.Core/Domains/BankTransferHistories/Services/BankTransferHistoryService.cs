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

        public IEnumerable<BankTransferHistory> GetUserTransferHistory(int userId)
        {
            bool isUserExist = _userRepository.IsUserExist(userId);
            if (!isUserExist)
            {
                throw new ValidationException("Ошибка: Такого пользователя нет в БД");
            }

            return _bankTransferHistoryRepository.GetUserTransferHistory(userId);
        }
    }
}
