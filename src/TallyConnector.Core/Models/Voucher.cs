﻿using TallyConnector.Core.Models.Interfaces.Voucher;
using TallyConnector.Core.Models.TallyComplexObjects;

namespace TallyConnector.Core.Models;

[Serializable]
[XmlRoot(ElementName = "VOUCHER", Namespace = "")]
[TallyObjectType(TallyObjectType.Vouchers)]
[TDLCollection(Type = "Voucher")]
public class Voucher : TallyObject, IBaseObject, IBaseVoucherObject
{
    public Voucher()
    {
        _DeliveryNotes = new();
    }

    [XmlElement(ElementName = "DATE")]
    public DateTime Date { get; set; }

    [XmlElement(ElementName = "REFERENCEDATE")]
    public DateTime? ReferenceDate { get; set; }

    [XmlElement(ElementName = "REFERENCE")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Reference { get; set; }


    [XmlElement(ElementName = "VOUCHERTYPENAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string VoucherType { get; set; }


    [XmlElement(ElementName = "PERSISTEDVIEW")]
    [Column(TypeName = $"nvarchar(30)")]
    public VoucherViewType View { get; set; }

    [XmlElement(ElementName = "VCHGSTCLASS")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? VoucherGSTClass { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRE")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public bool? IsCostCentre { get; set; }

    [XmlElement(ElementName = "COSTCENTRENAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? CostCentreName { get; set; }

    [XmlElement(ElementName = "VCHENTRYMODE")]
    [Column(TypeName = $"nvarchar(30)")]
    public string? VoucherEntryMode { get; set; }

    [XmlElement(ElementName = "ISINVOICE")]
    public bool IsInvoice { get; set; }

    [XmlElement(ElementName = "VOUCHERNUMBER")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? VoucherNumber { get; set; }

    [XmlElement(ElementName = "ISOPTIONAL")]
    public bool IsOptional { get; set; }

    [XmlElement(ElementName = "EFFECTIVEDATE")]
    public DateTime? EffectiveDate { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    public string? Narration { get; set; }

    [XmlElement(ElementName = "PRICELEVEL")]
    [Column(TypeName = $"nvarchar({Constants.MaxNarrLength})")]
    public string? PriceLevel { get; set; }

    //E-Invoice Details
    [TallyCategory(Constants.Voucher.Category.EInvoiceDetails)]
    [XmlElement(ElementName = "BILLTOPLACE")]
    [Column(TypeName = $"nvarchar({Constants.MaxNarrLength})")]
    public string? BillToPlace { get; set; }

    [TallyCategory(Constants.Voucher.Category.EInvoiceDetails)]
    [XmlElement(ElementName = "IRN")]
    public string? IRN { get; set; }

    [TallyCategory(Constants.Voucher.Category.EInvoiceDetails)]
    [XmlElement(ElementName = "IRNACKNO")]
    public string? IRNAckNo { get; set; }

    [TallyCategory(Constants.Voucher.Category.EInvoiceDetails)]
    [XmlElement(ElementName = "IRNACKDATE")]
    public DateTime? IRNAckDate { get; set; }



    //Dispatch Details
    [TallyCategory("DispatchDetails")]
    [XmlIgnore]
    public string? DeliveryNoteNo { get; set; }

    [TallyCategory("DispatchDetails")]
    [XmlIgnore]
    public DateTime? ShippingDate { get; set; }

    private DeliveryNotes _DeliveryNotes;

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

    //Shipping Details
    [TallyCategory("ShippingDetails")]
    [JsonIgnore]
    [XmlElement(ElementName = "BASICSHIPDELIVERYNOTE")]
    public DeliveryNotes DeliveryNotes
    {
        get
        {
            DeliveryNoteNo = _DeliveryNotes.DeliveryNote;
            ShippingDate = _DeliveryNotes.ShippingDate;
            return _DeliveryNotes;
        }
        set
        {
            _DeliveryNotes.ShippingDate = value.ShippingDate;
            _DeliveryNotes.DeliveryNote = value.DeliveryNote;
            _DeliveryNotes = value;
        }
    }

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
    public string? DesktinationCountry { get; set; }

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

    //[XmlElement(ElementName = "EWAYBILLDETAILS.LIST")]
    //public List<EwayBillDetail>? EWayBillDetails { get; set; }

    [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST", Type = typeof(AllLedgerEntry))]
    [XmlElement(ElementName = "LEDGERENTRIES.LIST", Type = typeof(ELedgerEntry))]
    public List<AllLedgerEntry>? Ledgers { get; set; }


    [XmlElement(ElementName = "INVENTORYENTRIES.LIST", Type = typeof(EAllInventoryEntry))]
    [XmlElement(ElementName = "ALLINVENTORYENTRIES.LIST", Type = typeof(AllInventoryEntry))]
    public List<AllInventoryEntry>? InventoryAllocations { get; set; }

    [XmlElement(ElementName = "INVENTORYENTRIESOUT.LIST")]
    [TDLCollection(CollectionName = "INVENTORYENTRIESOUT")]
    public List<InventoryoutAllocations>? InventoriesOut { get; set; }

    [XmlElement(ElementName = "INVENTORYENTRIESIN.LIST")]
    [TDLCollection(CollectionName = "INVENTORYENTRIESIN")]
    public List<InventoryinAllocations>? InventoriesIn { get; set; }

    //[XmlElement(ElementName = "CATEGORYENTRY.LIST")]
    //public CategoryEntry? CategoryEntry { get; set; }

    //[XmlElement(ElementName = "ATTENDANCEENTRIES.LIST")]
    //public List<AttendanceEntry>? AttendanceEntries { get; set; }


    public override string ToString()
    {
        
        return $"{VoucherType} - {VoucherNumber}";
    }
}



[XmlRoot(ElementName = "LEDGERENTRIES.LIST")]
[TDLCollection(CollectionName = "LEDGERENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{Constants.Voucher.ViewType.InvoiceVoucherView}")]
public class ELedgerEntry : AllLedgerEntry
{

}

[XmlRoot(ElementName = "ALLLEDGERENTRIES.LIST")]


public class BaseLedgerEntry : IBaseLedgerEntry
{
    public BaseLedgerEntry()
    {
    }


    [XmlElement(ElementName = "LEDGERNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string LedgerName { get; set; }



    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    public bool IsDeemedPositive { get; set; }


    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmountField? Amount { get; set; }


    //[XmlElement(ElementName = "CATEGORYALLOCATIONS.LIST")]
    //public List<CostCategoryAllocations>? CostCategoryAllocations { get; set; }

    [XmlElement(ElementName = "ADDLALLOCTYPE")]
    public AdAllocType AdAllocType { get; set; }

    [XmlElement(ElementName = "ISPARTYLEDGER")]
    public bool IsPartyLedger { get; set; }

    [XmlElement(ElementName = "SWIFTCODE")]
    public string? SWIFTCode { get; set; }

    //[XmlElement(ElementName = "BANKALLOCATIONS.LIST")]
    //public List<BankAllocation>? BankAllocations { get; set; }


    [XmlArray("USERDESCRIPTION.LIST",Namespace = "UDF")]
    [XmlArrayItem(ElementName = "USERDESCRIPTION", Namespace = "UDF")]
    [TDLCollection(CollectionName = "USERDESCRIPTION")]
    public List<string>? UserDescriptions { get; set; }


}
[TDLCollection(CollectionName = "ALLLEDGERENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{Constants.Voucher.ViewType.AccountingVoucherView}")]
public class AllLedgerEntry : BaseLedgerEntry
{
    [XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
    [TDLCollection(CollectionName = "BILLALLOCATIONS")]
    public List<BillAllocations>? BillAllocations { get; set; } = [];

    [XmlElement(ElementName = "INVENTORYALLOCATIONS.LIST")]
    [TDLCollection(CollectionName = "INVENTORYALLOCATIONS", ExplodeCondition = $"$$NUMITEMS:INVENTORYALLOCATIONS>0")]
    public List<BaseInventoryEntry>? InventoryAllocations { get; set; } = [];
}

[XmlRoot(ElementName = "BILLALLOCATIONS.LIST")]

public class BillAllocations : IBaseObject
{
    public BillAllocations()
    {

    }

    [XmlElement(ElementName = "BILLTYPE")]
    public BillRefType? BillType { get; set; }

    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }


    [XmlElement(ElementName = "BILLCREDITPERIOD")]
    public DueDate? BillCreditPeriod { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmountField? Amount { get; set; }



}

[XmlRoot(ElementName = "INVENTORYENTRIESIN.LIST")]
public class InventoryinAllocations : BaseInventoryEntry
{
}

[XmlRoot(ElementName = "INVENTORYENTRIESOUT.LIST")]
public class InventoryoutAllocations : BaseInventoryEntry
{

}

[TDLCollection(CollectionName = "ALLINVENTORYENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{Constants.Voucher.ViewType.InvoiceVoucherView}")]
public class AllInventoryEntry : BaseInventoryEntry
{
    [XmlElement(ElementName = "ACCOUNTINGALLOCATIONS.LIST")]
    [TDLCollection(CollectionName = "ACCOUNTINGALLOCATIONS")]
    public List<BaseLedgerEntry>? Ledgers { get; set; } = [];
}
[TDLCollection(CollectionName = "INVENTORYENTRIES", ExplodeCondition = $"$PERSISTEDVIEW =$$SysName:{Constants.Voucher.ViewType.MfgJournalVoucherView}")]
public class EAllInventoryEntry : AllInventoryEntry
{

}

[XmlRoot(ElementName = "INVENTORYALLOCATIONS.LIST")]


public class BaseInventoryEntry : IBaseObject
{
    public BaseInventoryEntry()
    {
    }
    [XmlArray("BASICUSERDESCRIPTION.LIST")]
    [XmlArrayItem(ElementName = "BASICUSERDESCRIPTION")]

    public List<string>? UserDescriptions { get; set; }


    [XmlElement(ElementName = "STOCKITEMNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? StockItemName { get; set; }




    [XmlElement(ElementName = "BOMNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
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


    [XmlElement(ElementName = "BATCHALLOCATIONS.LIST")]
    public List<BatchAllocations>? BatchAllocations { get; set; }

    //[XmlElement(ElementName = "CATEGORYALLOCATIONS.LIST")]
    //public List<CostCategoryAllocations>? CostCategoryAllocations { get; set; }

}



[XmlRoot(ElementName = "BATCHALLOCATIONS.LIST")]
public class BatchAllocations
{
    [XmlElement(ElementName = "MFDON")]
    public DateTime? ManufacturedOn { get; set; }

    [XmlElement(ElementName = "TRACKINGNUMBER")]
    public string? TrackingNo { get; set; }

    [XmlElement(ElementName = "ORDERNO")]
    public string? OrderNo { get; set; }

    [XmlElement(ElementName = "GODOWNNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? GodownName { get; set; }


    [XmlElement(ElementName = "BATCHNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? BatchName { get; set; }

    [XmlElement(ElementName = "ORDERDUEDATE")]
    public DueDate? OrderDueDate { get; set; }

    [XmlElement(ElementName = "EXPIRYPERIOD")]
    public DueDate? ExpiryPeriod { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmountField? Amount { get; set; }

    [XmlElement(ElementName = "ACTUALQTY")]
    public TallyQuantityField? ActualQuantity { get; set; }

    [XmlElement(ElementName = "BILLEDQTY")]
    public TallyQuantityField? BilledQuantity { get; set; }
}

[XmlRoot(ElementName = "CATEGORYALLOCATIONS.LIST")]
public class CostCategoryAllocations : TallyBaseObject
{
    public CostCategoryAllocations()
    {
    }
    public CostCategoryAllocations(string name, List<CostCenterAllocations> costCenterAllocations)
    {
        CostCategoryName = name;
        CostCenterAllocations = costCenterAllocations;
    }

    [XmlElement(ElementName = "CATEGORY")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? CostCategoryName { get; set; }

    [XmlElement(ElementName = "COSTCATEGORYID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? CostCategoryId { get; set; }

    [XmlElement(ElementName = "COSTCENTREALLOCATIONS.LIST")]
    public List<CostCenterAllocations>? CostCenterAllocations { get; set; }

}
[XmlRoot(ElementName = "COSTCENTREALLOCATIONS.LIST")]
public class CostCenterAllocations : TallyBaseObject
{
    public CostCenterAllocations()
    {
    }
    public CostCenterAllocations(string name, TallyAmount amount)
    {
        Name = name;
        Amount = amount;
    }

    [XmlElement(ElementName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "COSTCENTREID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? CostCenterId { get; set; }

    [XmlElement(ElementName = "AMOUNT")]

    public TallyAmount? Amount { get; set; }


}


[XmlRoot(ElementName = "ATTENDANCEENTRIES.LIST")]
public class AttendanceEntry
{
    [XmlElement(ElementName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "EMPLOYEEID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string EmployeeId { get; set; }

    [XmlElement(ElementName = "ATTENDANCETYPE")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? AttendanceType { get; set; }

    [XmlElement(ElementName = "ATTENDANCETYPEID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string AttendanceTypeId { get; set; }

    [XmlElement(ElementName = "ATTDTYPETIMEVALUE")]
    [Column(TypeName = "decimal(10,4)")]
    public decimal? AttendanceTypeTimeValue { get; set; }

    [XmlElement(ElementName = "ATTDTYPEVALUE")]
    public TallyQuantity? AttendanceTypeValue { get; set; }

    public bool IsNull()
    {
        if (string.IsNullOrEmpty(Name) &&
            string.IsNullOrEmpty(AttendanceType) && AttendanceTypeValue == null)
        {
            return true;
        }
        return false;
    }
}

[XmlRoot(ElementName = "INVOICEDELNOTES.LIST")]
public class DeliveryNotes
{
    [XmlElement(ElementName = "BASICSHIPPINGDATE")]
    public DateTime? ShippingDate { get; set; }

    [XmlElement(ElementName = "BASICSHIPDELIVERYNOTE")]
    public string? DeliveryNote { get; set; }
}


[XmlRoot(ElementName = "EWAYBILLDETAILS.LIST")]
public class EwayBillDetail : IBaseObject
{
    [XmlElement(ElementName = "BILLDATE")]
    public DateTime? BillDate { get; set; }

    [XmlElement(ElementName = "CONSOLIDATEDBILLDATE")]
    public DateTime? ConsolidatedBillDate { get; set; }

    [XmlElement(ElementName = "BILLNUMBER")]
    public string? BillNumber { get; set; }

    [XmlElement(ElementName = "CONSOLIDATEDBILLNUMBER")]
    public string? ConsolidatedBillNumber { get; set; }

    [XmlElement(ElementName = "SUBTYPE")]
    public SubSupplyType SubType { get; set; }

    [XmlElement(ElementName = "DOCUMENTTYPE")]
    public DocumentType DocumentType { get; set; }

    [XmlElement(ElementName = "CONSIGNORPLACE")]
    public string? DispatchFrom { get; set; }

    [XmlElement(ElementName = "CONSIGNEEPLACE")]
    public string? DispatchTo { get; set; }

    [XmlElement(ElementName = "ISCANCELLED")]
    public string? IsCancelled { get; set; }

    [XmlElement(ElementName = "ISCANCELPENDING")]
    public string? IsCancelledPending { get; set; }

    [XmlElement(ElementName = "TRANSPORTDETAILS.LIST")]
    public List<TransporterDetail>? TransporterDetails { get; set; }

}

[XmlRoot(ElementName = "TRANSPORTDETAILS.LIST")]
public class TransporterDetail : IBaseObject
{
    [XmlElement(ElementName = "DISTANCE")]
    public string? Distance { get; set; }

    [XmlElement(ElementName = "TRANSPORTERNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? TransporterName { get; set; }

    [XmlElement(ElementName = "TRANSPORTERID")]
    public string? TransporterId { get; set; }

    [XmlElement(ElementName = "TRANSPORTMODE")]
    public TransportMode? TransportMode { get; set; }

    /// <summary>
    /// Document/Landing/RR/Airway Number/ 
    /// </summary>
    [XmlElement(ElementName = "DOCUMENTNUMBER")]
    public string? DocumentNumber { get; set; }

    [XmlElement(ElementName = "DOCUMENTDATE")]
    public string? DocumentDate { get; set; }

    [XmlElement(ElementName = "VEHICLENUMBER")]
    public string? VehicleNumber { get; set; }

    [XmlElement(ElementName = "VEHICLETYPE")]
    public VehicleType? VehicleType { get; set; }

}

[XmlRoot(ElementName = "CATEGORYENTRY.LIST")]
public class CategoryEntry
{
    [XmlElement(ElementName = "CATEGORY")]
    public string Category { get; set; }
    [XmlElement(ElementName = "CATEGORYID")]
    public string CategoryId { get; set; }

    [XmlElement(ElementName = "EMPLOYEEENTRIES.LIST")]
    public List<EmployeeEntry> EmployeeEntries { get; set; }
}

[XmlRoot(ElementName = "EMPLOYEEENTRIES.LIST")]
public class EmployeeEntry
{
    [XmlElement(ElementName = "EMPLOYEENAME")]
    public string EmployeeName { get; set; }

    [XmlElement(ElementName = "EMPLOYEEID")]
    public string EmployeeId { get; set; }

    [XmlElement(ElementName = "EMPLOYEESORTORDER")]
    public int EmployeeSortOrder { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmount Amount { get; set; }

    [XmlElement(ElementName = "PAYHEADALLOCATIONS.LIST")]
    public List<PayHeadAllocation> PayHeadAllocations { get; set; }
}
[XmlRoot(ElementName = "PAYHEADALLOCATIONS.LIST")]
public class PayHeadAllocation
{
    [XmlElement(ElementName = "PAYHEADNAME")]
    public string PayHeadName { get; set; }

    [XmlElement(ElementName = "LEDGERID")]
    public string LedgerId { get; set; }

    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    [JsonIgnore]
    public bool? IsDeemedPositive
    {
        get
        {
            if (Amount != null)
            {
                return Amount.IsDebit;
            }
            return null;

        }
        set { }
    }

    [XmlElement(ElementName = "PAYHEADSORTORDER")]
    public int PayHeadSortOrder { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmount Amount { get; set; }
}

[XmlRoot(ElementName = "BANKALLOCATIONS.LIST")]
public class BankAllocation
{
    [XmlElement(ElementName = "DATE")]
    public TallyDate Date { get; set; }

    /// <summary>
    /// Use this field for Bank Reconcilliation Date
    /// </summary>
    [XmlElement(ElementName = "BANKERSDATE")]
    public DateTime? BankersDate { get; set; }

    [XmlElement(ElementName = "INSTRUMENTDATE")]
    public TallyDate InstrumentDate { get; set; }

    [XmlElement(ElementName = "INSTRUMENTNUMBER")]
    public string? InstrumentNumber { get; set; }

    [XmlElement(ElementName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "EMAIL")]
    public string? Email { get; set; }

    [XmlElement(ElementName = "TRANSACTIONTYPE")]
    public BankTransactionType TransactionType { get; set; }

    [XmlElement(ElementName = "BANKNAME")]
    public string? BankName { get; set; }

    [XmlElement(ElementName = "IFSCODE")]
    public string? IFSCCode { get; set; }

    [XmlElement(ElementName = "ACCOUNTNUMBER")]
    public string? AccountNumber { get; set; }

    [XmlElement(ElementName = "BENEFICIARYCODE")]
    public string? BeneficiaryCode { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    public string? Remarks { get; set; }

    [XmlElement(ElementName = "TRANSFERMODE")]
    public string? TransferMode { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmount? Amount { get; set; }

    [XmlElement(ElementName = "PAYMENTFAVOURING")]
    public string? PaymentFavouring { get; set; }

    [XmlElement(ElementName = "UNIQUEREFERENCENUMBER")]
    public string? UniqueReferenceNumber { get; set; }

    [XmlElement(ElementName = "BANKPARTYNAME")]
    public string? BankPartyName { get; set; }


    internal bool IsNull()
    {
        if (Date == null && BankersDate == null && InstrumentDate == null)
        {
            return true;
        }
        return false;
    }
}
public class GSTRegistration
{
    [XmlAttribute(AttributeName = "TAXTYPE")]
    public string TaxType { get; set; } = "GST";

    [XmlAttribute(AttributeName = "TAXREGISTRATION")]
    public string TaxRegistration { get; set; }

    [XmlText]
    public string RegistrationName { get; set; }
}
public enum VoucherLookupField
{
    MasterId = 1,
    AlterId = 2,
    VoucherNumber = 3,
    GUID = 4,
}
public enum BillRefType
{
    [EnumXMLChoice(Choice = "New Ref")]
    NewRef,
    [EnumXMLChoice(Choice = "On Account")]
    OnAccount,
    [EnumXMLChoice(Choice = "Agst Ref")]
    AgstRef,
    [EnumXMLChoice(Choice = "Advance")]
    Advance

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

/// <summary>
/// <para>e-Waybill SubTypes as per  Tally</para>
/// <para>TDL Reference -  "DEFTDL:src\voucher\vchreport\vchgstewaybillsubforms\vchgstewaybillfunctions.tdl"(496)
/// Search using "subSupplyTypeCode-"</para>
/// </summary>
public enum SubSupplyType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Supply")]
    Supply = 1,
    [XmlEnum(Name = "Import")]
    Import = 2,
    [XmlEnum(Name = "Export")]
    Export = 3,
    [XmlEnum(Name = "Job Work")]
    JobWork = 4,
    [XmlEnum(Name = "For Own Use")]
    ForOwnUse = 5,
    [XmlEnum(Name = "Job Work Returns")]
    JobWorkReturns = 6,

    [XmlEnum(Name = "Sales Return")]
    SalesReturn = 7,
    [XmlEnum(Name = "Others")]
    Others = 8,
    [XmlEnum(Name = "SKD/CKD/Lots")]
    SKD_CKD_Lots = 9,
    [XmlEnum(Name = "Lines Sales")]
    LinesSales = 10,
    [XmlEnum(Name = "Recipient Not Known")]
    RecipientNotKnown = 11,
    [XmlEnum(Name = "Exhibition or Fairs")]
    ExhibitionorFairs = 12,

}
/// <summary>
/// <para>e-Waybill DocTypes as per  Tally</para>
/// <para>TDL Reference -  "DEFTDL:src\voucher\vchreport\vchgstewaybillsubforms\vchgstewaybillfunctions.tdl"(496)
/// Search using "docTypeCode-"</para>
/// </summary>
public enum DocumentType
{
    [XmlEnum(Name = "")]
    None = 0,

    [XmlEnum(Name = "Tax Invoice")]
    TaxInvoice = 1,
    [XmlEnum(Name = "Bill of Supply")]
    BillofSupply = 2,
    [XmlEnum(Name = "Bill of Entry")]
    BillofEntry = 3,
    [XmlEnum(Name = "Challan")]
    Challan = 4,
    [XmlEnum(Name = "Delivery Challan")]
    DeliveryChallan = 5,
    [XmlEnum(Name = "Credit Note")]
    CreditNote = 6,
    [XmlEnum(Name = "Others")]
    Others = 7,
}

/// <summary>
/// <para>e-Waybill TransportModes as per  Tally</para>
/// <para>TDL Reference -  "DEFTDL:src\voucher\vchreport\vchgstewaybillsubforms\vchgstewaybillfunctions.tdl"(496)
/// Search using "transModeCode-"</para>
/// </summary>
public enum TransportMode
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Road")]
    Road = 1,
    [XmlEnum(Name = "1 - Road")]
    Road_InPrime = 1,
    [XmlEnum(Name = "Rail")]
    Rail = 2,
    [XmlEnum(Name = "2 - Rail")]
    Rail_InPrime = 2,
    [XmlEnum(Name = "Air")]
    Air = 3,
    [XmlEnum(Name = "3 - Air")]
    Air_InPrime = 3,
    [XmlEnum(Name = "Ship")]
    Ship = 4,
    [XmlEnum(Name = "4 - Ship")]
    Ship_InPrime = 4,
}


/// <summary>
/// <para>e-Waybill VehicleTypes as per  Tally</para>
///  <para>TDL Reference -  "DEFTDL:src\voucher\vchreport\vchgstewaybillsubforms\vchgstewaybillfunctions.tdl"
///  Search using "Over Dimensional Cargo"</para>
/// </summary>
public enum VehicleType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Regular")]
    Regular = 1,
    [XmlEnum(Name = "R - Regular")]
    Regular_InPrime = 1,
    [XmlEnum(Name = "Over Dimensional Cargo")]
    OverDimensionalCargo = 2,
    [XmlEnum(Name = "O - Over Dimensional Cargo")]
    OverDimensionalCargo_InPrime = 2,

}

public enum BankTransactionType
{
    [XmlEnum(Name = "Others")]
    Others = 0,
    [XmlEnum(Name = "ATM")]
    ATM = 1,
    [XmlEnum(Name = "Cash")]
    Cash = 2,
    [XmlEnum(Name = "Cheque")]
    Cheque = 3,
    [XmlEnum(Name = "Card")]
    Card = 4,
    [XmlEnum(Name = "ECS")]
    ECS = 5,
    [XmlEnum(Name = "Electronic Cheque")]
    ElectronicCheque = 6,
    [XmlEnum(Name = "Electronic DD/PO")]
    ElectronicDDPO = 7,
    [XmlEnum(Name = "Inter Bank Transfer")]
    InterBankTransfer = 8,
    [XmlEnum(Name = "Same Bank Transfer")]
    SameBankTransfer = 9,
    [XmlEnum(Name = "Cheque/DD")]
    ChequeDD = 10,
}

