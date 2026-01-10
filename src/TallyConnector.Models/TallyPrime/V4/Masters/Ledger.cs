using TallyConnector.Models.Common;

namespace TallyConnector.Models.TallyPrime.V4.Masters;

[XmlType(AnonymousType = true)]
[XmlRoot("LEDGER")]
public partial class Ledger : Base.Masters.Ledger
{
    [XmlElement(ElementName = "CONTACTDETAILS.LIST")]
    [TDLCollection(CollectionName = "CONTACTDETAILS", ExplodeCondition = "$$NUMITEMS:CONTACTDETAILS>0")]
    public List<ContactDetail>? ContactDetails { get; set; }
}
