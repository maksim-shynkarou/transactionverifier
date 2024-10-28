using TestTask.TransactionVerifier.Common.Enums;
using TestTask.TransactionVerifier.WebApi.Requests.Base;

namespace TestTask.TransactionVerifier.WebApi.Requests;

public class GetComparisionResultsRequest : BasePaginatedRequest
{
    public string FileHash { get; set; }

    public TransactionComparisionType ComparisionType { get; set; }
}
