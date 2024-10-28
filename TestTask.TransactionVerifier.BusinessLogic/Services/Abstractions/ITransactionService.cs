using TestTask.TransactionVerifier.BusinessLogic.Requests;

namespace TestTask.TransactionVerifier.BusinessLogic.Services.Abstractions;


public interface ITransactionService
{
    Task<bool> ProcessData(ProcessCsvTransactionsRequest request, CancellationToken cancellationToken);
}
