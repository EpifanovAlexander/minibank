using Minibank.Core.Exceptions;
using Minibank.Core.Domains.Currencies.Services;

namespace Minibank.Core.Domains.Currencies
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyRateService _currencyRateService;

        public CurrencyConverter(ICurrencyRateService currencyRateService)
        {
            _currencyRateService = currencyRateService;
        }

        public async Task<double> Convert(double amount, Currency? fromCurrency, Currency? toCurrency, CancellationToken cancellationToken)
        {
            if (amount < 0)
            {
                throw new ValidationException("Ошибка: указана отрицательная сумма для конвертирования");
            }

            double exchangeRate = await _currencyRateService.GetExchangeRate(fromCurrency, toCurrency, cancellationToken);
            return Math.Round(amount * exchangeRate, 2);
        }
    }
}