using Microsoft.EntityFrameworkCore;
using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.DataAccess.Contexts.Abstractions;
using TestTask.TransactionVerifier.DataAccess.Entities;
using TestTask.TransactionVerifier.WebApi.Requests;
using TestTask.TransactionVerifier.WebApi.Services.Interfaces;

namespace TestTask.TransactionVerifier.WebApi.Services.Implementations;

//Никогда не писал ничего подобного, но решил попробовать. Не уверен, что будет круто работать, так как времени тратить очень много не хочется на тесты. Но по крайней мере интересно что получится после тестов.
// Скорее всего какие-то еджкейсы останутся, но я думаю, что если потратить еще времени, то тут можно получить интересный результат с хорошей производительностью и без багов.
public class TransactionService : ITransactionService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ITransactionVerifierDbContext _transactionVerifierDbContext;

    public const int BatchSize = 100; // настроить исходя из задачи/мощности
    public const int TransactionGapDays = 1;
    public static readonly TimeSpan TransactionMinGap = TimeSpan.FromDays(-TransactionGapDays);
    public static readonly TimeSpan TransactionMaxGap = TimeSpan.FromDays(TransactionGapDays);

    public TransactionService(IServiceScopeFactory serviceScopeFactory, ITransactionVerifierDbContext transactionVerifierDbContext)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _transactionVerifierDbContext = transactionVerifierDbContext;
    }

    public async Task<List<CsvTransactionModel>> ProcessData(ProcessDataRequest request, CancellationToken cancellationToken)
    {
        var (updatedCsvTransactions, success) = await ProcessTransactionsInBatchesAsync(request.CsvTransactions, request.FileHash, cancellationToken);

        if (success)
        {
            return updatedCsvTransactions;
        }

        throw new Exception("Need to debug. Possible edgecase.");
    }


    private async Task<(List<CsvTransactionModel> RemainingCsvTransactions, bool Success)> ProcessTransactionsInBatchesAsync(List<CsvTransactionModel> csvTransactions, string fileHash, CancellationToken cancellationToken)
    {
        // TODO подумать над переделкой на hashset, если в CSV много транзакций. Улучшит поиск и удаление из коллекции.
        var remainingCsvTransactions = new List<CsvTransactionModel>(csvTransactions);

        if (!remainingCsvTransactions.Any())
            return (remainingCsvTransactions, true);

        int skip = 0;
        var minDateTime = csvTransactions.MinBy(x => x.ProcessedAt)!.ProcessedAt.Add(TransactionMinGap);
        var maxDateTime = csvTransactions.MaxBy(x => x.ProcessedAt)!.ProcessedAt.Add(TransactionMaxGap);

        while (true)
        {
            var batch = await GetBatchAsync(cancellationToken, minDateTime, maxDateTime, skip);

            if (!batch.Any()) break;

            var itemsToRemove = new List<CsvTransactionModel>();
            var amountOfRemovedItems = 0;
            var amountOfItemsInCsv = csvTransactions.Count;

            await Parallel.ForEachAsync(batch, cancellationToken, async (transaction, token) =>
            {
                if (amountOfItemsInCsv == amountOfRemovedItems)
                {
                    return;
                }

                var intersectTransaction = remainingCsvTransactions
                    .FirstOrDefault(x => x.Amount == transaction.Amount &&
                                         x.Description == transaction.Description &&
                                         x.ProcessedAt >= transaction.ProcessedAt.Add(TransactionMinGap) &&
                                         x.ProcessedAt <= transaction.ProcessedAt.Add(TransactionMaxGap));

                if (intersectTransaction != null)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scopedContext = scope.ServiceProvider.GetRequiredService<ITransactionVerifierDbContext>();
                        var dbCsvTransaction = new CsvTransaction
                        {
                            Amount = intersectTransaction.Amount,
                            ProcessedAt = intersectTransaction.ProcessedAt,
                            CsvFileHash = fileHash,
                            Description = intersectTransaction.Description,
                            TransactionId = transaction.Id
                        };

                        await scopedContext.CsvTransactions.AddAsync(dbCsvTransaction, token);
                        await scopedContext.SaveChangesAsync(token);
                    }

                    amountOfRemovedItems++;
                    itemsToRemove.Add(intersectTransaction);
                }
            });

            foreach (var item in itemsToRemove)
            {
                remainingCsvTransactions.Remove(item);
            }

            skip += BatchSize;
        }

        return (remainingCsvTransactions, true);
    }

    private async Task<List<Transaction>> GetBatchAsync(CancellationToken cancellationToken, DateTime minDateTime, DateTime maxDateTime,
        int skip)
    {
        var batch = await _transactionVerifierDbContext.Transactions
            .Where(x => x.ProcessedAt >= minDateTime && x.ProcessedAt <= maxDateTime)
            .Skip(skip)
            .Take(BatchSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return batch;
    }
}
