using System.Text.Json.Serialization;
using TallyConnector.Models.Common;

namespace TallyConnector.Models.Base;
[XmlRoot(ElementName = "VOUCHER")]
[XmlType(AnonymousType = true)]
[TDLCollection(Type = "Voucher")]
public partial class Voucher : BaseTallyObject
{
    [XmlElement(ElementName = "DATE")]
    public DateTime Date { get; set; }

    [XmlElement(ElementName = "EFFECTIVEDATE")]
    public DateTime? EffectiveDate { get; set; }

    [XmlElement(ElementName = "REFERENCEDATE")]
    public DateTime? ReferenceDate { get; set; }

    [XmlElement(ElementName = "REFERENCE")]
    [TDLField(FetchText = "REFERENCE", ExcludeInFetch = true)]
    public string? Reference { get; set; }

    [XmlElement(ElementName = "VOUCHERTYPENAME")]
    public string VoucherType { get; set; }

    [XmlElement(ElementName = "PERSISTEDVIEW")]
    public VoucherViewType View { get; set; }

    [XmlElement(ElementName = "VCHGSTCLASS")]
    public string? VoucherGSTClass { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRE")]
    public bool? IsCostCentre { get; set; }

    [XmlElement(ElementName = "COSTCENTRENAME")]
    public string? CostCentreName { get; set; }

    [XmlElement(ElementName = "VCHENTRYMODE")]
    public string? VoucherEntryMode { get; set; }

    [XmlElement(ElementName = "ISINVOICE")]
    public bool IsInvoice { get; set; }

    [XmlElement(ElementName = "VOUCHERNUMBER")]
    public string? VoucherNumber { get; set; }

    [XmlElement(ElementName = "ISOPTIONAL")]
    public bool IsOptional { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    public string? Narration { get; set; }

    [XmlElement(ElementName = "PRICELEVEL")]
    public string? PriceLevel { get; set; }

    //E-Invoice Details
    //[TallyCategory(Constants.Voucher.Category.EInvoiceDetails)]
    [XmlElement(ElementName = "BILLTOPLACE")]
    public string? BillToPlace { get; set; }

    //[TallyCategory(Constants.Voucher.Category.EInvoiceDetails)]
    [XmlElement(ElementName = "IRN")]
    public string? IRN { get; set; }

    // [TallyCategory(Constants.Voucher.Category.EInvoiceDetails)]
    [XmlElement(ElementName = "IRNACKNO")]
    public string? IRNAckNo { get; set; }

    //[TallyCategory(Constants.Voucher.Category.EInvoiceDetails)]
    [XmlElement(ElementName = "IRNACKDATE")]
    public string? IRNAckDate { get; set; }





    [TallyCategory("DispatchDetails")]
    [XmlElement(ElementName = "DISPATCHFROMNAME")]
    public string? DispatchFromName { get; set; }

    [TallyCategory("DispatchDetails")]
    [XmlElement(ElementName = "DISPATCHFROMSTATENAME")]
    public string? DispatchFromStateName { get; set; }

    [TallyCategory("DispatchDetails")]
    [XmlElement(ElementName = "DISPATCHFROMPINCODE")]
    public string? DispatchFromPinCode { get; set; }

    [TallyCategory("DispatchDetails")]
    [XmlElement(ElementName = "DISPATCHFROMPLACE")]
    public string? DispatchFromPlace { get; set; }

    [TallyCategory("ShippingDetails")]
    [TDLCollection(CollectionName = "INVOICEDELNOTES", ExplodeCondition = $"$$NumItems:INVOICEDELNOTES > 0")]
    [XmlElement(ElementName = "DELIVERYNOTES")]
    public DeliveryNotes? DeliveryNotes { get; set; }

    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "BASICSHIPDOCUMENTNO")]
    public string? DispatchDocNo { get; set; }
    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "BASICSHIPPEDBY")]
    public string? BasicShippedBy { get; set; }
    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "BASICFINALDESTINATION")]
    public string? Destination { get; set; }

    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "EICHECKPOST")]
    public string? CarrierName { get; set; }

    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "BILLOFLADINGNO")]
    public string? BillofLandingNo { get; set; }

    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "BILLOFLADINGDATE")]
    public string? BillofLandingDate { get; set; }


    //Export Shipping Details
    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICPLACEOFRECEIPT")]
    public string? PlaceOfReceipt { get; set; }

    /// <summary>
    /// Vehicle or Ship or Flight Number
    /// </summary>
    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICSHIPVESSELNO")]
    public string? ShipOrFlightNo { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICPORTOFLOADING")]
    public string? LandingPort { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICPORTOFDISCHARGE")]
    public string? DischargePort { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICDESTINATIONCOUNTRY")]
    public string? DestinationCountry { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "SHIPPINGBILLNO")]
    public string? ShippingBillNo { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "SHIPPINGBILLDATE")]
    public string? ShippingBillDate { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "PORTCODE")]
    public string? PortCode { get; set; }

    //OrderDetails
    [TallyCategory("OrderDetails")]
    [XmlElement(ElementName = "BASICDUEDATEOFPYMT")]
    public string? BasicDueDateofPayment { get; set; }

    [TallyCategory("OrderDetails")]
    [XmlElement(ElementName = "BASICORDERREF")]
    public string? OrderReference { get; set; }



    //Party Details
    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PARTYNAME")]
    public string? PartyName { get; set; }

    [XmlElement(ElementName = "VOUCHERNUMBERSERIES")]
    public string? VoucherNumberSeries { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PARTYMAILINGNAME")]
    public string? PartyMailingName { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "STATENAME")]
    public string? State { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "COUNTRYOFRESIDENCE")]
    public string? Country { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "GSTREGISTRATIONTYPE")]
    public string? RegistrationType { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PARTYGSTIN")]
    public string? PartyGSTIN { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PLACEOFSUPPLY")]
    public string? PlaceOfSupply { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PARTYPINCODE")]
    public string? PINCode { get; set; }

    //Consignee Details
    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "BASICBUYERNAME")]
    public string? ConsigneeName { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEEMAILINGNAME")]
    public string? ConsigneeMailingName { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEESTATENAME")]
    public string? ConsigneeState { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEECOUNTRYNAME")]
    public string? ConsigneeCountry { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEEGSTIN")]
    public string? ConsigneeGSTIN { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEEPINCODE")]
    public string? ConsigneePinCode { get; set; }

    [XmlArray(ElementName = "ADDRESS.LIST")]
    [XmlArrayItem(ElementName = "ADDRESS")]
    [TDLCollection(CollectionName = "Address", ExplodeCondition = "$$NumItems:ADDRESS<1")]
    public List<string>? Address { get; set; }

    [XmlArray(ElementName = "BASICBUYERADDRESS.LIST")]
    [XmlArrayItem(ElementName = "BASICBUYERADDRESS")]
    [TDLCollection(CollectionName = "Address", ExplodeCondition = "$$NumItems:BASICBUYERADDRESS<1")]

    public List<string>? BuyerAddress { get; set; }

    [XmlElement(ElementName = "ISCANCELLED")]
    public bool? IsCancelled { get; set; }

    //EWAY Details
    [XmlElement(ElementName = "OVRDNEWAYBILLAPPLICABILITY")]
    public bool? OverrideEWayBillApplicability { get; set; }

    [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST", Type = typeof(AllLedgerEntry))]
    [XmlElement(ElementName = "LEDGERENTRIES.LIST", Type = typeof(LedgerEntry))]
    public List<AllLedgerEntry> LedgerEntries { get; set; } = [];

    [XmlElement(ElementName = "ALLINVENTORYENTRIES.LIST", Type = typeof(AllInventoryAllocations))]
    [XmlElement(ElementName = "INVENTORYENTRIES.LIST", Type = typeof(InventoryEntries))]
    public List<AllInventoryAllocations>? InventoryAllocations { get; set; } = [];

    public override string ToString()
    {
        return $"Voucher - {VoucherType}_{VoucherNumber}_{Date.ToShortDateString()}";
    }
}
public partial class BaseInventoryEntry
{
    [XmlArray("BASICUSERDESCRIPTION.LIST")]
    [XmlArrayItem(ElementName = "BASICUSERDESCRIPTION")]
    [TDLCollection(CollectionName = "BASICUSERDESCRIPTION")]
    public List<string> UserDescriptions { get; set; }

    [XmlElement(ElementName = "INDEXNUMBER")]
    public int IndexNumber { get; set; }

    [XmlElement(ElementName = "STOCKITEMNAME")]
    public string? StockItemName { get; set; }


    [XmlElement(ElementName = "BOMNAME")]
    public string? BOMName { get; set; }

    [XmlElement(ElementName = "ISSCRAP")]
    public bool? IsScrap { get; set; }


    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    public bool IsDeemedPositive { get; set; }

    [XmlElement(ElementName = "RATE")]
    public TallyRateField? Rate { get; set; }

    [XmlElement(ElementName = "ACTUALQTY")]
    public TallyQuantityField? ActualQuantity { get; set; }

    [XmlElement(ElementName = "BILLEDQTY")]
    public TallyQuantityField? BilledQuantity { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmountField? Amount { get; set; }
}

[XmlRoot(ElementName = "ALLINVENTORYENTRIES.LIST")]
[TDLCollection(CollectionName = "ALLINVENTORYENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{Core.Constants.Voucher.ViewType.InvoiceVoucherView}")]
public partial class AllInventoryAllocations : BaseInventoryEntry
{
    [XmlElement(ElementName = "ACCOUNTINGALLOCATIONS.LIST")]
    [TDLCollection(CollectionName = "ACCOUNTINGALLOCATIONS")]
    public List<BaseLedgerEntry>? Ledgers { get; set; } = [];
}

[XmlRoot(ElementName = "INVENTORYENTRIES.LIST")]
[TDLCollection(CollectionName = "INVENTORYENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{Core.Constants.Voucher.ViewType.MfgJournalVoucherView}")]
public partial class InventoryEntries : AllInventoryAllocations
{
}
public partial class BaseLedgerEntry
{
    public BaseLedgerEntry()
    {
    }


    [XmlElement(ElementName = "INDEXNUMBER")]
    public int IndexNumber { get; set; }

    [XmlElement(ElementName = "LEDGERNAME")]
    public string LedgerName { get; set; }

    [XmlElement(ElementName = "LEDGERTAXTYPE")]
    public string? LedgerTaxType { get; set; }

    [XmlElement(ElementName = "VCHLEDGERTYPE")]
    public string? LedgerType { get; set; }

    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    public bool IsDeemedPositive { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmountField Amount { get; set; }
}
[XmlRoot(ElementName = "ALLLEDGERENTRIES.LIST")]
[TDLCollection(CollectionName = "ALLLEDGERENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{TallyConnector.Core.Constants.Voucher.ViewType.AccountingVoucherView}")]
public partial class AllLedgerEntry : BaseLedgerEntry
{
    [XmlElement(ElementName = "ADDLALLOCTYPE")]
    public AdAllocType AdAllocType { get; set; }

    [XmlElement(ElementName = "ISPARTYLEDGER")]
    public bool IsPartyLedger { get; set; }

    [XmlElement(ElementName = "SWIFTCODE")]
    public string? SWIFTCode { get; set; }
}
[XmlRoot(ElementName = "LEDGERENTRIES.LIST")]
[TDLCollection(CollectionName = "LEDGERENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{Core.Constants.Voucher.ViewType.InvoiceVoucherView}")]
public partial class LedgerEntry : AllLedgerEntry
{
    public string? LedgerEntryProp { get; set; }

}
[XmlRoot(ElementName = "INVOICEDELNOTES.LIST")]
public partial class DeliveryNotes
{
    [XmlElement(ElementName = "BASICSHIPPINGDATE")]
    public DateTime? ShippingDate { get; set; }

    [XmlElement(ElementName = "BASICSHIPDELIVERYNOTE")]
    public string DeliveryNote { get; set; }
}
/// <summary>
/// <para>Voucher ViewTypes avavailable in Tally</para>
/// <para>TDL Reference -  src\voucher\vchreport.tdl
/// Search using "Set						: SVViewName"</para>
/// </summary>
public enum VoucherViewType
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Accounting Voucher View")]
    AccountingVoucherView = 1,

    [EnumXMLChoice(Choice = "Invoice Voucher View")]
    InvoiceVoucherView = 2,

    [EnumXMLChoice(Choice = "PaySlip Voucher View")]
    PaySlipVoucherView = 3,

    [EnumXMLChoice(Choice = "Multi Consumption Voucher View")]
    MultiConsumptionVoucherView = 4,

    [EnumXMLChoice(Choice = "Consumption Voucher View")]
    ConsumptionVoucherView = 5,
}