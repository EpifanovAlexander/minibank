namespace Minibank.Core.Interfaces
{
    public interface ICurrencyRateService
    {
        public int GetExchangeRate(string currency);
    }
}
