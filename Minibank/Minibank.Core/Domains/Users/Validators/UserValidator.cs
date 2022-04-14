using FluentValidation;

namespace Minibank.Core.Domains.Users.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Login).NotEmpty().WithMessage("Логин не должен быть пустым");
            RuleFor(user => user.Login.Length).LessThanOrEqualTo(20).WithMessage("Длина логина должна быть не больше 20 символов");
        }
    }
}
