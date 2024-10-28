namespace TestTask.TransactionVerifier.WebApi.Requests.Base;

public class BasePaginatedRequest
{
    //TODO надо понять что будет прилетать с фронта. Сейчас используется индекс страницы (начиная с 0)
    public int PageNumber { get; set; } = 0;
    public int PageSize { get; set; } = 10;
}
