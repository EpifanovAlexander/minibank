using Minibank.Core.Exceptions;
using Minibank.Core.Interfaces;

namespace Minibank.Core
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyRateService _currencyRateService;

        public CurrencyConverter(ICurrencyRateService currencyRateService)
        {
            _currencyRateService = currencyRateService;
        }

        public double Convert(int sum, string currency)
        {
            if (sum < 0)
            {
                throw new UserFriendlyException("Ошибка: получена отрицательная сумма в результате конвертирования");
            }

            int exchangeRate = _currencyRateService.GetExchangeRate(currency);
            if (exchangeRate<0)
            {
                throw new UserFriendlyException("Ошибка: неверно указана валюта");
            }

            double result = (double)sum / exchangeRate;
            return Math.Round(result, 3);
        }
    }
}