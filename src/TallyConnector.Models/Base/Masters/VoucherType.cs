using TallyConnector.Models.Common;

namespace TallyConnector.Models.Base.Masters;

[XmlRoot(ElementName = "VOUCHERTYPE")]
[XmlType(AnonymousType = true)]
[TDLObjectsMethodName(nameof(GetVoucherTypeObjects))]
public partial class VoucherType : BaseAliasedMasterObject
{
    [XmlElement(ElementName = "PARENT")]
    public string Parent { get; set; }

    [XmlElement(ElementName = "NUMBERINGMETHOD")]
    public string? NumberingMethod { get; set; }

    [XmlElement(ElementName = "USEZEROENTRIES")]
    public bool? UseZeroEntries { get; set; }

    [XmlElement(ElementName = "ISACTIVE")]
    public bool? IsActive { get; set; }

    [XmlElement(ElementName = "PRINTAFTERSAVE")]
    public bool? PrintAfterSave { get; set; }

    [XmlElement(ElementName = "USEFORPOSINVOICE")]
    public bool? UseforPOSInvoice { get; set; }

    [XmlElement(ElementName = "VCHPRINTBANKNAME")]
    public string? VchPrintBankName { get; set; }

    [XmlElement(ElementName = "VCHPRINTTITLE")]
    public string? VchPrintTitle { get; set; }

    [XmlElement(ElementName = "TAXUNITNAME")]
    public string? TaxUnitName { get; set; }

    [XmlElement(ElementName = "VCHPRINTJURISDICTION")]
    public string? VchPrintJurisdiction { get; set; }

    [XmlElement(ElementName = "ISOPTIONAL")]
    public bool? IsOptional { get; set; }

    [XmlElement(ElementName = "COMMONNARRATION")]
    public bool? CommonNarration { get; set; }

    [XmlElement(ElementName = "MULTINARRATION")]
    public bool? MultiNarration { get; set; }  //Narration for each Ledger

    [XmlElement(ElementName = "ISDEFAULTALLOCENABLED")]
    public bool? IsDefaultAllocationEnabled { get; set; }

    [XmlElement(ElementName = "AFFECTSSTOCK")]
    public bool? EffectStock { get; set; }

    [XmlElement(ElementName = "ASMFGJRNL")]
    public bool? AsMfgJrnl { get; set; }

    [XmlElement(ElementName = "USEFORJOBWORK")]
    public bool? UseforJobwork { get; set; }

    [XmlElement(ElementName = "ISFORJOBWORKIN")]
    public bool? IsforJobworkIn { get; set; }

    [XmlElement(ElementName = "VOUCHERCLASSLIST.LIST")]
    [TDLCollection(CollectionName = "VOUCHERCLASSLIST")]
    public List<VoucherClass>? VoucherClasses { get; set; }

    [XmlElement(ElementName = "DEFAULTVOUCHERCATEGORY")]
    public DefaultVoucherCategory? DefaultVoucherCategory { get; set; }

    [XmlElement(ElementName = "COREVOUCHERTYPE")]
    public CoreVoucherType? CoreVoucherType { get; set; }


    [XmlElement(ElementName = "CANDELETE")]
    public bool? CanDelete { get; set; }

