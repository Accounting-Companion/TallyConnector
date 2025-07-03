using TallyConnector.Abstractions.Attributes;

namespace TallyConnector.Models.TallyPrime.V6.Masters;
[XmlType(AnonymousType = true)]
[XmlRoot("GROUP")]
//[ImplementTallyRequestableObject]
[GenerateITallyRequestableObect]
[GenerateMeta]
public partial  class Group : Base.Masters.Group
{
}
