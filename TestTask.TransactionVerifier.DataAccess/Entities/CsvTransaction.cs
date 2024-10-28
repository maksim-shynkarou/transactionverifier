using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace TestTask.TransactionVerifier.DataAccess.Entities;

    public class CsvTransaction
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime ProcessedAt { get; set; }

    //public TransactionComparisionType TransactionComparisionType { get; set; }

    public string Description { get; set; }

    public virtual CsvFile CsvFile { get; set; }

    public string CsvFileHash { get; set; }

    public Guid? TransactionId { get; set; }

    public Transaction? Transaction { get; set; }
}

internal class CsvTransactionConfiguration : IEntityTypeConfiguration<CsvTransaction>
{
    public void Configure(EntityTypeBuilder<CsvTransaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.Description).HasMaxLength(255);
    }
}

