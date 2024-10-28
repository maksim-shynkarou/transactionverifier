using TestTask.TransactionVerifier.Common.Models;
using TestTask.TransactionVerifier.DataAccess.Entities;

namespace TestTask.TransactionVerifier.WebApi.Services.Interfaces;

public interface ICsvFileService
{
    Task<List<CsvFileModel>> GetAllFilesAsync(CancellationToken cancellationToken);

    Task<bool> IsCsvFileProcessed(IFormFile file, CancellationToken cancellationToken);

    Task<CsvFile> AddFileAsync(IFormFile file, CancellationToken cancellationToken);
}
