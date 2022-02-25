namespace Minibank.Core
{
    public interface ICurrencyService
    {
        public int GetExchangeRate(string currency);
    }
}
