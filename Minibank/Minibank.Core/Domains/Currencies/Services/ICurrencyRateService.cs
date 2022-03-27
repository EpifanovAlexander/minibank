namespace Minibank.Core.Domains.Currencies.Services
{
    public interface ICurrencyRateService
    {
        Task<double> GetExchangeRate(Currency fromCurrency, Currency toCurrency);
    }
}
