namespace TestTask.TransactionVerifier.WebApi.Requests.Base;

public class BasePaginatedRequest
{
    //TODO надо понять что будет прилетать с фронта. Можно подстроить TotalPages под этот показатель. Потому что сейчас TotalPages в респонсе это количество, а здесь это индекс страницы.
    public int PageNumber { get; set; } = 0;
    public int PageSize { get; set; } = 10;
}
