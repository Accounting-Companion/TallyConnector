using TallyConnector.Models.Base.Masters;
using TallyConnector.Models.Common;

namespace TallyConnector.Models.TallyPrime.V6.Masters;
[XmlType(AnonymousType = true)]
[XmlRoot("GROUP")]
[ImplementTallyRequestableObject]
public partial  class Group : BaseGroup
{
}
