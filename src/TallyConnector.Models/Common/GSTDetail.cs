﻿namespace TallyConnector.Models.Common;

/// <summary>
/// Contains GST Details of StockItem , Ledgers ..etc.,
/// </summary>
[XmlRoot(ElementName = "GSTDETAILS.LIST")]
public partial class GSTDetail
{
    [XmlElement(ElementName = "APPLICABLEFROM")]
    public DateTime? ApplicableFrom { get; set; }

    [XmlElement(ElementName = "CALCULATIONTYPE")]
    public string? CalculationType { get; set; }

    [XmlElement(ElementName = "GSTCALCSLABONMRP")]
    public bool CalculateSlabOnMRP { get; set; }

    [XmlElement(ElementName = "GSTNATUREOFTRANSACTION")]
    public GSTNatureOfTransaction NatureOfTransaction { get; set; }


    [XmlElement(ElementName = "HSNCODE")]
    public string? HSNCode { get; set; }

    [XmlElement(ElementName = "HSN")]
    public string? HSNDescription { get; set; }

    [XmlElement(ElementName = "HSNMASTERNAME")]
    public string? HSNMasterName { get; set; }

    [XmlElement(ElementName = "ISNONGSTGOODS")]
    public bool? IsNonGSTGoods { get; set; }

    [XmlElement(ElementName = "TAXABILITY")]
    public GSTTaxabilityType Taxability { get; set; }

    [XmlElement(ElementName = "SRCOFGSTDETAILS")]
    public string? SourceOfGSTDetails { get; set; }

    [XmlElement(ElementName = "ISREVERSECHARGEAPPLICABLE")]
    public bool? IsReverseChargeApplicable { get; set; }

    [XmlElement(ElementName = "GSTINELIGIBLEITC")]
    public bool IsInEligibleforITC { get; set; }

    [XmlElement(ElementName = "INCLUDEEXPFORSLABCALC")]
    public bool? IncludeExpForSlabCalc { get; set; }

    //[XmlElement(ElementName = "STATEWISEDETAILS.LIST")]
    //public List<StateWiseDetail>? StateWiseDetails { get; set; }

}

[XmlRoot(ElementName = "STATEWISEDETAILS.LIST")]
public partial class StateWiseDetail
{
    [XmlElement(ElementName = "STATENAME")]
    public string? StateName { get; set; }

    [XmlElement(ElementName = "RATEDETAILS.LIST")]
    public List<GSTRateDetail>? GSTRateDetails { get; set; }


}

[XmlRoot(ElementName = "RATEDETAILS.LIST")]
public partial class GSTRateDetail
{
    [XmlElement(ElementName = "GSTRATEDUTYHEAD")]
    public string? DutyHead { get; set; }

    [XmlElement(ElementName = "GSTRATEVALUATIONTYPE")]
    public string? ValuationType { get; set; }

    [XmlElement(ElementName = "GSTRATE")]
    public float GSTRate { get; set; }
}

