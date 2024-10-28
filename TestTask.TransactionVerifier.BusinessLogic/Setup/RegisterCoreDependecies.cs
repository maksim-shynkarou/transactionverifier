using Microsoft.Extensions.DependencyInjection;
using TestTask.TransactionVerifier.BusinessLogic.Services;
using TestTask.TransactionVerifier.BusinessLogic.Services.Abstractions;

namespace TestTask.TransactionVerifier.BusinessLogic.Setup;

public static class RegisterCoreDependecies
{
    public static IServiceCollection RegisterCoreDependencies(this IServiceCollection services)
    {
        services.AddScoped<ICsvProcessingService, CsvTransactionService>();
        services.AddScoped<ICsvFileService, CsvFileService>();
        services.AddScoped<ITransactionService, TransactionService>();

        return services;
    }
}
