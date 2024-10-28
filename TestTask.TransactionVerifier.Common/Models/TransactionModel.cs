namespace TestTask.TransactionVerifier.Common.Models
{
    public class TransactionModel
    {
        public decimal Amount { get; set; }

        public DateTime ProcessedAt { get; set; }

        public string Description { get; set; }
    }
}
