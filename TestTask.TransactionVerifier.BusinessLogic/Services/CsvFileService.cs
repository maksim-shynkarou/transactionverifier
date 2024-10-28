using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TestTask.TransactionVerifier.BusinessLogic.Services.Abstractions;
using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.DataAccess.Contexts.Abstractions;
using TestTask.TransactionVerifier.DataAccess.Entities;

namespace TestTask.TransactionVerifier.BusinessLogic.Services;

internal class CsvFileService(ITransactionVerifierDbContext context) : ICsvFileService
{
    public async Task<bool> IsCsvFileProcessed(string fileHash, CancellationToken cancellationToken)
    {
        return await context.CsvFiles
            .AsNoTracking()
            .AnyAsync(x => x.Hash == fileHash, cancellationToken: cancellationToken);
    }

    public async Task<CsvFile> AddFileAsync(IFormFile file, string fileHash, CancellationToken cancellationToken)
    {
        var dbFile = new CsvFile
        {
            FileName = file.FileName,
            Hash = fileHash,
            ProcessedAt = DateTime.UtcNow
        };

        await context.CsvFiles.AddAsync(dbFile, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return dbFile;
    }

    public async Task<List<CsvFileModel>> GetAllFilesAsync(CancellationToken cancellationToken)
    {
        return await context.CsvFiles
            .AsNoTracking()
            .Select(x =>
            new CsvFileModel
            {
                FileHash = x.Hash,
                FileName = x.FileName,
                ProcessedAt = x.ProcessedAt
            })
            .ToListAsync(cancellationToken);
    }
}
