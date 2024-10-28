namespace TestTask.TransactionVerifier.Common.Models;

public sealed record CsvTransactionModel
{
    public decimal Amount { get; init; }

    public DateTime ProcessedAt { get; init; }

    public string Description { get; init; }
}
