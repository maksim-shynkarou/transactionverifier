using Microsoft.AspNetCore.Http;

namespace TestTask.TransactionVerifier.Common.Extensions;
public static class FormFileExtensions
{
    public static async Task<string> GetFileMd5HashAsync(this IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        using (var md5 = System.Security.Cryptography.MD5.Create())
        using (var stream = file.OpenReadStream())
        {
            var hashBytes = await md5.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
