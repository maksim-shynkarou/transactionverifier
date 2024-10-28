using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using TestTask.TransactionVerifier.BusinessLogic.Requests;
using TestTask.TransactionVerifier.BusinessLogic.Services.Abstractions;
using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.DataAccess.Contexts.Abstractions;
using TestTask.TransactionVerifier.DataAccess.Entities;

namespace TestTask.TransactionVerifier.BusinessLogic.Services;

public class TransactionService(ITransactionVerifierDbContext transactionVerifierDbContext) : ITransactionService
{
    public const int FetchDataBatchSize = 100; // настроить исходя из задачи/мощности
    public const int PushDataBatchSize = 100; // настроить исходя из задачи/мощности
    public const int TransactionGapDays = 1;
    public static readonly TimeSpan TransactionMinGap = TimeSpan.FromDays(-TransactionGapDays);
    public static readonly TimeSpan TransactionMaxGap = TimeSpan.FromDays(TransactionGapDays);

    public async Task<bool> ProcessData(ProcessCsvTransactionsRequest request, CancellationToken cancellationToken)
    {
        return await ProcessTransactionsInBatchesAsync(request.CsvTransactions, request.FileHash, cancellationToken);
    }

    private async Task<bool> ProcessTransactionsInBatchesAsync(List<CsvTransactionModel> csvTransactions, string fileHash, CancellationToken cancellationToken)
    {
        var intersectCsvTransactions = new ConcurrentDictionary<Guid, CsvTransaction>();
        var remainingCsvTransactions = new ConcurrentDictionary<CsvTransactionModel, object?>(
            csvTransactions.ToDictionary(item => item, object? (item) => null)
        );

        if (!remainingCsvTransactions.Any())
            return true;

        var skip = 0;
        var minDateTime = csvTransactions.MinBy(x => x.ProcessedAt)!.ProcessedAt.Add(TransactionMinGap);
        var maxDateTime = csvTransactions.MaxBy(x => x.ProcessedAt)!.ProcessedAt.Add(TransactionMaxGap);

        while (true)
        {
            var batch = await GetBatchAsync(cancellationToken, minDateTime, maxDateTime, skip);

            if (!batch.Any()) break;

            Parallel.ForEach(batch, (transaction, token) =>
            {
                if (!remainingCsvTransactions.Any()) return;

                var intersectTransaction = remainingCsvTransactions
                    .FirstOrDefault(x => x.Key.Amount == transaction.Amount &&
                                         x.Key.Description == transaction.Description &&
                                         x.Key.ProcessedAt >= transaction.ProcessedAt.Add(TransactionMinGap) &&
                                         x.Key.ProcessedAt <= transaction.ProcessedAt.Add(TransactionMaxGap)).Key;

                if (intersectTransaction == null) return;

                var dbCsvTransaction = new CsvTransaction
                {
                    Amount = intersectTransaction.Amount,
                    ProcessedAt = intersectTransaction.ProcessedAt,
                    CsvFileHash = fileHash,
                    Description = intersectTransaction.Description,
                    TransactionId = transaction.Id
                };

                intersectCsvTransactions.TryAdd(Guid.NewGuid(), dbCsvTransaction);
                remainingCsvTransactions.TryRemove(intersectTransaction, out _);
            });

            await SaveCsvIntersectTransactions(cancellationToken, intersectCsvTransactions);
            intersectCsvTransactions.Clear();
            skip += FetchDataBatchSize;
        }

        await SaveUniqueCsvTransactions(fileHash, cancellationToken, remainingCsvTransactions);

        return true;
    }

    private async Task SaveCsvIntersectTransactions(CancellationToken cancellationToken,
        ConcurrentDictionary<Guid, CsvTransaction> intersectCsvTransactions)
    {
        var csvIntersectTransactionsToInsert = intersectCsvTransactions.Values.ToList();
        for (var i = 0; i < csvIntersectTransactionsToInsert.Count; i += PushDataBatchSize)
        {
            var insertBatch = csvIntersectTransactionsToInsert.Skip(i).Take(PushDataBatchSize).ToList();

            await transactionVerifierDbContext.CsvTransactions.AddRangeAsync(insertBatch, cancellationToken);
            await transactionVerifierDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task SaveUniqueCsvTransactions(string fileHash, CancellationToken cancellationToken,
        ConcurrentDictionary<CsvTransactionModel, object?> remainingCsvTransactions)
    {
        var uniqueCsvTransactions = remainingCsvTransactions.Keys.Select(x => new CsvTransaction()
        {
            Amount = x.Amount,
            ProcessedAt = x.ProcessedAt,
            CsvFileHash = fileHash,
            Description = x.Description
        });

        await transactionVerifierDbContext.CsvTransactions.AddRangeAsync(uniqueCsvTransactions, cancellationToken);
        await transactionVerifierDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<Transaction>> GetBatchAsync(CancellationToken cancellationToken, DateTime minDateTime, DateTime maxDateTime,
        int skip)
    {
        var batch = await transactionVerifierDbContext.Transactions
            .Where(x => x.ProcessedAt >= minDateTime && x.ProcessedAt <= maxDateTime)
            .Skip(skip)
            .Take(FetchDataBatchSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return batch;
    }
}
