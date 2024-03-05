using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.TallyPrime3;
public class Prime3Voucher : Voucher
{
    [XmlElement(ElementName = "GSTREGISTRATION")]
    public GSTRegistration? GSTRegistration { get; set; }

    [XmlElement(ElementName = "DATE")]
    public new string Date { get; set; }

    [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST", Type = typeof(CLedgerEntry))]
    [XmlElement(ElementName = "LEDGERENTRIES.LIST", Type = typeof(CELedgerEntry))]
    public new List<CLedgerEntry>? Ledgers { get; set; }
}
[XmlRoot(ElementName = "ALLLEDGERENTRIES.LIST")]
[TDLCollection(CollectionName = "ALLLEDGERENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{Constants.Voucher.ViewType.AccountingVoucherView}")]
public class CLedgerEntry : LedgerEntry
{
    [XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
    [TDLCollection(CollectionName = "BILLALLOCATIONS")]
    public new  List<CBillAllocations>? BillAllocations { get; set; }
}

[XmlRoot(ElementName = "LEDGERENTRIES.LIST")]
[TDLCollection(CollectionName = "LEDGERENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{Constants.Voucher.ViewType.InvoiceVoucherView}")]
public class CELedgerEntry : CLedgerEntry
{

}
[XmlRoot(ElementName = "BILLALLOCATIONS.LIST")]
public class CBillAllocations : BillAllocations
{

}