using TallyConnector.Abstractions.Attributes;
using TallyConnector.Abstractions.Models;

namespace TallyConnector.Models.TallyPrime.V6.Masters;

[XmlType(AnonymousType = true)]
[XmlRoot("LEDGER")]
[GenerateITallyRequestableObect]
[GenerateMeta]
public partial class Ledger : Base.Masters.Ledger
{
}