public enum GSTNatureOfTransaction
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Branch Transfer Outward")]
    BranchTransferOutward,
    [EnumXMLChoice(Choice = "Local Sales - Exempt")]
    LocalSalesExempt,
    [EnumXMLChoice(Choice = "Local Sales - Nil Rated")]
    LocalSalesNilRated,
    [EnumXMLChoice(Choice = "Local Sales - Taxable")]
    LocalSalesTaxable,
    [EnumXMLChoice(Choice = "Interstate Sales - Exempt")]
    InterstateSalesExempt,
    [EnumXMLChoice(Choice = "Interstate Sales - Nil Rated")]
    InterstateSalesNilRated,
    [EnumXMLChoice(Choice = "Interstate Sales - Taxable")]
    InterstateSalesTaxable,
    [EnumXMLChoice(Choice = "Interstate Deemed Exports - Exempt")]
    InterstateDeemedExportsExempt,
    [EnumXMLChoice(Choice = "Interstate Deemed Exports - Nil Rated")]
    InterstateDeemedExportsNilRated,
    [EnumXMLChoice(Choice = "Interstate Deemed Exports - Taxable")]
    InterstateDeemedExportsTaxable,
    [EnumXMLChoice(Choice = "Local Deemed Exports - Exempt")]
    LocalDeemedExportsExempt,
    [EnumXMLChoice(Choice = "Local Deemed Exports - Nil Rated")]
    LocalDeemedExportsNilRated,
    [EnumXMLChoice(Choice = "Local Deemed Exports - Taxable")]
    LocalDeemedExportsTaxable,
    [EnumXMLChoice(Choice = "Exports - Exempt")]
    ExportsExempt,
    [EnumXMLChoice(Choice = "Exports - Nil Rated")]
    ExportsNilRated,
    [EnumXMLChoice(Choice = "Exports - Taxable")]
    ExportsTaxable,
    [EnumXMLChoice(Choice = "Exports - LUT/Bond")]
    Exports_LUT_Bond,
    [EnumXMLChoice(Choice = "Sales to SEZ - Exempt")]
    SalestoSEZExempt,
    [EnumXMLChoice(Choice = "Sales to SEZ - Nil Rated")]
    SalestoSEZNilRated,
    [EnumXMLChoice(Choice = "Sales to SEZ - Taxable")]
    SalestoSEZTaxable,
    [EnumXMLChoice(Choice = "Sales to SEZ - LUT/Bond")]
    SalestoSEZ_LUT_Bond,
    [EnumXMLChoice(Choice = "High Sea Sales")]
    HighSeaSales,
    [EnumXMLChoice(Choice = "Sales from Customs Bonded Warehouse")]
    SalesfromCustomsBondedWarehouse,
    [EnumXMLChoice(Choice = "Branch Transfer Inward")]
    BranchTransferInward,
    [EnumXMLChoice(Choice = "Local Purchase - Exempt")]
    LocalPurchaseExempt,
    [EnumXMLChoice(Choice = "Local Purchase - Nil Rated")]
    LocalPurchaseNilRated,
    [EnumXMLChoice(Choice = "Local Purchase - Taxable")]
    LocalPurchaseTaxable,
    [EnumXMLChoice(Choice = "Interstate Purchase - Exempt")]
    InterstatePurchaseExempt,
    [EnumXMLChoice(Choice = "Interstate Purchase - Nil Rated")]
    InterstatePurchaseNilRated,
    [EnumXMLChoice(Choice = "Interstate Purchase - Taxable")]
    InterstatePurchaseTaxable,
    [EnumXMLChoice(Choice = "Local Purchase Deemed Exports - Exempt")]
    LocalPurchaseDeemedExportsExempt,
    [EnumXMLChoice(Choice = "Local Purchase Deemed Exports - Nil Rated")]
    LocalPurchaseDeemedExportsNilRated,
    [EnumXMLChoice(Choice = "Local Purchase Deemed Exports - Taxable")]
    LocalPurchaseDeemedExportsTaxable,
    [EnumXMLChoice(Choice = "Interstate Purchase Deemed Exports - Exempt")]
    InterstatePurchaseDeemedExportsExempt,
    [EnumXMLChoice(Choice = "Interstate Purchase Deemed Exports - Nil Rated")]
    InterstatePurchaseDeemedExportsNilRated,
    [EnumXMLChoice(Choice = "Interstate Purchase Deemed Exports - Taxable")]
    InterstatePurchaseDeemedExportsTaxable,
    [EnumXMLChoice(Choice = "Purchase from Composition Dealer")]
    PurchasefromCompositionDealer,
    [EnumXMLChoice(Choice = "Imports - Exempt")]
    ImportsExempt,
    [EnumXMLChoice(Choice = "Imports - Nil Rated")]
    ImportsNilRated,
    [EnumXMLChoice(Choice = "Imports - Taxable")]
    ImportsTaxable,
    [EnumXMLChoice(Choice = "Purchase from SEZ - Exempt")]
    PurchasefromSEZExempt,
    [EnumXMLChoice(Choice = "Purchase from SEZ - Nil Rated")]
    PurchasefromSEZNilRated,
    [EnumXMLChoice(Choice = "Purchase from SEZ - Taxable")]
    PurchasefromSEZTaxable,
    [EnumXMLChoice(Choice = "Purchase from SEZ - LUT/Bond")]
    PurchasefromSEZLUTBond,
    [EnumXMLChoice(Choice = "Purchase from SEZ (Without Bill of Entry) - Exempt")]
    PurchasefromSEZWithoutBillofEntryExempt,
    [EnumXMLChoice(Choice = "Purchase from SEZ (Without Bill of Entry) - Nil Rated")]
    PurchasefromSEZWithoutBillofEntryNilRated,
    [EnumXMLChoice(Choice = "Purchase from SEZ (Without Bill of Entry) - Taxable")]
    PurchasefromSEZWithoutBillofEntryTaxable,
    [EnumXMLChoice(Choice = "High Sea Purchases")]
    HighSeaPurchases,
    [EnumXMLChoice(Choice = "Purchase from Customs Bonded Warehouse")]
    PurchasefromCustomsBondedWarehouse,
    [EnumXMLChoice(Choice = "Local Sales - Non GST")]
    LocalSalesNonGST,
    [EnumXMLChoice(Choice = "Local Deemed Exports - Non GST")]
    LocalDeemedExportsNonGST,
    [EnumXMLChoice(Choice = "Interstate Sales - Non GST")]
    InterstateSalesNonGST,
    [EnumXMLChoice(Choice = "Interstate Deemed Exports - Non GST")]
    InterstateDeemedExportsNonGST,
    [EnumXMLChoice(Choice = "Exports - Non GST")]
    ExportsNonGST,
    [EnumXMLChoice(Choice = "Sales to SEZ - Non GST")]
    SalesToSEZNonGST,
    [EnumXMLChoice(Choice = "Local Purchase - Non GST")]
    LocalPurchaseNonGST,
    [EnumXMLChoice(Choice = "Interstate Purchase - Non GST")]
    InterstatePurchaseNonGST,
    [EnumXMLChoice(Choice = "Local Purchase Deemed Exports - Non GST")]
    LocalPurchaseDeemedExportsNonGST,
    [EnumXMLChoice(Choice = "Interstate Purchase Deemed Exports - Non GST")]
    InterstatePurchaseDeemedExportsNonGST,
    [EnumXMLChoice(Choice = "Imports - Non GST")]
    ImportsNonGST,
    [EnumXMLChoice(Choice = "DTA Imports - Non GST")]
    DTAImportsNonGST,
    [EnumXMLChoice(Choice = "DTA Exports - Non GST")]
    DTAExportsNonGST,
    [EnumXMLChoice(Choice = "Purchase from SEZ - Non GST")]
    PurchasefromSEZNonGST,
    [EnumXMLChoice(Choice = "Purchase from SEZ (Without Bill of Entry) - Non GST")]
    PurchasefromSEZWithoutBillofEntryNonGST,
}
/// <summary>
/// <para> GST TaxabilityTypes as per  Tally</para>
///  <para>TDL Reference -  "DEFTDL:src\master\gstclassification\gstclassificationcoll.tdl"
///  Search using "GSTTaxType"</para>
/// </summary>
public enum GSTTaxabilityType
{
    [EnumXMLChoice(Choice = "")]
    None = 0,
    [EnumXMLChoice(Choice = "Taxable")]
    Taxable = 1,
    [EnumXMLChoice(Choice = "Exempt")]
    Exempt = 2,
    [EnumXMLChoice(Choice = "Nil Rated")]
    NilRated = 3,
    [EnumXMLChoice(Choice = "Unknown")]
    Unknown = 4,
    [EnumXMLChoice(Choice = "Non-GST")]
    NONGST = 5,

}
/// <summary>
/// GST Registration details of party ledgers
/// </summary>
[XmlRoot(ElementName = "LEDGSTREGDETAILS.LIST")]
public partial class LedgerGSTRegistrationDetail
{
    [XmlElement("APPLICABLEFROM")]
    public DateTime ApplicableFrom { get; set; }

