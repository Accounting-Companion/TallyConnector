using TallyConnector.Core.Models.TallyComplexObjects;

namespace TallyConnector.Core.Models.Interfaces.Voucher;
public interface IBaseLedgerEntry
{
    string LedgerName { get; set; }
    TallyAmountField? Amount { get; set; }
}
