using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Models.Base.Masters;

namespace TallyConnector.Models.TallyPrime.V6.Masters;
[XmlType(AnonymousType = true)]
[XmlRoot("CURRENCY")]
[ImplementTallyRequestableObject]
public partial class Currency : BaseCurrency
{
}
