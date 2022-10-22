namespace TallyConnector.Core.Models;

[Serializable]
[XmlRoot(ElementName = "VOUCHER", Namespace = "")]
public class Voucher : BasicTallyObject, ITallyObject
{
    public Voucher()
    {
        _DeliveryNotes = new();
    }

    [XmlElement(ElementName = "DATE")]
    public TallyDate Date { get; set; }

    [XmlElement(ElementName = "REFERENCEDATE")]
    public TallyDate? ReferenceDate { get; set; }

    [XmlElement(ElementName = "REFERENCE")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Reference { get; set; }


    [XmlElement(ElementName = "VOUCHERTYPENAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string VoucherType { get; set; }

    [XmlElement(ElementName = "VOUCHERTYPEID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? VoucherTypeId { get; set; }


    [XmlElement(ElementName = "PERSISTEDVIEW")]
    [Column(TypeName = $"nvarchar(30)")]
    public VoucherViewType View { get; set; }

    [XmlElement(ElementName = "VOUCHERNUMBER")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? VoucherNumber { get; set; }

    [XmlElement(ElementName = "ISOPTIONAL")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsOptional { get; set; }

    [XmlElement(ElementName = "EFFECTIVEDATE")]
    public TallyDate? EffectiveDate { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    [Column(TypeName = $"nvarchar({Constants.MaxNarrLength})")]
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
    public string? IRNAckDate { get; set; }



    //Dispatch Details
    [TallyCategory("DispatchDetails")]
    [XmlIgnore]
    public string? DeliveryNoteNo { get; set; }

    [TallyCategory("DispatchDetails")]
    [XmlIgnore]
    public TallyDate? ShippingDate { get; set; }

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

    [XmlElement(ElementName = "PARTYLEDGERID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? PartyLedgerId { get; set; }

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


    [XmlElement(ElementName = "ISCANCELLED")]
    public TallyYesNo? IsCancelled { get; set; }

    //EWAY Details
    [XmlElement(ElementName = "OVRDNEWAYBILLAPPLICABILITY")]
    public TallyYesNo? OverrideEWayBillApplicability { get; set; }

    [XmlElement(ElementName = "EWAYBILLDETAILS.LIST")]
    public List<EwayBillDetail>? EWayBillDetails { get; set; }

    [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST", Type = typeof(VoucherLedger))]
    [XmlElement(ElementName = "LEDGERENTRIES.LIST", Type = typeof(EVoucherLedger))]
    public List<VoucherLedger>? Ledgers { get; set; }


    [XmlElement(ElementName = "ALLINVENTORYENTRIES.LIST", Type = typeof(AllInventoryAllocations))]
    [XmlElement(ElementName = "INVENTORYENTRIES.LIST", Type = typeof(InventoryEntries))]
    public List<AllInventoryAllocations>? InventoryAllocations { get; set; }

    [XmlElement(ElementName = "INVENTORYENTRIESOUT.LIST")]
    public List<InventoryoutAllocations>? InventoriesOut { get; set; }

    [XmlElement(ElementName = "INVENTORYENTRIESIN.LIST")]
    public List<InventoryinAllocations>? InventoriesIn { get; set; }

    [XmlElement(ElementName = "CATEGORYENTRY.LIST")]
    public CategoryEntry? CategoryEntries { get; set; }

    [XmlElement(ElementName = "ATTENDANCEENTRIES.LIST")]
    public List<AttendanceEntry>? AttendanceEntries { get; set; }

    [JsonIgnore]
    [XmlAttribute(AttributeName = "DATE")]
    [NotMapped]
    public string? Dt
    {
        get
        {
            if (Date != null)
            {
                return ((DateTime)Date!).ToString("yyyMMdd");
            }
            else { return null; }
        }
        set => Date = value ?? string.Empty;
    }


    [JsonIgnore]
    [NotMapped]
    [XmlAttribute(AttributeName = "VCHTYPE")]
    public string? VchType
    {
        get
        {
            return VoucherType;
        }
        set
        {
            VoucherType = value;
        }
    }



    public void OrderLedgers()
    {
        //if (VchType != "Contra" && VchType != "Purchase" && VchType != "Receipt" && VchType != "Credit Note")
        //{
        //    Ledgers?.Sort((x, y) => y.LedgerName!.CompareTo(x.LedgerName));//First Sort Ledger list Using Ledger Names
        //    Ledgers?.Sort((x, y) => y.Amount!.Amount!.CompareTo(x.Amount!.Amount)); //Next sort Ledger List Using Ledger Amounts
        //    Ledgers?.Sort((x, y) => y.Amount!.IsDebit.CompareTo(x.Amount!.IsDebit));
        //}
        //else
        //{
        //    Ledgers?.Sort((x, y) => x.LedgerName!.CompareTo(y.LedgerName));//First Sort Ledger list Using Ledger Names
        //    Ledgers?.Sort((x, y) => x.Amount!.Amount.CompareTo(y.Amount!.Amount)); //Next sort Ledger List Using Ledger Amounts
        //    Ledgers?.Sort((x, y) => x.Amount!.IsDebit.CompareTo(y.Amount!.IsDebit));
        //}

    }

    public new string GetJson(bool Indented = false)
    {
        OrderLedgers();

        return base.GetJson(Indented);
    }

    public new string GetXML(XmlAttributeOverrides? attrOverrides = null)
    {
        OrderLedgers();
        GetJulianday();
        return base.GetXML(attrOverrides);
    }
    public void GetJulianday()
    {
        Ledgers?.ForEach(ledg =>
        {
            ledg.BillAllocations?.ForEach(billalloc =>
            {
                if (billalloc.BillCreditPeriod != null)
                {
                    EffectiveDate ??= Date;
                    DateTime dateTime = (DateTime)EffectiveDate!;
                    double days = dateTime.Subtract(new DateTime(1900, 1, 1)).TotalDays + 1;
                   // billalloc.BillCP.JD = days.ToString();
                }
            });
        });
    }

    public new void PrepareForExport()
    {
        OrderLedgers(); //Ensures ledgers are ordered in correct way
        GetJulianday();
        //InventoryAllocations?.ForEach(c => c.BatchAllocations?.ForEach(btch => btch.OrderDueDate = Date));
    }

    /// <inheritdoc/>
    public override void RemoveNullChilds()
    {
        EWayBillDetails = EWayBillDetails?.Where(Ewaybilldetail => !Ewaybilldetail.IsNull())?.ToList();
        if (EWayBillDetails?.Count == 0)
        {
            EWayBillDetails = null;
        }
    }

    public override string ToString()
    {
        return $"{VoucherType} - {VoucherNumber}";
    }
}

[XmlRoot(ElementName = "LEDGERENTRIES.LIST")]
public class EVoucherLedger : VoucherLedger
{

}

[XmlRoot(ElementName = "ALLLEDGERENTRIES.LIST")]
public class VoucherLedger : TallyBaseObject
{

    public VoucherLedger()
    {
    }


    [XmlElement(ElementName = "INDEXNUMBER")]
    public int IndexNumber { get; set; }

    [XmlElement(ElementName = "LEDGERNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string LedgerName { get; set; }

    [XmlElement(ElementName = "LEDGERID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? LedgerId { get; set; }

    [XmlElement(ElementName = "LEDGERTAXTYPE")]
    public string? LedgerTaxType { get; set; }

    [XmlElement(ElementName = "VCHLEDGERTYPE")]
    public string? LedgerType { get; set; }


    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    [JsonIgnore]
    public TallyYesNo? IsDeemedPositive
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


    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmount? Amount { get; set; }



    [XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
    public List<BillAllocations>? BillAllocations { get; set; }

    [XmlElement(ElementName = "INVENTORYALLOCATIONS.LIST")]
    public List<InventoryAllocations>? InventoryAllocations { get; set; }

    [XmlElement(ElementName = "CATEGORYALLOCATIONS.LIST")]
    public List<CostCategoryAllocations>? CostCategoryAllocations { get; set; }



}

[XmlRoot(ElementName = "BILLALLOCATIONS.LIST")]
public class BillAllocations : TallyBaseObject
{
    public BillAllocations()
    {
        
    }

    [XmlElement(ElementName = "BILLTYPE")]
    public BillRefType? BillType { get; set; }

    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "BILLID")]
    public int BillId { get; set; }

    [XmlElement(ElementName = "LEDGERID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? LedgerId { get; set; }

    [XmlElement(ElementName = "BILLCREDITPERIOD")]
    public TallyDueDate? BillCreditPeriod { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmount? Amount { get; set; }


}
[XmlRoot(ElementName = "BILLCREDITPERIOD")]
public class BillCP
{
    [XmlAttribute(AttributeName = "JD")]
    public string? JD { get; set; }

    private string? _days;
    [XmlAttribute(AttributeName = "Days")]
    public string? Days
    {
        get { return _days; }
        set { _days = value; }
    }

    [XmlText]
    public string? TextValue
    {
        get { return _days; }
        set { _days = value; }
    }

}


[XmlRoot(ElementName = "INVENTORYENTRIESIN.LIST")]
public class InventoryinAllocations : InventoryAllocations
{
}

[XmlRoot(ElementName = "INVENTORYENTRIESOUT.LIST")]
public class InventoryoutAllocations : InventoryAllocations
{

}

[XmlRoot(ElementName = "ALLINVENTORYENTRIES.LIST")]
public class AllInventoryAllocations : InventoryAllocations
{
    [XmlElement(ElementName = "ACCOUNTINGALLOCATIONS.LIST")]
    public List<VoucherLedger>? Ledgers { get; set; }
}
[XmlRoot(ElementName = "INVENTORYENTRIES.LIST")]
public class InventoryEntries : AllInventoryAllocations
{
}

[XmlRoot(ElementName = "INVENTORYALLOCATIONS.LIST")]
public class InventoryAllocations : TallyBaseObject
{
    public InventoryAllocations()
    {
    }

    [XmlElement(ElementName = "INDEXNUMBER")]
    public int IndexNumber { get; set; }

    [XmlElement(ElementName = "STOCKITEMNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? StockItemName { get; set; }

    [XmlElement(ElementName = "STOCKITEMID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? StockItemId { get; set; }

    [XmlElement(ElementName = "BOMNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? BOMName { get; set; }

    [XmlElement(ElementName = "ISSCRAP")]
    public TallyYesNo? IsScrap { get; set; }


    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    [JsonIgnore]
    public TallyYesNo? IsDeemedPositive
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

    [XmlElement(ElementName = "RATE")]
    public TallyRate? Rate { get; set; }

    [XmlElement(ElementName = "ACTUALQTY")]
    public TallyQuantity? ActualQuantity { get; set; }

    [XmlElement(ElementName = "BILLEDQTY")]
    public TallyQuantity? BilledQuantity { get; set; }


    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmount? Amount { get; set; }

    [XmlElement(ElementName = "BATCHALLOCATIONS.LIST")]
    public List<BatchAllocations>? BatchAllocations { get; set; }

    [XmlElement(ElementName = "CATEGORYALLOCATIONS.LIST")]
    public List<CostCategoryAllocations>? CostCategoryAllocations { get; set; }

}

[XmlRoot(ElementName = "BATCHALLOCATIONS.LIST")]
public class BatchAllocations : TallyBaseObject//Godown Allocations
{

    [XmlElement(ElementName = "TRACKINGNUMBER")]
    public string? TrackingNo { get; set; }

    [XmlElement(ElementName = "ORDERNO")]
    public string? OrderNo { get; set; }

    [XmlElement(ElementName = "GODOWNNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? GodownName { get; set; }

    [XmlElement(ElementName = "GODOWNID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? GodownId { get; set; }

    [XmlElement(ElementName = "BATCHNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? BatchName { get; set; }

    [XmlElement(ElementName = "ORDERDUEDATE")]
    public TallyDueDate? OrderDueDate { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public TallyAmount? Amount { get; set; }

    [XmlElement(ElementName = "ACTUALQTY")]
    public TallyQuantity? ActualQuantity { get; set; }

    [XmlElement(ElementName = "BILLEDQTY")]
    public TallyQuantity? BilledQuantity { get; set; }
}

[XmlRoot(ElementName = "CATEGORYALLOCATIONS.LIST")]
public class CostCategoryAllocations : TallyBaseObject
{
    public CostCategoryAllocations()
    {
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
    public decimal? AttendanceTypeTimeValue { get; set; }

    [XmlElement(ElementName = "ATTDTYPEVALUE")]
    public TallyQuantity? AttendanceTypeValue { get; set; }

}

[XmlRoot(ElementName = "INVOICEDELNOTES.LIST")]
public class DeliveryNotes
{
    [XmlElement(ElementName = "BASICSHIPPINGDATE")]
    public TallyDate? ShippingDate { get; set; }

    [XmlElement(ElementName = "BASICSHIPDELIVERYNOTE")]
    public string? DeliveryNote { get; set; }
}


[XmlRoot(ElementName = "EWAYBILLDETAILS.LIST")]
public class EwayBillDetail : TallyBaseObject, ICheckNull
{
    [XmlElement(ElementName = "BILLDATE")]
    public TallyDate? BillDate { get; set; }

    [XmlElement(ElementName = "CONSOLIDATEDBILLDATE")]
    public TallyDate? ConsolidatedBillDate { get; set; }

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

    /// <inheritdoc/>
    public bool IsNull()
    {
        TransporterDetails = TransporterDetails?.Where(Details => !Details.IsNull())?.ToList();
        if (TransporterDetails?.Count == 0)
        {
            TransporterDetails = null;
        }
        if (TransporterDetails == null && BillDate is null || BillDate == DateTime.MinValue && string.IsNullOrEmpty(BillNumber))
        {
            return true;
        }
        return false;
    }
}

[XmlRoot(ElementName = "TRANSPORTDETAILS.LIST")]
public class TransporterDetail : TallyBaseObject, ICheckNull
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

    public bool IsNull()
    {
        if (string.IsNullOrEmpty(Distance)
            && string.IsNullOrEmpty(TransporterName)
            && string.IsNullOrEmpty(TransporterId)
            && (TransportMode is null || TransportMode is Models.TransportMode.None)
            && string.IsNullOrEmpty(DocumentNumber)
            && string.IsNullOrEmpty(VehicleNumber)
            && (VehicleType is null || VehicleType is Models.VehicleType.None))
        {
            return true;
        }
        return false;
    }
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
    public TallyYesNo? IsDeemedPositive
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

public enum VoucherLookupField
{
    MasterId = 1,
    AlterId = 2,
    VoucherNumber = 3,
    GUID = 4,
}
public enum BillRefType
{
    [XmlEnum(Name = "New Ref")]
    NewRef,
    [XmlEnum(Name = "On Account")]
    OnAccount,
    [XmlEnum(Name = "Agst Ref")]
    AgstRef,
    [XmlEnum(Name = "Advance")]
    Advance

}

/// <summary>
/// <para>Voucher ViewTypes avavailable in Tally</para>
/// <para>TDL Reference -  src\voucher\vchreport.tdl
/// Search using "Set						: SVViewName"</para>
/// </summary>
public enum VoucherViewType
{
    [XmlEnum(Name = "")]
    None = 0,
    [XmlEnum(Name = "Accounting Voucher View")]
    AccountingVoucherView = 1,

    [XmlEnum(Name = "Invoice Voucher View")]
    InvoiceVoucherView = 2,

    [XmlEnum(Name = "PaySlip Voucher View")]
    PaySlipVoucherView = 3,

    [XmlEnum(Name = "Multi Consumption Voucher View")]
    MultiConsumptionVoucherView = 4,

    [XmlEnum(Name = "Consumption Voucher View")]
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



