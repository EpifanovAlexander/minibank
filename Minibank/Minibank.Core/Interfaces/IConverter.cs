namespace Minibank.Core
{
    public interface IConverter
    {
        double Convert(int sum, string currency);
    }
}
