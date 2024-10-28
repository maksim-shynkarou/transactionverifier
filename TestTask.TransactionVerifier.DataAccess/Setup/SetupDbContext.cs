using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestTask.TransactionVerifier.DataAccess.Contexts;
using TestTask.TransactionVerifier.DataAccess.Contexts.Abstractions;

namespace TestTask.TransactionVerifier.DataAccess.Setup;
public static class SetupDbContext
{
    public sealed class SetupDbContextOptions
    {
        public string ConnectionString{ get; set; }

        internal SetupDbContextOptions()
        {

        }
    }
    public static IServiceCollection RegisterDbContext(this IServiceCollection services, Action<SetupDbContextOptions> options)
    {
        var optionsData = new SetupDbContextOptions();
        options.Invoke(optionsData);

        services.AddDbContext<ITransactionVerifierDbContext, TransactionVerifierDbContext>(o =>
            o.UseSqlServer(optionsData.ConnectionString));

        return services;
    }
}
