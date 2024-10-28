using TestTask.TransactionVerifier.Common.Models;

namespace TestTask.TransactionVerifier.Common.Comparers;
public class TransactionComparer
{
    public int Compare(TransactionModel x, TransactionModel y)
    {
        if (x == null || y == null)
        {
            throw new ArgumentNullException("Objects being compared cannot be null");
        }

        var amountComparison = x.Amount.CompareTo(y.Amount);
        if (amountComparison != 0)
            return amountComparison;
        

        var descriptionComparison = string.Compare(x.Description, y.Description, StringComparison.CurrentCultureIgnoreCase);
        if (descriptionComparison != 0)
            return descriptionComparison;
        
        return x.ProcessedAt.CompareTo(y.ProcessedAt);
    }
}
