using Minibank.Core.Exceptions;
using Minibank.Core.Domains.Currencies.Services;
using Minibank.Core.Domains.Currencies;
using Minibank.Data.HttpClients.Models;
using System.Net.Http.Json;

namespace Minibank.Data.HttpClients
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly HttpClient _httpClient;

        public CurrencyRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public double GetExchangeRate(Currency fromCurrency, Currency toCurrency)
        {
            var currenciesDictionary = _httpClient.GetFromJsonAsync<CourseResponse>("daily_json.js")
                .GetAwaiter().GetResult().Valute;

            currenciesDictionary.Add("RUB", new ValueItem() { Value = 1 });

            ValueItem itemFromCurrency = new();
            if (!currenciesDictionary.TryGetValue(fromCurrency.ToString(), out itemFromCurrency))
            {
                throw new ValidationException("Ошибка: неверно указана начальная валюта");
            }

            if (fromCurrency.Equals(toCurrency))
            {
                return 1;
            }

            ValueItem itemToCurrency = new();
            if (!currenciesDictionary.TryGetValue(toCurrency.ToString(), out itemToCurrency))
            {
                throw new ValidationException("Ошибка: неверно указана конечная валюта");
            }

            return itemFromCurrency.Value / itemToCurrency.Value;
        }
    }
}