using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.Currencies.Services;
using Minibank.Core.Domains.Currencies;
using Minibank.Core.Domains.Users.Services;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankTransferHistories.Services;

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
            return services;
        }
    }
}
