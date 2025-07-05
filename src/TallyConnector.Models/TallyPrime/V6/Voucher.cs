using System.Text.Json.Serialization;

namespace TallyConnector.Models.TallyPrime.V6;

[XmlRoot(ElementName = "VOUCHER")]
[XmlType(AnonymousType = true)]
[GenerateITallyRequestableObect]
[GenerateMeta]
public partial class Voucher :TallyConnector.Models.Base.Voucher
{
}

