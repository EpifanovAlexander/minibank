namespace Minibank.Core.Domains.Currencies.Services
{
    public interface ICurrencyConverter
    {
        double Convert(double amount, Currency fromCurrency, Currency toCurrency);
    }
}
