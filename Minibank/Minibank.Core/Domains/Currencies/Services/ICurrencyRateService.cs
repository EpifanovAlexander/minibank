namespace Minibank.Core.Domains.Currencies.Services
{
    public interface ICurrencyRateService
    {
        public double GetExchangeRate(Currency fromCurrency, Currency toCurrency);
    }
}
