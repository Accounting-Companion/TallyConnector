using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Interfaces;
using TallyConnector.Core.Models.TallyComplexObjects;

namespace CustomService.Models;
[XmlRoot(ElementName = "VOUCHER", Namespace = "")]
public class Voucher : ITallyBaseObject
{
    [XmlElement(ElementName = "DATE")]
    public DateTime Date { get; set; }

    [XmlElement(ElementName = "VOUCHERTYPENAME")]
    public string VoucherType { get; set; }

    [XmlElement(ElementName = "PERSISTEDVIEW")]
    public TallyConnector.Core.Models.VoucherViewType View { get; set; }

    [XmlElement(ElementName = "VOUCHERNUMBER")]
    public string? VoucherNumber { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    public string? Narration { get; set; }

    [XmlElement(ElementName = "PARTYNAME")]
    public string? PartyName { get; set; }

    [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST")]
    public List<LedgerEntry> LedgerEntries { get; set; }
}

public class LedgerEntry : IBaseLedgerEntry
{
    [XmlElement(ElementName = "LEDGERNAME")]
    public string LedgerName { get; set; } = null!;

    [XmlElement(ElementName = "AMOUNT")]
    public TallyConnector.Core.Models.TallyComplexObjects.TallyAmountField Amount { get; set; } = null!;
    [XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
    public List<BillAllocation> BillAllocations { get; set; }

}

[XmlRoot(ElementName = "BILLALLOCATIONS.LIST")]
public class BillAllocation : ITallyBaseObject
{
    [XmlElement(ElementName = "BILLTYPE")]
    public string? BillType { get; set; }

    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmountField Amount { get; set; }
}
