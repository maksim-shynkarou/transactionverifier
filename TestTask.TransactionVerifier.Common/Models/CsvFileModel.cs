namespace TestTask.TransactionVerifier.Common.Models
{
    public class CsvFileModel
    {
        public string FileName { get; set; }

        public string FileHash { get; set; }

        public DateTime ProcessedAt { get; set; }
    }
}
