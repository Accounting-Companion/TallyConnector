using System.Text.Json.Serialization;

namespace TallyConnector.Models.TallyPrime.V6;
[ImplementTallyRequestableObject]
[XmlRoot(ElementName = "VOUCHER")]
[XmlType(AnonymousType = true)]
public partial class Voucher :TallyConnector.Models.Base.Voucher
{
}

