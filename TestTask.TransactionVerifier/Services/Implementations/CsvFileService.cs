using Microsoft.EntityFrameworkCore;
using TestTask.TransactionVerifier.Common.Extensions;
using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.DataAccess.Contexts.Abstractions;
using TestTask.TransactionVerifier.DataAccess.Entities;
using TestTask.TransactionVerifier.WebApi.Services.Interfaces;

namespace TestTask.TransactionVerifier.WebApi.Services.Implementations;

public class CsvFileService(ITransactionVerifierDbContext context) : ICsvFileService
{
    public async Task<bool> IsCsvFileProcessed(IFormFile file, CancellationToken cancellationToken)
    {
        var fileHash = await file.GetFileMd5HashAsync();

        return await context.CsvFiles
            .AsNoTracking()
            .AnyAsync(x => x.Hash == fileHash, cancellationToken: cancellationToken);
    }

    public async Task<CsvFile> AddFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var fileHash = await file.GetFileMd5HashAsync();

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
