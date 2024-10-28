using TestTask.TransactionVerifier.Common.Enums;
using TestTask.TransactionVerifier.Common.Requests.Base;

namespace TestTask.TransactionVerifier.BusinessLogic.Requests;

public class GetComparisionResultsRequest : BasePaginatedRequest
{
    public string FileHash { get; set; }

    public TransactionComparisionType ComparisionType { get; set; }
}