    public static TallyCustomObject[] GetVoucherTypeObjects()
    {
        const string CoreVchtypeFormulae1 = "CoreVoucherType:(if $$IsSales:$NAME then \"Sales\" else if $$IsPurchase:$NAME then \"Purchase\" else if $$IsDebitNote:$Name then \"DebitNote\" else if $$IsCreditNote:$Name then \"CreditNote\" else if $$IsPayment:$Name then \"Payment\" else if $$IsReceipt:$Name then \"Receipt\" else if  $$IsContra:$Name then \"Contra\" else if  $$IsJournal:$Name then \"Journal\" else if  $$IsSalesOrder:$Name then \"SalesOrder\" else if  $$IsPurcOrder:$Name then \"PurchaseOrder\" else if  $$IsMemo:$Name then \"Memo\" else \"\") +  ";
        const string CoreVchtypeFormulae2 = "(if  $$IsRevJrnl:$Name then \"Reversing Journal\"  else if $$IsJobMaterialReceive:$NAME then \"MaterialIn\" else if  $$IsJobMaterialIssue:$Name then \"MaterialOut\" else if  $$IsJobOrderIn:$Name then \"JobWork In Order\" else if  $$IsJobOrderOut:$Name then \"JobWork Out Order\" else if  $$IsRcptNote:$Name then \"ReceiptNote\" else \"\") + ";
        const string CoreVchtypeFormulae3 = "(if  $$IsDelNote:$Name then \"DeliveryNote\" else if  $$IsPhysStock:$Name then \"PhysicalStock\" else if  $$IsPayroll:$Name then \"Payroll\" else if  $$IsAttendance:$Name then \"Attendance\" else if  $$IsRejIn:$Name then \"RejectionsIn\"  else if  $$IsRejOut:$Name then \"RejectionsOut\" else if  $$IsStockJrnl:$Name then \"StockJournal\" else \"\")";

        return [new TallyCustomObject("VoucherType", [
            "DefaultVoucherCategory:if $$IsAccountingVch:$NAME then \"AccountingVch\" else " +
                                   "if $$IsInventoryVch:$NAME then \"InventoryVch\" " +
                                   "else if $$IsOrderVch:$Name then \"OrderVch\" else " +
                                   "if $$IsPayrollVch:$Name then \"PayrollVch\" else " +
                                   "if $$IsAttendance:$Name then \"PayrollAttndVch\" else \"\"",

                                   CoreVchtypeFormulae1 + CoreVchtypeFormulae2 + CoreVchtypeFormulae3]) { IsModify = YesNo.Yes }];
    }
}
public partial class VoucherClass
{
    [XmlElement(ElementName = "CLASSNAME")]
    public string ClassName { get; set; }

    [XmlElement(ElementName = "POSCARDLEDGER")]
    public string? POSCardLedger { get; set; }

    [XmlElement(ElementName = "POSCASHLEDGER")]
    public string? POSCashLedger { get; set; }

    [XmlElement(ElementName = "POSGIFTLEDGER")]
    public string? POSGiftLedger { get; set; }

    [XmlElement(ElementName = "POSCHEQUELEDGER")]
    public string? POSChequeLedger { get; set; }

    [XmlElement(ElementName = "FORJOBCOSTING")]
    public bool? ForJobCosting { get; set; }

    [XmlElement(ElementName = "USEFORINTEREST")]
    public bool? UseforInterest { get; set; }

    [XmlElement(ElementName = "USEFORGAINLOSS")]
    public bool? UseforGainLoss { get; set; }

    [XmlElement(ElementName = "USEFORGODOWNTRANSFER")]
    public bool UseforGodownTransfer { get; set; }

    [XmlElement(ElementName = "USEFORCOMPOUND")]
    public bool? UseforCompound { get; set; }

    [XmlElement(ElementName = "CLASSFORVAT")]
    public bool? ClassforVAT { get; set; }

    [XmlElement(ElementName = "USEFORFBT")]
    public bool? UseforFBT { get; set; }

    [XmlElement(ElementName = "POSENABLECARDLEDGER")]
    public bool? POSEnableCardLedger { get; set; }

    [XmlElement(ElementName = "POSENABLECASHLEDGER")]
    public bool? POSEnableCashLedger { get; set; }

    [XmlElement(ElementName = "POSENABLEGIFTLEDGER")]
    public bool? POSEnableGiftLedger { get; set; }

    [XmlElement(ElementName = "POSENABLECHEQUELEDGER")]
    public bool? PosEnableChequeLedger { get; set; }

    [XmlElement(ElementName = "USEFOREXCISECOMMERCIALINVOICE")]
    public bool? UseforExcisECommercialInvoice { get; set; }

    [XmlElement(ElementName = "USEFORSERVICETAX")]
    public bool? UseforServiceTax { get; set; }

    [XmlElement(ElementName = "CLASSFOREXCISE")]
    public bool? ClassforExcise { get; set; }

    [XmlElement(ElementName = "CLASSFORDEALEREXCISESHORTAGE")]
    public bool? ClassforDealerExciseShortage { get; set; }

