using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TestTask.TransactionVerifier.Common.Enums;
using TestTask.TransactionVerifier.Common.Extensions;
using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.DataAccess.Contexts.Abstractions;
using TestTask.TransactionVerifier.DataAccess.Entities;
using TestTask.TransactionVerifier.WebApi.Requests;
using TestTask.TransactionVerifier.WebApi.Responses;
using TestTask.TransactionVerifier.WebApi.Services.Interfaces;

namespace TestTask.TransactionVerifier.WebApi.Services.Implementations;

public class CsvTransactionService(ITransactionVerifierDbContext context, ITransactionService transactionService) : ICsvProcessingService
{

    public async Task ProcessTransactions(IFormFile? csvFile, CancellationToken cancellationToken)
    {
        if ((csvFile?.Length ?? 0) == 0)
        {
            throw new Exception("File is empty or not provided.");
        }

        var fileHash = await csvFile!.GetFileMd5HashAsync();

        var csvTransactions = await GetCsvTransactionModelsFromCsvFile(csvFile);

        var request = new ProcessDataRequest
        {
            CsvTransactions = csvTransactions,
            FileHash = fileHash
        };

        var csvUniqeTransactions = await transactionService.ProcessData(request, cancellationToken);

        foreach (var csvTransaction in csvUniqeTransactions)
        {
            var dbCsvTransaction = new CsvTransaction()
            {
                Amount = csvTransaction.Amount,
                ProcessedAt = csvTransaction.ProcessedAt,
                CsvFileHash = fileHash,
                Description = csvTransaction.Description
            };

            await context.CsvTransactions.AddAsync(dbCsvTransaction, cancellationToken);
            
        }

        await context.SaveChangesAsync(cancellationToken);
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
