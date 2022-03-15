using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankTransferHistories.Repositories;
using Minibank.Core.Domains.Currencies.Services;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Data.DbModels.BankAccounts.Repositories;
using Minibank.Data.DbModels.BankTransferHistories.Repositories;
using Minibank.Data.DbModels.Users.Repositories;
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
            services.AddScoped<IBankTransferHistoryRepository, BankTransferHistoryRepository>();

            return services;
        }
    }
}