    [XmlElement(ElementName = "POSENABLEONACCOUNTLEDGER")]
    public bool? POSEnableOnAccountLedger { get; set; }

    [XmlElement(ElementName = "USEBANKALLOCFORCC")]
    public bool? UseBankAllocforcc { get; set; }

    [XmlElement(ElementName = "ISDEFAULTCLASS")]
    public bool? IsDefaultClass { get; set; }

    [XmlElement(ElementName = "ADJDIFFINFIRSTLEDGER")]
    public bool? AdjDiffinFirstLedger { get; set; }

    [XmlElement(ElementName = "ADJDIFFINFIRSTLEDGERITEM")]
    public bool? AdjDiffinFirstLedgerItem { get; set; }

    [XmlElement(ElementName = "LEDGERFORINVENTORYLIST.LIST")]
    [TDLCollection(CollectionName = "LEDGERFORINVENTORYLIST")]
    public List<VoucherClassLedger>? LedgersforInventory { get; set; }

    [XmlElement(ElementName = "LEDGERENTRIESLIST.LIST")]
    [TDLCollection(CollectionName = "LEDGERENTRIESLIST")]
    public List<VoucherClassLedger>? LedgerEntries { get; set; }

    [XmlElement(ElementName = "DEFAULTACCALLOCFORITEM.LIST")]
    [TDLCollection(CollectionName = "DEFAULTACCALLOCFORITEM")]
    public List<DefaultAllocforItem>? StockEntries { get; set; }

}
public partial class VoucherClassLedger
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    [XmlElement(ElementName = "ROUNDTYPE")]
    public RoundType? RoundType { get; set; }

    [XmlElement(ElementName = "GSTCLASSIFICATIONNATURE")]
    public string? GSTClassificationNature { get; set; }

    [XmlElement(ElementName = "METHODTYPE")]
    public CalculationMethod? MethodType { get; set; }

    [XmlElement(ElementName = "CLASSRATE")]
    public string? ClassRate { get; set; }

    /// <summary>
    /// Override using Item Default
    /// </summary>
    [XmlElement(ElementName = "LEDGERFROMITEM")]
    public bool LedgerfromItem { get; set; }

    [XmlElement(ElementName = "REMOVEZEROENTRIES")]
    public bool RemoveZeroEntries { get; set; }

    [XmlElement(ElementName = "ROUNDLIMIT")]
    [Column(TypeName = "decimal(10,4)")]
    public decimal? Roundlimit { get; set; }
}

public partial class DefaultAllocforItem
{
    [XmlElement(ElementName = "STOCKITEMNAME")]
    public string StockItemName { get; set; }

    /// <summary>
    /// Override using Item Default
    /// </summary>
    [XmlElement(ElementName = "LEDGERFROMITEM")]
    public bool LedgerfromItem { get; set; }

