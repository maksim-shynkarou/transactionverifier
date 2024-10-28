using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.WebApi.Requests;
using TestTask.TransactionVerifier.WebApi.Responses;

namespace TestTask.TransactionVerifier.WebApi.Services.Interfaces;

public interface ICsvProcessingService
{
    Task ProcessTransactions(IFormFile? csvFile, CancellationToken cancellationToken);

    Task<PaginatedResponse<TransactionModel>> GetComparisionResultAsync(GetComparisionResultsRequest request,
        CancellationToken cancellationToken);
}
