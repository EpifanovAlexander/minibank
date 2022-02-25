using Minibank.Core;

namespace Minibank.Data
{
    public class CurrencyService : ICurrencyService
    {
        private Random _random;
        public CurrencyService()
        {
            _random = new Random();
        }

        public int GetExchangeRate(string currency)
        {
            switch (currency)
            {
                case "RUB": return 1;
                case "USD": return 20 + _random.Next(60);
                case "EUR": return 30 + _random.Next(60);
            }
            return -1;
        }
    }
}