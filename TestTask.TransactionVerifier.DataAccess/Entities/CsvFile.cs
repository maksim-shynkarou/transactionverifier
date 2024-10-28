using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace TestTask.TransactionVerifier.DataAccess.Entities;

public class CsvFile
{
    public string Hash { get; set; } = null!;

    public DateTime ProcessedAt { get; set; }

    public string FileName { get; set; } = null!;

    public ICollection<CsvTransaction> CsvTransactions { get; set; } = new List<CsvTransaction>();
}

internal class CsvFileConfiguration : IEntityTypeConfiguration<CsvFile>
{
    public void Configure(EntityTypeBuilder<CsvFile> builder)
    {
        builder.HasKey(t => t.Hash);
        builder.Property(t => t.Hash).HasMaxLength(1000);
        builder.Property(t => t.FileName).HasMaxLength(255);

        builder
            .HasMany(o => o.CsvTransactions)
            .WithOne(c => c.CsvFile)
            .HasForeignKey(o => o.CsvFileHash);

        
    }
}
