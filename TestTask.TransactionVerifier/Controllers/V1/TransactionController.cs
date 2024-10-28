using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestTask.TransactionVerifier.BusinessLogic.Requests;
using TestTask.TransactionVerifier.BusinessLogic.Services.Abstractions;
using TestTask.TransactionVerifier.Common.Extensions;
using TestTask.TransactionVerifier.WebApi.Controllers.Base;

namespace TestTask.TransactionVerifier.WebApi.Controllers.V1;

public class TransactionController(ICsvProcessingService csvProcessingService, ICsvFileService csvFileService)
    : ApiControllerVersionedBase
{
    [HttpGet("get-all-files")]
    public async Task<IActionResult> GetAllFiles(CancellationToken cancellationToken)
    {
        return Ok(await csvFileService.GetAllFilesAsync(cancellationToken));
    }

    [HttpPost("process-transactions")]
    public async Task<IActionResult> UploadCsv(IFormFile file, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var fileHash = await file.GetFileMd5HashAsync();

        var isFileProcessed = await csvFileService.IsCsvFileProcessed(fileHash, cancellationToken);

        if (isFileProcessed)
            return Ok($"File already processed. Hash: {await file.GetFileMd5HashAsync()}");

        await csvFileService.AddFileAsync(file, fileHash, cancellationToken);

        await csvProcessingService.ProcessTransactions(file, fileHash, cancellationToken);

        stopwatch.Stop();
        return Ok(stopwatch.Elapsed);
    }

    [HttpGet("get-comparision-results")]
    public async Task<IActionResult> GetResult([FromQuery] GetComparisionResultsRequest request, CancellationToken cancellationToken)
    {
        return Ok(await csvProcessingService.GetComparisionResultAsync(request, cancellationToken));
    }
}
