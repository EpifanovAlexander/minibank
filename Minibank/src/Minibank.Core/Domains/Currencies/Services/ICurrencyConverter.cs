namespace Minibank.Core.Domains.Currencies.Services
{
    public interface ICurrencyConverter
    {
        Task<double> Convert(double amount, Currency? fromCurrency, Currency? toCurrency, CancellationToken cancellationToken);
    }
}
