using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.TallyComplexObjects;

namespace CustomService.Models;
[TDLCollection(Type = "Bills")]
public class Bill : ITallyBaseObject
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "BILLDATE")]
    public DateTime BillDate { get; set; }

    [XmlElement(ElementName = "LEDGERNAME")]
    public string LedgerName { get; set; }

    [XmlElement(ElementName = "OPENINGBALANCE")]
    public TallyAmountField OpeningBalance { get; set; }

    [XmlElement(ElementName = "FINALBALANCE")]
    public TallyAmountField Balance { get; set; }


}
