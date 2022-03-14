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

        public double Convert(double amount, Currency fromCurrency, Currency toCurrency)
        {
            if (amount < 0)
            {
                throw new ValidationException("Ошибка: указана отрицательная сумма для конвертирования");
            }

            double exchangeRate = _currencyRateService.GetExchangeRate(fromCurrency, toCurrency);
            return Math.Round(amount*exchangeRate, 2);
        }
    }
}