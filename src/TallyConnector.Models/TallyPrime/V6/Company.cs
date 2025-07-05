using TallyConnector.Abstractions.Attributes;

namespace TallyConnector.Models.TallyPrime.V6;
[XmlRoot(ElementName = "COMPANY")]
[XmlType(AnonymousType = true)]
[GenerateITallyRequestableObect]
[GenerateMeta]
public partial class Company : Base.Company
{
}
