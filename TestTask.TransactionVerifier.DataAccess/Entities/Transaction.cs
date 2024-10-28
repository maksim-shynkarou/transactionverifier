using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace TestTask.TransactionVerifier.DataAccess.Entities;

public class Transaction
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime ProcessedAt { get; set; }

    public string Description { get; set; }


    public ICollection<CsvTransaction> CsvTransactions { get; set; } = new List<CsvTransaction>();
}

internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder
            .HasMany(t => t.CsvTransactions)
            .WithOne(ct => ct.Transaction)
            .HasForeignKey(ct => ct.TransactionId);
    }
}
