using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.WebApi.Requests;

namespace TestTask.TransactionVerifier.WebApi.Services.Interfaces;


public interface ITransactionService
{
    Task<List<CsvTransactionModel>> ProcessData(ProcessDataRequest request, CancellationToken cancellationToken);
}
