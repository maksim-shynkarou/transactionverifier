using Microsoft.AspNetCore.Http;
using TestTask.TransactionVerifier.BusinessLogic.Requests;
using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.Common.Responses;

namespace TestTask.TransactionVerifier.BusinessLogic.Services.Abstractions;

public interface ICsvProcessingService
{
    Task ProcessTransactions(IFormFile? csvFile, string fileHash, CancellationToken cancellationToken);

    Task<PaginatedResponse<TransactionModel>> GetComparisionResultAsync(GetComparisionResultsRequest request,
        CancellationToken cancellationToken);
}
