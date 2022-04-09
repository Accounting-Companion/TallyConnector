using System.Globalization;
using System.Text.RegularExpressions;

namespace TallyConnector.Models;

[Serializable]
[XmlRoot(ElementName = "VOUCHER", Namespace = "")]
public class Voucher : BasicTallyObject, ITallyObject
{
    public Voucher()
    {
        _DeliveryNotes = new();
    }

    [XmlElement(ElementName = "DATE")]
    public string Date { get; set; }

    [XmlElement(ElementName = "REFERENCEDATE")]
    public string ReferenceDate { get; set; }

    [XmlElement(ElementName = "REFERENCE")]
    public string Reference { get; set; }


    [XmlElement(ElementName = "VOUCHERTYPENAME")]
    public string VoucherType { get; set; }


    [XmlElement(ElementName = "PERSISTEDVIEW")]
    public VoucherViewType View { get; set; }

    [XmlElement(ElementName = "VOUCHERNUMBER")]
    public string VoucherNumber { get; set; }

    [XmlElement(ElementName = "ISOPTIONAL")]
    public string IsOptional { get; set; }

    [XmlElement(ElementName = "EFFECTIVEDATE")]
    public string EffectiveDate { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    public string Narration { get; set; }

    [XmlElement(ElementName = "PRICELEVEL")]
    public string PriceLevel { get; set; }

    //E-Invoice Details
    [TallyCategory("E-InvoiceDetails")]
    [XmlElement(ElementName = "BILLTOPLACE")]
    public string BillToPlace { get; set; }

    [TallyCategory("E-InvoiceDetails")]
    [XmlElement(ElementName = "IRN")]
    public string IRN { get; set; }

    [TallyCategory("E-InvoiceDetails")]
    [XmlElement(ElementName = "IRNACKNO")]
    public string IRNAckNo { get; set; }

    [TallyCategory("E-InvoiceDetails")]
    [XmlElement(ElementName = "IRNACKDATE")]
    public string IRNAckDate { get; set; }



    //Dispatch Details
    [TallyCategory("DispatchDetails")]
    [XmlIgnore]
    public string DeliveryNoteNo { get; set; }

    [TallyCategory("DispatchDetails")]
    [XmlIgnore]
    public string ShippingDate { get; set; }

    private DeliveryNotes _DeliveryNotes;

    [TallyCategory("DispatchDetails")]
    [XmlElement(ElementName = "DISPATCHFROMNAME")]
    public string DispatchFromName { get; set; }

    [TallyCategory("DispatchDetails")]
    [XmlElement(ElementName = "DISPATCHFROMSTATENAME")]
    public string DispatchFromStateName { get; set; }

    [TallyCategory("DispatchDetails")]
    [XmlElement(ElementName = "DISPATCHFROMPINCODE")]
    public string DispatchFromPinCode { get; set; }

    [TallyCategory("DispatchDetails")]
    [XmlElement(ElementName = "DISPATCHFROMPLACE")]
    public string DispatchFromPlace { get; set; }

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
    public string DispatchDocNo { get; set; }
    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "BASICSHIPPEDBY")]
    public string BasicShippedBy { get; set; }
    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "BASICFINALDESTINATION")]
    public string Destination { get; set; }

    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "EICHECKPOST")]
    public string CarrierName { get; set; }

    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "BILLOFLADINGNO")]
    public string BillofLandingNo { get; set; }

    [TallyCategory("ShippingDetails")]
    [XmlElement(ElementName = "BILLOFLADINGDATE")]
    public string BillofLandingDate { get; set; }


    //Export Shipping Details
    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICPLACEOFRECEIPT")]
    public string PlaceOfReceipt { get; set; }

    /// <summary>
    /// Vehicle or Ship or Flight Number
    /// </summary>
    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICSHIPVESSELNO")]
    public string ShipOrFlightNo { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICPORTOFLOADING")]
    public string LandingPort { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICPORTOFDISCHARGE")]
    public string DischargePort { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "BASICDESTINATIONCOUNTRY")]
    public string DesktinationCountry { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "SHIPPINGBILLNO")]
    public string ShippingBillNo { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "SHIPPINGBILLDATE")]
    public string ShippingBillDate { get; set; }

    [TallyCategory("ExportShippingDetails")]
    [XmlElement(ElementName = "PORTCODE")]
    public string PortCode { get; set; }

    //OrderDetails
    [TallyCategory("OrderDetails")]
    [XmlElement(ElementName = "BASICDUEDATEOFPYMT")]
    public string BasicDueDateofPayment { get; set; }

    [TallyCategory("OrderDetails")]
    [XmlElement(ElementName = "BASICORDERREF")]
    public string OrderReference { get; set; }



    //Party Details
    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PARTYNAME")]
    public string PartyName { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PARTYLEDGERNAME")]
    public string PartyLedgerName { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PARTYMAILINGNAME")]
    public string PartyMailingName { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "STATENAME")]
    public string State { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "COUNTRYOFRESIDENCE")]
    public string Country { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "GSTREGISTRATIONTYPE")]
    public string RegistrationType { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PARTYGSTIN")]
    public string PartyGSTIN { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PLACEOFSUPPLY")]
    public string PlaceOfSupply { get; set; }

    [TallyCategory("PartyDetails")]
    [XmlElement(ElementName = "PARTYPINCODE")]
    public string PINCode { get; set; }

    //Consignee Details
    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "BASICBUYERNAME")]
    public string ConsigneeName { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEEMAILINGNAME")]
    public string ConsigneeMailingName { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEESTATENAME")]
    public string ConsigneeState { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEECOUNTRYNAME")]
    public string ConsigneeCountry { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEEGSTIN")]
    public string ConsigneeGSTIN { get; set; }

    [TallyCategory("ConsigneeDetails")]
    [XmlElement(ElementName = "CONSIGNEEPINCODE")]
    public string ConsigneePinCode { get; set; }





    [XmlElement(ElementName = "ISCANCELLED")]
    public string IsCancelled { get; set; }

    //EWAY Details
    [XmlElement(ElementName = "OVRDNEWAYBILLAPPLICABILITY")]
    public YesNo OverrideEWayBillApplicability { get; set; }

    [XmlElement(ElementName = "EWAYBILLDETAILS.LIST")]
    public List<EwayBillDetail> EWayBillDetails { get; set; }

    [XmlElement(ElementName = "ALLLEDGERENTRIES.LIST", Type = typeof(VoucherLedger))]
    [XmlElement(ElementName = "LEDGERENTRIES.LIST", Type = typeof(EVoucherLedger))]
    public List<VoucherLedger> Ledgers { get; set; }


    [XmlElement(ElementName = "ALLINVENTORYENTRIES.LIST", Type = typeof(AllInventoryAllocations))]
    [XmlElement(ElementName = "INVENTORYENTRIES.LIST", Type = typeof(InventoryEntries))]
    public List<AllInventoryAllocations> InventoryAllocations { get; set; }

    [XmlElement(ElementName = "INVENTORYENTRIESOUT.LIST")]
    public List<InventoryoutAllocations> InventoriesOut { get; set; }

    [XmlElement(ElementName = "INVENTORYENTRIESIN.LIST")]
    public List<InventoryinAllocations> InventoriesIn { get; set; }



    [XmlElement(ElementName = "ATTENDANCEENTRIES.LIST")]
    public List<AttendanceEntry> AttendanceEntries { get; set; }

    [JsonIgnore]
    [XmlAttribute(AttributeName = "DATE")]
    public string Dt
    {
        get
        {
            return Date;
        }
        set
        {
            Date = value;
        }
    }


    /// <summary>
    /// Accepted Values //Create, Alter, Delete
    /// </summary>
    [JsonIgnore]
    [XmlAttribute(AttributeName = "Action")]
    public Action Action { get; set; }


    [JsonIgnore]
    [XmlAttribute(AttributeName = "VCHTYPE")]
    public string VchType
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
        if (VchType != "Contra" && VchType != "Purchase" && VchType != "Receipt" && VchType != "Credit Note")
        {
            Ledgers?.Sort((x, y) => y.LedgerName.CompareTo(x.LedgerName));//First Sort Ledger list Using Ledger Names
            Ledgers?.Sort((x, y) => y.Amount.CompareTo(x.Amount)); //Next sort Ledger List Using Ledger Amounts

        }
        else
        {
            Ledgers?.Sort((x, y) => x.LedgerName.CompareTo(y.LedgerName));//First Sort Ledger list Using Ledger Names
            Ledgers?.Sort((x, y) => x.Amount.CompareTo(y.Amount)); //Next sort Ledger List Using Ledger Amounts
        }

        //Looop Through all Ledgers
        Ledgers?.ForEach(c =>
        {
            //Sort Bill Allocations
            c.BillAllocations?.Sort((x, y) => x.Name.CompareTo(y.Name)); //First Sort BillAllocations Using Bill Numbers
            c.BillAllocations?.Sort((x, y) => x.Amount.CompareTo(y.Amount));//Next sort BillAllocationst Using  Amounts

            c.CostCategoryAllocations?.Sort((x, y) => x.CostCategoryName.CompareTo(y.CostCategoryName));

            c.CostCategoryAllocations?.ForEach(cc =>
            {
                cc.CostCenterAllocations?.Sort((x, y) => x.Name.CompareTo(y.Name));
                cc.CostCenterAllocations?.Sort((x, y) => x.Amount.CompareTo(y.Amount));
            });
            //sort Inventory Allocations
            c.InventoryAllocations?.Sort((x, y) => x.ActualQuantity.CompareTo(y.ActualQuantity));
            c.InventoryAllocations?.Sort((x, y) => x.Amount.CompareTo(y.Amount));

            c.InventoryAllocations?.ForEach(inv =>
            {
                inv.BatchAllocations?.Sort((x, y) => x.GodownName.CompareTo(y.GodownName));
                inv.BatchAllocations?.Sort((x, y) => x.Amount.CompareTo(y.Amount));

                inv.CostCategoryAllocations?.Sort((x, y) => x.CostCategoryName.CompareTo(y.CostCategoryName));

                inv.CostCategoryAllocations?.ForEach(cc =>
                {
                    cc.CostCenterAllocations?.Sort((x, y) => x.Name.CompareTo(y.Name));
                    cc.CostCenterAllocations?.Sort((x, y) => x.Amount.CompareTo(y.Amount));
                });
            });

        });
    }

    public new string GetJson(bool Indented = false)
    {
        OrderLedgers();

        return base.GetJson(Indented);
    }

    public new string GetXML(XmlAttributeOverrides attrOverrides = null)
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
                    DateTime dateTime = DateTime.ParseExact(EffectiveDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                    double days = dateTime.Subtract(new DateTime(1900, 1, 1)).TotalDays + 1;
                    billalloc.BillCP.JD = days.ToString();
                }
            });
        });
    }

    public new void PrepareForExport()
    {
        OrderLedgers(); //Ensures ledgers are ordered in correct way
        GetJulianday();
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


    [XmlElement(ElementName = "LEDGERNAME")]
    public string LedgerName { get; set; }


    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    public string IsDeemedPositive
    {
        get
        {
            if (_Amount != null)
            {
                if (!_Amount.Contains('-'))
                {
                    return "No";
                }
                else
                {
                    return "Yes";
                }
            }
            return null;

        }
        set { }
    }

    private string _Amount;

    public string ForexAmount { get; set; }

    public string RateofExchange { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public string Amount
    {
        get
        {
            if (ForexAmount != null && RateofExchange != null)
            {
                _Amount = $"{ForexAmount} @ {RateofExchange}";
            }
            else if (ForexAmount != null)
            {
                _Amount = ForexAmount;
            }
            return _Amount;
        }
        set
        {
            if (value != null)
            {
                double t_amount;
                if (value.ToString().Contains('='))
                {

                    List<string> SplittedValues = value.ToString().Split('=').ToList();
                    var CleanedAmounts = Regex.Match(SplittedValues[1], @"[0-9.]+");
                    bool Isnegative = SplittedValues[1].Contains('-');
                    bool sucess = Isnegative ? double.TryParse('-' + CleanedAmounts.Value, out t_amount) : double.TryParse(CleanedAmounts.ToString(), out t_amount);
                    CleanedAmount = sucess ? t_amount : null;
                    var ForexInfo = SplittedValues[0].Split('@');
                    ForexAmount = ForexInfo[0].Trim();
                    RateofExchange = Regex.Match(ForexInfo[1], @"[0-9.]+").Value;
                }
                else
                {
                    double.TryParse(value, out t_amount);
                    CleanedAmount = t_amount;
                    _Amount = value;
                }
            }
            else
            {
                _Amount = value;
            }

        }

    }
    [XmlIgnore]
    public double? CleanedAmount { get; set; }


    [XmlElement(ElementName = "BILLALLOCATIONS.LIST")]
    public List<BillAllocations> BillAllocations { get; set; }

    [XmlElement(ElementName = "INVENTORYALLOCATIONS.LIST")]
    public List<InventoryAllocations> InventoryAllocations { get; set; }

    [XmlElement(ElementName = "CATEGORYALLOCATIONS.LIST")]
    public List<CostCategoryAllocations> CostCategoryAllocations { get; set; }



}

[XmlRoot(ElementName = "BILLALLOCATIONS.LIST")]
public class BillAllocations : TallyBaseObject
{
    public BillAllocations()
    {
        _BillCP = new();
    }

    [XmlElement(ElementName = "BILLTYPE")]
    public string BillType { get; set; }

    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    private string _Amount;

    public string ForexAmount { get; set; }

    private BillCP _BillCP;



    [JsonIgnore]
    [XmlElement(ElementName = "BILLCREDITPERIOD")]
    public BillCP BillCP
    {
        get { _BillCP.Days = BillCreditPeriod; return _BillCP; }
        set
        {
            _BillCP = value;
            BillCreditPeriod = value.Days;
        }
    }

    private string _billCreditPeriod;

    [XmlIgnore]
    public string BillCreditPeriod
    {
        get { return _billCreditPeriod; }
        set { _billCreditPeriod = value; }
    }

    public string RateofExchange { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public string Amount
    {
        get
        {
            if (ForexAmount != null && RateofExchange != null)
            {
                _Amount = $"{ForexAmount} @ {RateofExchange}";
            }
            else if (ForexAmount != null)
            {
                _Amount = ForexAmount;
            }
            return _Amount;
        }
        set
        {
            if (value != null)
            {
                if (value.ToString().Contains('='))
                {
                    var s = value.ToString().Split('=');
                    var k = s[0].Split('@');
                    ForexAmount = k[0];
                    RateofExchange = k[1].Split()[2].Split('/')[0];
                    _Amount = s[1].Split()[2];
                }
                else
                {
                    _Amount = value;
                }
            }
            else
            {
                _Amount = value;
            }

        }
    }

}
[XmlRoot(ElementName = "BILLCREDITPERIOD")]
public class BillCP
{
    [XmlAttribute(AttributeName = "JD")]
    public string JD { get; set; }

    private string _days;
    [XmlAttribute(AttributeName = "Days")]
    public string Days
    {
        get { return _days; }
        set { _days = value; }
    }

    [XmlText]
    public string TextValue
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


    [XmlElement(ElementName = "STOCKITEMNAME")]
    public string StockItemName { get; set; }

    [XmlElement(ElementName = "BOMNAME")]
    public string BOMName { get; set; }

    [XmlElement(ElementName = "ISSCRAP")]
    public string IsScrap { get; set; }

    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    public string DeemedPositive { get; set; }

    [XmlElement(ElementName = "RATE")]
    public string Rate { get; set; }

    [XmlElement(ElementName = "ACTUALQTY")]
    public string ActualQuantity { get; set; }

    [XmlElement(ElementName = "BILLEDQTY")]
    public string BilledQuantity { get; set; }

    private string _Amount;


    public string ForexAmount { get; set; }

    public string RateofExchange { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public string Amount
    {
        get
        {
            if (ForexAmount != null && RateofExchange != null)
            {
                _Amount = $"{ForexAmount} @ {RateofExchange}";
            }
            else if (ForexAmount != null)
            {
                _Amount = ForexAmount;
            }
            return _Amount;
        }
        set
        {
            if (value != null)
            {
                if (value.ToString().Contains('='))
                {
                    var s = value.ToString().Split('=');
                    var k = s[0].Split('@');
                    ForexAmount = k[0];
                    RateofExchange = k[1].Split()[2].Split('/')[0];
                    _Amount = s[1].Split()[2];
                }
                else
                {
                    _Amount = value;
                }
            }
            else
            {
                _Amount = value;
            }

        }
    }

    [XmlElement(ElementName = "BATCHALLOCATIONS.LIST")]
    public List<BatchAllocations> BatchAllocations { get; set; }

    [XmlElement(ElementName = "CATEGORYALLOCATIONS.LIST")]
    public List<CostCategoryAllocations> CostCategoryAllocations { get; set; }

}

[XmlRoot(ElementName = "BATCHALLOCATIONS.LIST")]
public class BatchAllocations : TallyBaseObject//Godown Allocations
{

    [XmlElement(ElementName = "TRACKINGNUMBER")]
    public string TrackingNo { get; set; }

    [XmlElement(ElementName = "ORDERNO")]
    public string OrderNo { get; set; }

    [XmlElement(ElementName = "GODOWNNAME")]
    public string GodownName { get; set; }

    [XmlElement(ElementName = "BATCHNAME")]
    public string BatchName { get; set; }


    private string _Amount;

    public string ForexAmount { get; set; }

    public string RateofExchange { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public string Amount
    {
        get
        {
            if (ForexAmount != null && RateofExchange != null)
            {
                _Amount = $"{ForexAmount} @ {RateofExchange}";
            }
            else if (ForexAmount != null)
            {
                _Amount = ForexAmount;
            }
            return _Amount;
        }
        set
        {
            if (value != null)
            {
                if (value.ToString().Contains('='))
                {
                    var s = value.ToString().Split('=');
                    var k = s[0].Split('@');
                    ForexAmount = k[0];
                    RateofExchange = k[1].Split()[2].Split('/')[0];
                    _Amount = s[1].Split()[2];
                }
                else
                {
                    _Amount = value;
                }
            }
            else
            {
                _Amount = value;
            }

        }
    }

    [XmlElement(ElementName = "ACTUALQTY")]
    public string ActualQuantity { get; set; }

    [XmlElement(ElementName = "BILLEDQTY")]
    public string BilledQuantity { get; set; }
}

[XmlRoot(ElementName = "CATEGORYALLOCATIONS.LIST")]
public class CostCategoryAllocations : TallyBaseObject
{
    public CostCategoryAllocations()
    {
    }

    [XmlElement(ElementName = "CATEGORY")]
    public string CostCategoryName { get; set; }

    [XmlElement(ElementName = "COSTCENTREALLOCATIONS.LIST")]
    public List<CostCenterAllocations> CostCenterAllocations { get; set; }

}
[XmlRoot(ElementName = "COSTCENTREALLOCATIONS.LIST")]
public class CostCenterAllocations : TallyBaseObject
{
    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }

    private string _Amount;

    public string ForexAmount { get; set; }

    public string RateofExchange { get; set; }

    [XmlElement(ElementName = "AMOUNT")]
    public string Amount
    {
        get
        {
            if (ForexAmount != null && RateofExchange != null)
            {
                _Amount = $"{ForexAmount} @ {RateofExchange}";
            }
            else if (ForexAmount != null)
            {
                _Amount = ForexAmount;
            }
            return _Amount;
        }
        set
        {
            if (value != null)
            {
                if (value.ToString().Contains('='))
                {
                    var s = value.ToString().Split('=');
                    var k = s[0].Split('@');
                    ForexAmount = k[0];
                    RateofExchange = k[1].Split()[2].Split('/')[0];
                    _Amount = s[1].Split()[2];
                }
                else
                {
                    _Amount = value;
                }
            }
            else
            {
                _Amount = value;
            }

        }
    }


}


[XmlRoot(ElementName = "ATTENDANCEENTRIES.LIST")]
public class AttendanceEntry
{

    [XmlIgnore]
    public string AttdTypeId { get; set; }



    [XmlElement(ElementName = "NAME")]
    public string Name { get; set; }


    [XmlElement(ElementName = "ATTENDANCETYPE")]
    public string AttdType { get; set; }

    [XmlElement(ElementName = "ATTDTYPETIMEVALUE")]
    public string AttdTypeTimeValue { get; set; }

    [XmlElement(ElementName = "ATTDTYPEVALUE")]
    public string AttdTypeValue { get; set; }

}

[XmlRoot(ElementName = "INVOICEDELNOTES.LIST")]
public class DeliveryNotes
{
    [XmlElement(ElementName = "BASICSHIPPINGDATE")]
    public string ShippingDate { get; set; }

    [XmlElement(ElementName = "BASICSHIPDELIVERYNOTE")]
    public string DeliveryNote { get; set; }
}


[XmlRoot(ElementName = "EWAYBILLDETAILS.LIST")]
public class EwayBillDetail : TallyBaseObject
{
    [XmlElement(ElementName = "BILLDATE")]
    public string BillDate { get; set; }

    [XmlElement(ElementName = "CONSOLIDATEDBILLDATE")]
    public string ConsolidatedBillDate { get; set; }

    [XmlElement(ElementName = "BILLNUMBER")]
    public string BillNumber { get; set; }

    [XmlElement(ElementName = "CONSOLIDATEDBILLNUMBER")]
    public string ConsolidatedBillNumber { get; set; }

    [XmlElement(ElementName = "SUBTYPE")]
    public SubSupplyType SubType { get; set; }

    [XmlElement(ElementName = "DOCUMENTTYPE")]
    public DocumentType DocumentType { get; set; }

    [XmlElement(ElementName = "CONSIGNORPLACE")]
    public string DispatchFrom { get; set; }

    [XmlElement(ElementName = "CONSIGNEEPLACE")]
    public string DispatchTo { get; set; }

    [XmlElement(ElementName = "ISCANCELLED")]
    public string IsCancelled { get; set; }

    [XmlElement(ElementName = "ISCANCELPENDING")]
    public string IsCancelledPending { get; set; }

    [XmlElement(ElementName = "TRANSPORTDETAILS.LIST")]
    public List<TransporterDetail> TransporterDetails { get; set; }



}

[XmlRoot(ElementName = "TRANSPORTDETAILS.LIST")]
public class TransporterDetail : TallyBaseObject
{
    [XmlElement(ElementName = "DISTANCE")]
    public string Distance { get; set; }

    [XmlElement(ElementName = "TRANSPORTERNAME")]
    public string TransporterName { get; set; }

    [XmlElement(ElementName = "TRANSPORTERID")]
    public string TransporterId { get; set; }

    [XmlElement(ElementName = "TRANSPORTMODE")]
    public TransportMode TransportMode { get; set; }

    /// <summary>
    /// Document/Landing/RR/Airway Number/ 
    /// </summary>
    [XmlElement(ElementName = "DOCUMENTNUMBER")]
    public string DocumentNumber { get; set; }

    [XmlElement(ElementName = "DOCUMENTDATE")]
    public string DocumentDate { get; set; }

    [XmlElement(ElementName = "VEHICLENUMBER")]
    public string VehicleNumber { get; set; }

    [XmlElement(ElementName = "VEHICLETYPE")]
    public VehicleType VehicleType { get; set; }
}

/// <summary>
/// Voucher Message
/// </summary>




[XmlRoot(ElementName = "ENVELOPE")]
public class VoucherEnvelope : TallyXmlJson
{

    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public VBody Body { get; set; } = new VBody();
}

[XmlRoot(ElementName = "BODY")]
public class VBody
{
    [XmlElement(ElementName = "DESC")]
    public Description Desc { get; set; } = new Description();

    [XmlElement(ElementName = "DATA")]
    public VData Data { get; set; } = new VData();
}

[XmlRoot(ElementName = "DATA")]
public class VData
{
    [XmlElement(ElementName = "TALLYMESSAGE")]
    public VoucherMessage Message { get; set; } = new VoucherMessage();

    [XmlElement(ElementName = "COLLECTION")]
    public VouchColl Collection { get; set; } = new VouchColl();
}

[XmlRoot(ElementName = "COLLECTION")]
public class VouchColl
{
    [XmlElement(ElementName = "VOUCHER")]
    public List<Voucher> Vouchers { get; set; }
}

[XmlRoot(ElementName = "TALLYMESSAGE")]
public class VoucherMessage
{
    [XmlElement(ElementName = "VOUCHER")]
    public Voucher Voucher { get; set; }
}

public enum VoucherLookupField
{
    MasterId = 1,
    AlterId = 2,
    VoucherNumber = 3,
    GUID = 4,
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
    [XmlEnum(Name = "1 - Road")]
    Road = 1,
    [XmlEnum(Name = "2 - Rail")]
    Rail = 2,
    [XmlEnum(Name = "3 - Air")]
    Air = 3,
    [XmlEnum(Name = "4 - Ship")]
    Ship = 4,
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
    [XmlEnum(Name = "R - Regular")]
    Regular = 1,
    [XmlEnum(Name = "Over Dimensional Cargo")]
    OverDimensionalCargo = 2,

}




