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

        public async Task<double> GetExchangeRate(Currency? fromCurrency, Currency? toCurrency)
        {
            var response = await _httpClient.GetFromJsonAsync<CourseResponse>("daily_json.js");
            if (response == null)
            {
                throw new ValidationException("Ошибка: файл с данными о курсах валют не найден");
            }

            var currenciesDictionary = response.Valute;
            currenciesDictionary.Add("RUB", new ValueItem() { Value = 1 });

            ValueItem? itemFromCurrency = currenciesDictionary.GetValueOrDefault(fromCurrency.ToString());
            if (itemFromCurrency==null)
            {
                throw new ValidationException("Ошибка: неверно указана начальная валюта");
            }

            if (fromCurrency.Equals(toCurrency))
            {
                return 1;
            }

            ValueItem? itemToCurrency = currenciesDictionary.GetValueOrDefault(toCurrency.ToString());
            if (itemToCurrency==null)
            {
                throw new ValidationException("Ошибка: неверно указана конечная валюта");
            }

            return itemFromCurrency.Value / itemToCurrency.Value;
        }
    }
}