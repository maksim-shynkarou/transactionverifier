using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.DataAccess.Entities;

namespace TestTask.TransactionVerifier.WebApi.Services.Interfaces;

public interface ICsvFileService
{
    Task<List<CsvFileModel>> GetAllFilesAsync(CancellationToken cancellationToken);

    Task<bool> IsCsvFileProcessed(string fileHash, CancellationToken cancellationToken);

    Task<CsvFile> AddFileAsync(IFormFile file, string fileHash, CancellationToken cancellationToken);
}
