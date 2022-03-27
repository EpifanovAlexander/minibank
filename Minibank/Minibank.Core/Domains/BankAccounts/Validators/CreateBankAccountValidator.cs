using FluentValidation;
using Minibank.Core.Domains.Currencies;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.BankAccounts.Validators
{
    public class CreateBankAccountValidator : AbstractValidator<CreateBankAccount>
    {
        public CreateBankAccountValidator(IUserRepository userRepository)
        {
            RuleFor(account => account.Sum).GreaterThanOrEqualTo(0)
                .WithMessage("Ошибка: Нельзя добавить счёт с отрицательной суммой");

            RuleFor(account => account.Currency.ToString()).IsEnumName(typeof(Currency), caseSensitive: false)
                .WithMessage("Ошибка: Нельзя создать счёт с такой валютой");

            RuleFor(account => account).MustAsync((account, CancellationToken) => userRepository.Exists(account.UserId))
                .WithMessage(account => $"Ошибка: Такого пользователя нет в БД. Id пользователя: {account.UserId}");
          
        }
    }
}
