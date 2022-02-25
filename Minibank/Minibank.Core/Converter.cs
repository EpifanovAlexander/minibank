using Minibank.Core.Exceptions;

namespace Minibank.Core
{
    public class Converter : IConverter
    {
        private readonly ICurrencyService _currencyService;

        public Converter(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public double Convert(int sum, string currency)
        {
            int exchangeRate = _currencyService.GetExchangeRate(currency);
            if (exchangeRate<0)
            {
                throw new UserFriendlyException("Ошибка: неверно указана валюта");
            }

            double result = (double) sum / exchangeRate;
            if (result < 0)
            {
                throw new UserFriendlyException("Ошибка: получена отрицательная сумма в результате конвертирования");
            }
            return Math.Round(result, 3);
        }
    }
}