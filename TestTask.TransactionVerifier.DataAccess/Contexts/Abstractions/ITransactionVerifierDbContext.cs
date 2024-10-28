using Microsoft.EntityFrameworkCore;
using TestTask.TransactionVerifier.DataAccess.Entities;

namespace TestTask.TransactionVerifier.DataAccess.Contexts.Abstractions;
public interface ITransactionVerifierDbContext
{
    DbSet<Transaction> Transactions { get; }
    DbSet<CsvFile> CsvFiles { get; }
    DbSet<CsvTransaction> CsvTransactions { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