    [XmlElement("GSTREGISTRATIONTYPE")]
    public GSTRegistrationType GSTRegistrationType { get; set; }

    [XmlElement("STATE")]
    public string? State { get; set; }

    [XmlElement("PLACEOFSUPPLY")]
    public string? PlaceOfSupply { get; set; }

    [XmlElement("ISOTHTERRITORYASSESSEE")]
    public bool? IsOtherTerritoryAssesse { get; set; }

    [XmlElement("CONSIDERPURCHASEFOREXPORT")]
    public bool ConsiderPurchaseForExport { get; set; }

    [XmlElement("ISTRANSPORTER")]
    public bool IsTransporter { get; set; }

    [XmlElement("TRANSPORTERID")]
    public string? TransporterId { get; set; }

    [XmlElement("ISCOMMONPARTY")]
    public bool? IsCommonParty { get; set; }

    [XmlElement("GSTIN")]
    public string? GSTIN { get; set; }
}


public partial class HSNDetail
{
    [XmlElement("APPLICABLEFROM")]
    public DateTime ApplicableFrom { get; set; }

    [XmlElement("HSN")]
    public string HSNDescription { get; set; }

    [XmlElement("HSNCODE")]
    public string HSNCode { get; set; }

    [XmlElement("HSNCLASSIFICATIONNAME")]
    public string HSNClassificationName { get; set; }

    [XmlElement("SRCOFHSNDETAILS")]
    public string Source { get; set; }
}