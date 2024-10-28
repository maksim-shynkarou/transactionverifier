using System.Xml.Linq;

namespace TestTask.TransactionVerifier.Common.Models
{
    public class CsvTransactionModel
    {
        public decimal Amount { get; set; }

        public DateTime ProcessedAt { get; set; }

        public string Description { get; set; }
    }
}
