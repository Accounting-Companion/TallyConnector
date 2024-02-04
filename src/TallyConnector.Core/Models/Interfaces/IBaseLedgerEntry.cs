using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallyConnector.Core.Models.TallyComplexObjects;

namespace TallyConnector.Core.Models.Interfaces;
public interface IBaseLedgerEntry
{
    string LedgerName { get; set; }
    TallyAmountField Amount { get; set; }
}
