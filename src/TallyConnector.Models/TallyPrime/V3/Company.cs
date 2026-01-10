namespace TallyConnector.Models.TallyPrime.V3;

[XmlRoot(ElementName = "COMPANY")]
[XmlType(AnonymousType = true)]
[GenerateITallyRequestableObect(Abstractions.Models.GenerationMode.GetMultiple)]
[GenerateMeta]
public partial class Company : Base.Company
{
}
