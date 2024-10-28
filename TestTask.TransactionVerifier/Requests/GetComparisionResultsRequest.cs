using System.Text.Json.Serialization;
using TestTask.TransactionVerifier.Common.Enums;
using TestTask.TransactionVerifier.WebApi.Requests.Base;

namespace TestTask.TransactionVerifier.WebApi.Requests;

public class GetComparisionResultsRequest : BasePaginatedRequest
{
    public string FileHash { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionComparisionType ComparisionType { get; set; }
}
