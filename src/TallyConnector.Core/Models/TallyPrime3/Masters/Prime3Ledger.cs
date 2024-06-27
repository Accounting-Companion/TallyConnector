using TallyConnector.Core.Models.Masters;

namespace TallyConnector.Core.Models.TallyPrime3.Masters;
[XmlRoot("LEDGER")]
[XmlType(AnonymousType = true)]
[TDLCollection(Type = "Ledger")]
public partial class Prime3Ledger : Ledger
{

}