    [XmlElement(ElementName = "DEFAULTACCALLOCFORITEM.LIST")]
    [TDLCollection(CollectionName = "DEFAULTACCALLOCFORITEM")]
    public List<VoucherClassLedger>? LedgerEntries { get; set; }
}
public enum CalculationMethod
{
    [EnumXMLChoice(Choice = "")]
    None,
    [EnumXMLChoice(Choice = "GST")]
    GST = 1,
    [EnumXMLChoice(Choice = "TCS")]
    TCS = 2,
    [EnumXMLChoice(Choice = "TDS")]
    TDS = 3,
    [EnumXMLChoice(Choice = "Excise")]
    Excise = 4,
    [EnumXMLChoice(Choice = "FBT")]
    FBT = 5,
    [EnumXMLChoice(Choice = "Service Tax")]
    ServiceTax = 6,
    [EnumXMLChoice(Choice = "VAT")]
    VAT = 7,
    [EnumXMLChoice(Choice = "Default")]
    Default = 8,
    [EnumXMLChoice(Choice = "CST")]
    CST = 9,
    [EnumXMLChoice(Choice = "CENVAT")]
    CENVAT = 10,
    [EnumXMLChoice(Choice = "Krishi Kalyan Cess")]
    KrishiKalyanCess = 11,
    [EnumXMLChoice(Choice = "Swachh Bharat Cess")]
    SwachhBharatCess = 12,
    [EnumXMLChoice(Choice = "Additional Tax")]
    AdditionalTax,
    [EnumXMLChoice(Choice = "Surcharge On VAT")]
    SurchargeOnVAT,
    [EnumXMLChoice(Choice = "Cess On VAT")]
    CessOnVAT,
    [EnumXMLChoice(Choice = "NHIL")]
    NHIL,
    [EnumXMLChoice(Choice = "On Item Rate")]
    OnItemRate,
    [EnumXMLChoice(Choice = "On Total Sales")]
    OnTotalSales,
    [EnumXMLChoice(Choice = "On Current SubTotal")]
    OnCurrentSubTotal,
    [EnumXMLChoice(Choice = "As Surcharge")]
    AsSurcharge,
    [EnumXMLChoice(Choice = "As Additional Excise")]
    AsExciseSurcharge,
    [EnumXMLChoice(Choice = "Based on Quantity")]
    OnQuantity,
    [EnumXMLChoice(Choice = "As Flat Rate")]
    AsFlatRate,
    [EnumXMLChoice(Choice = "As User Defined Value")]
    AsUserDefined,
    [EnumXMLChoice(Choice = "As Total Amount Rounding")]
    AsRounding,
    [EnumXMLChoice(Choice = "On VAT Rate")]
    OnVATRate,
    [EnumXMLChoice(Choice = "On Sales Tax Rate")]
    OnSalesTaxRate
}

public enum DefaultVoucherCategory
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "AccountingVch")]
    AccountingVch = 1,
    [EnumXMLChoice(Choice = "InventoryVch")]
    InventoryVch = 2,
    [EnumXMLChoice(Choice = "OrderVch")]
    OrderVch = 3,
    [EnumXMLChoice(Choice = "PayrollVch")]
    PayrollVch = 4,
    [EnumXMLChoice(Choice = "PayrollAttndVch")]
    PayrollAttndVch = 5,
}

public enum CoreVoucherType
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Sales")]
    Sales = 1,
    [EnumXMLChoice(Choice = "Purchase")]
    Purchase = 2,
    [EnumXMLChoice(Choice = "DebitNote")]
    DebitNote = 3,
    [EnumXMLChoice(Choice = "CreditNote")]
    CreditNote = 4,
    [EnumXMLChoice(Choice = "Payment")]
    Payment = 5,
    [EnumXMLChoice(Choice = "Receipt")]
    Receipt = 6,
    [EnumXMLChoice(Choice = "Contra")]
    Contra = 7,
    [EnumXMLChoice(Choice = "Journal")]
    Journal = 8,
    [EnumXMLChoice(Choice = "SalesOrder")]
    SalesOrder = 9,
    [EnumXMLChoice(Choice = "PurchaseOrder")]
    PurchaseOrder = 10,
    [EnumXMLChoice(Choice = "Memo")]
    Memo = 11,
    [EnumXMLChoice(Choice = "Reversing Journal")]
    ReversingJournal = 12,
    [EnumXMLChoice(Choice = "MaterialIn")]
    MaterialIn = 13,
    [EnumXMLChoice(Choice = "MaterialOut")]
    MaterialOut = 14,
    [EnumXMLChoice(Choice = "JobWork In Order")]
    JobWorkInOrder = 15,
    [EnumXMLChoice(Choice = "JobWork Out Order")]
    JobWorkOutOrder = 16,
    [EnumXMLChoice(Choice = "ReceiptNote")]
    ReceiptNote = 17,
    [EnumXMLChoice(Choice = "DeliveryNote")]
    DeliveryNote = 18,
    [EnumXMLChoice(Choice = "PhysicalStock")]
    PhysicalStock = 19,
    [EnumXMLChoice(Choice = "Payroll")]
    Payroll = 20,
    [EnumXMLChoice(Choice = "Attendance")]
    Attendance = 21,
    [EnumXMLChoice(Choice = "RejectionsIn")]
    RejectionsIn = 22,
    [EnumXMLChoice(Choice = "RejectionsOut")]
    RejectionsOut = 23,
    [EnumXMLChoice(Choice = "StockJournal")]
    StockJournal = 24,
}
