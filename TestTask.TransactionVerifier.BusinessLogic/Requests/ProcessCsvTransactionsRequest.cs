using TestTask.TransactionVerifier.Common.Models;

namespace TestTask.TransactionVerifier.BusinessLogic.Requests;

public class ProcessCsvTransactionsRequest
{
    public List<CsvTransactionModel> CsvTransactions { get; set; }

    public string FileHash { get; set; }
}
