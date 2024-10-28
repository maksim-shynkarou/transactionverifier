using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TestTask.TransactionVerifier.BusinessLogic.Requests;
using TestTask.TransactionVerifier.BusinessLogic.Services.Abstractions;
using TestTask.TransactionVerifier.Common.Enums;
using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.Common.Responses;
using TestTask.TransactionVerifier.DataAccess.Contexts.Abstractions;

namespace TestTask.TransactionVerifier.BusinessLogic.Services;

internal class CsvTransactionService(ITransactionVerifierDbContext context, ITransactionService transactionService) : ICsvProcessingService
{
    public async Task ProcessTransactions(IFormFile? csvFile, string fileHash, CancellationToken cancellationToken)
    {
        if ((csvFile?.Length ?? 0) == 0)
        {
            throw new Exception("File is empty or not provided.");
        }

        var csvTransactions = await GetCsvTransactionModelsFromCsvFile(csvFile);

        var request = new ProcessCsvTransactionsRequest
        {
            CsvTransactions = csvTransactions,
            FileHash = fileHash
        };

        await transactionService.ProcessData(request, cancellationToken);
    }
    
    public async Task<PaginatedResponse<TransactionModel>> GetComparisionResultAsync(GetComparisionResultsRequest request, CancellationToken cancellationToken)
    {
        switch (request.ComparisionType)
        {
            case TransactionComparisionType.Intersect:
                return await GetIntersectedTransactionsAsync(request, cancellationToken);
            case TransactionComparisionType.DbUnique:
                return await GetDatabaseUniqueTransactionsAsync(request, cancellationToken);
            case TransactionComparisionType.CsvUnique:
                return await GetCsvUniqueTransactionsAsync(request, cancellationToken);
            default:
                return new PaginatedResponse<TransactionModel>([], request.PageNumber, request.PageSize, 0);
        }
    }

    private async Task<List<CsvTransactionModel>> GetCsvTransactionModelsFromCsvFile(IFormFile? csvFile)
    {
        var csvTransactions = new List<CsvTransactionModel>();

        using var stream = new StreamReader(csvFile.OpenReadStream());
        using var csv = new CsvReader(stream, CultureInfo.InvariantCulture);
        await foreach (var record in csv.GetRecordsAsync<CsvTransactionModel>())
        {
            csvTransactions.Add(record);
        }

        return csvTransactions;
    }

    private async Task<PaginatedResponse<TransactionModel>> GetIntersectedTransactionsAsync(GetComparisionResultsRequest request, CancellationToken cancellationToken)
    {
        var totalItems = await context.CsvTransactions
            .AsNoTracking()
            .Where(x => x.CsvFileHash == request.FileHash && x.TransactionId != null)
            .CountAsync(cancellationToken);

        var transactions = await context.CsvTransactions
            .AsNoTracking()
            .Where(x => x.CsvFileHash == request.FileHash && x.TransactionId != null)
            .Skip(request.PageNumber * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new TransactionModel()
            {
                Amount = x.Amount,
                Description = x.Description,
                ProcessedAt = x.ProcessedAt,
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TransactionModel>(transactions, request.PageNumber, request.PageSize, totalItems);
    }

    private async Task<PaginatedResponse<TransactionModel>> GetCsvUniqueTransactionsAsync(GetComparisionResultsRequest request, CancellationToken cancellationToken)
    {
        var totalItems = await context.CsvTransactions
            .AsNoTracking()
            .Where(x => x.CsvFileHash == request.FileHash && x.TransactionId == null)
            .CountAsync(cancellationToken);

        var transactions = await context.CsvTransactions
            .AsNoTracking()
            .Where(x => x.CsvFileHash == request.FileHash && x.TransactionId == null)
            .Skip(request.PageNumber * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new TransactionModel
            {
                Amount = x.Amount,
                Description = x.Description,
                ProcessedAt = x.ProcessedAt,
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TransactionModel>(transactions, request.PageNumber, request.PageSize, totalItems);
    }

    private async Task<PaginatedResponse<TransactionModel>> GetDatabaseUniqueTransactionsAsync(GetComparisionResultsRequest request, CancellationToken cancellationToken)
    {
        var totalItems = await context.Transactions
            .AsNoTracking()
            .Include(x => x.CsvTransactions)
            .Where(x => !x.CsvTransactions.Any(ct => ct.CsvFileHash == request.FileHash))
            .CountAsync(cancellationToken);

        var transactions = await context.Transactions
            .AsNoTracking()
            .Include(x => x.CsvTransactions)
            .Where(x => !x.CsvTransactions.Any(ct => ct.CsvFileHash == request.FileHash))
            .Skip(request.PageNumber * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new TransactionModel
            {
                Amount = x.Amount,
                Description = x.Description,
                ProcessedAt = x.ProcessedAt,
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TransactionModel>(transactions, request.PageNumber, request.PageSize, totalItems);
    }
}
