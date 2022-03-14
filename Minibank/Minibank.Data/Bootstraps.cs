using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Currencies.Services;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Data.DbModels.BankAccounts;
using Minibank.Data.DbModels.Users;
using Minibank.Data.HttpClients;


namespace Minibank.Data
{
    public static class Bootstraps
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ICurrencyRateService, CurrencyRateService>(options =>
            {
                options.BaseAddress = new Uri(configuration["CurrencyRatesUri"]);
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();

            return services;
        }
    }
}
