using TallyConnector.Models.Common;

namespace TallyConnector.Models.Base.Masters.Inventory;
[XmlRoot(ElementName = "STOCKITEM")]
[XmlType(AnonymousType = true)]
public partial class StockItem : BaseAliasedMasterObject
{
    [XmlElement(ElementName = "PARENT")]
    public string? StockGroup { get; set; }

    [XmlElement(ElementName = "CATEGORY")]
    public string? StockCategory { get; set; }

    [XmlElement(ElementName = "GSTAPPLICABLE")]
    public string? GSTApplicable { get; set; }

    [XmlElement(ElementName = "GSTTYPEOFSUPPLY")]
    public string? GSTTypeOfSupply { get; set; }

    [XmlElement(ElementName = "TCSAPPLICABLE")]
    public string? TCSApplicable { get; set; }

    [XmlElement(ElementName = "DESCRIPTION")]
    public string? Description { get; set; }

    [XmlElement(ElementName = "NARRATION")]
    public string? Narration { get; set; }

    [XmlElement(ElementName = "COSTINGMETHOD")]
    public string? CostingMethod { get; set; }

    [XmlElement(ElementName = "VALUATIONMETHOD")]
    public string? ValuationMethod { get; set; }

    [XmlElement(ElementName = "ISCOSTTRACKINGON")]
    public bool? IsCostTracking { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRESON")]
    public bool? IsCostCentresOn { get; set; }

    [XmlElement(ElementName = "ISBATCHWISEON")]
    public bool? MaintainInBranches { get; set; }

    [XmlElement(ElementName = "ISPERISHABLEON")]
    public bool? UseExpiryDates { get; set; }

    [XmlElement(ElementName = "HASMFGDATE")]
    public bool? TrackDateOfManufacturing { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    public string? BaseUnit { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITS")]
    public string? AdditionalUnits { get; set; }

    [XmlElement(ElementName = "INCLUSIVETAX")]
    public bool? InclusiveOfTax { get; set; }

    [XmlElement(ElementName = "DENOMINATOR")]
    [Column(TypeName = "decimal(9,4)")]
    public decimal? Denominator { get; set; }

    [XmlElement(ElementName = "CONVERSION")]
    [Column(TypeName = "decimal(9,4)")]
    public decimal? Conversion { get; set; }

    [XmlElement(ElementName = "BASICRATEOFEXCISE")]
    public string? RateOfDuty { get; set; }

    [XmlElement(ElementName = "OPENINGBALANCE")]
    public TallyQuantityField? OpeningBal { get; set; }

    [XmlElement(ElementName = "OPENINGVALUE")]
    public TallyAmountField? OpeningValue { get; set; }

    [XmlElement(ElementName = "OPENINGRATE")]
    public TallyRateField? OpeningRate { get; set; }

    /// <summary>
    ///  in Tally UI - Part No and Part No alias
    /// </summary>
    [XmlArray(ElementName = "MAILINGNAME.LIST")]
    [XmlArrayItem(ElementName = "MAILINGNAME")]
    [TDLCollection(CollectionName = "MAILINGNAME", ExplodeCondition = "$$NUMITEMS:MAILINGNAME>0")]
    public List<string>? MailingNames { get; set; }

    [XmlElement(ElementName = "GSTDETAILS.LIST")]
    [TDLCollection(CollectionName = "GSTDETAILS", ExplodeCondition = "$$NUMITEMS:GSTDETAILS>0")]
    public List<GSTDetail>? GSTDetails { get; set; }

    [XmlElement(ElementName = "HSNDETAILS.LIST")]
    [TDLCollection(CollectionName = "HSNDETAILS", ExplodeCondition = "$$NUMITEMS:HSNDETAILS>0")]
    public List<HSNDetail>? HSNDetails { get; set; }

}
