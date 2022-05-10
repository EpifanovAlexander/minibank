namespace Minibank.Data.HttpClients.Models
{
    public class CourseResponse
    {
        public DateTime Date { get; set; }

        public Dictionary<string, ValueItem> Valute { get; set; }
    }
}
