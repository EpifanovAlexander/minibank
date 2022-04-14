using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.Currencies.Services;
using Minibank.Core.Domains.Currencies;
using Minibank.Core.Domains.Users.Services;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankTransferHistories.Services;
using FluentValidation.AspNetCore;
using FluentValidation;

namespace Minibank.Core
{
    public static class Bootstraps
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IBankTransferHistoryService, BankTransferHistoryService>();

            services.AddFluentValidation()
                .AddValidatorsFromAssembly(typeof(UserService).Assembly);

            services.AddFluentValidation()
                .AddValidatorsFromAssembly(typeof(BankAccountService).Assembly);

            return services;
        }
    }
}
