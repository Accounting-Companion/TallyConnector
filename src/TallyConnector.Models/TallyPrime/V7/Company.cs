namespace TallyConnector.Models.TallyPrime.V7;
[XmlRoot(ElementName = "COMPANY")]
[XmlType(AnonymousType = true)]
[GenerateITallyRequestableObect(Abstractions.Models.GenerationMode.GetMultiple)]
[GenerateMeta]
public partial class Company : V6.Company
{
}
