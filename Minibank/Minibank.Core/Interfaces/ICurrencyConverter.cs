namespace Minibank.Core.Interfaces
{
    public interface ICurrencyConverter
    {
        double Convert(int sum, string currency);
    }
}
