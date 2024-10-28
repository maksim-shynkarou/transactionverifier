using Microsoft.EntityFrameworkCore;
using TestTask.TransactionVerifier.DataAccess.Contexts.Abstractions;
using TestTask.TransactionVerifier.DataAccess.Entities;

namespace TestTask.TransactionVerifier.DataAccess.Contexts;

internal class TransactionVerifierDbContext : DbContext, ITransactionVerifierDbContext
{

    public TransactionVerifierDbContext()
    {
    }

    public TransactionVerifierDbContext(DbContextOptions<TransactionVerifierDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<CsvFile> CsvFiles { get; set; }
    public DbSet<CsvTransaction> CsvTransactions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionVerifierDbContext).Assembly);

        modelBuilder.Entity<Transaction>().HasData(
            new Transaction { Id = Guid.NewGuid(), Amount = 100.50m, ProcessedAt = DateTime.Now.AddDays(-1), Description = "Transaction 1" },
            new Transaction { Id = Guid.NewGuid(), Amount = 200.75m, ProcessedAt = DateTime.Now.AddDays(-2), Description = "Transaction 2" },
            new Transaction { Id = Guid.NewGuid(), Amount = 50.25m, ProcessedAt = DateTime.Now.AddDays(-3), Description = "Transaction 3" },
            new Transaction { Id = Guid.NewGuid(), Amount = 300.00m, ProcessedAt = DateTime.Now.AddDays(-4), Description = "Transaction 4" },
            new Transaction { Id = Guid.NewGuid(), Amount = 450.00m, ProcessedAt = DateTime.Now.AddDays(-5), Description = "Transaction 5" },
            new Transaction { Id = Guid.NewGuid(), Amount = 120.45m, ProcessedAt = DateTime.Now.AddDays(-6), Description = "Transaction 6" },
            new Transaction { Id = Guid.NewGuid(), Amount = 320.85m, ProcessedAt = DateTime.Now.AddDays(-7), Description = "Transaction 7" },
            new Transaction { Id = Guid.NewGuid(), Amount = 150.95m, ProcessedAt = DateTime.Now.AddDays(-8), Description = "Transaction 8" },
            new Transaction { Id = Guid.NewGuid(), Amount = 510.65m, ProcessedAt = DateTime.Now.AddDays(-9), Description = "Transaction 9" },
            new Transaction { Id = Guid.NewGuid(), Amount = 60.75m, ProcessedAt = DateTime.Now.AddDays(-10), Description = "Transaction 10" },
            new Transaction { Id = Guid.NewGuid(), Amount = 80.00m, ProcessedAt = DateTime.Now.AddDays(-11), Description = "Transaction 11" },
            new Transaction { Id = Guid.NewGuid(), Amount = 90.50m, ProcessedAt = DateTime.Now.AddDays(-12), Description = "Transaction 12" },
            new Transaction { Id = Guid.NewGuid(), Amount = 225.35m, ProcessedAt = DateTime.Now.AddDays(-13), Description = "Transaction 13" },
            new Transaction { Id = Guid.NewGuid(), Amount = 180.60m, ProcessedAt = DateTime.Now.AddDays(-14), Description = "Transaction 14" },
            new Transaction { Id = Guid.NewGuid(), Amount = 275.80m, ProcessedAt = DateTime.Now.AddDays(-15), Description = "Transaction 15" },
            new Transaction { Id = Guid.NewGuid(), Amount = 310.25m, ProcessedAt = DateTime.Now.AddDays(-16), Description = "Transaction 16" },
            new Transaction { Id = Guid.NewGuid(), Amount = 480.75m, ProcessedAt = DateTime.Now.AddDays(-17), Description = "Transaction 17" },
            new Transaction { Id = Guid.NewGuid(), Amount = 620.85m, ProcessedAt = DateTime.Now.AddDays(-18), Description = "Transaction 18" },
            new Transaction { Id = Guid.NewGuid(), Amount = 750.45m, ProcessedAt = DateTime.Now.AddDays(-19), Description = "Transaction 19" },
            new Transaction { Id = Guid.NewGuid(), Amount = 130.00m, ProcessedAt = DateTime.Now.AddDays(-20), Description = "Transaction 20" }
        );
    }
}
