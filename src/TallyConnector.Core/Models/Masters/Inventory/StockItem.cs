using TallyConnector.Core.Models.TallyComplexObjects;

namespace TallyConnector.Core.Models.Masters.Inventory;

[XmlRoot(ElementName = "STOCKITEM")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.StockItems)]
public class StockItem : BaseMasterObject
{
    public StockItem()
    {
        LanguageNameList = new();
    }

    [XmlElement(ElementName = "OLDNAME")]
    [TDLField(Set = "$Name")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string OldName { get; set; }



    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? StockGroup { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:StockCategory:$Parent")]
    public string? StockGroupId { get; set; }

    [XmlElement(ElementName = "CATEGORY")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Category { get; set; }

    [XmlElement(ElementName = "CATEGORYID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:StockCategory:$Category")]
    public string? CategoryId { get; set; }

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
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsCostTracking { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRESON")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsCostCentresOn { get; set; }

    [XmlElement(ElementName = "ISBATCHWISEON")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? MaintainInBranches { get; set; }

    [XmlElement(ElementName = "ISPERISHABLEON")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? UseExpiryDates { get; set; }

    [XmlElement(ElementName = "HASMFGDATE")]
    public bool? TrackDateOfManufacturing { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? BaseUnit { get; set; }

    [XmlElement(ElementName = "BASEUNITID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:Unit:$BaseUnits")]
    public string? BaseUnitId { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITS")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? AdditionalUnits { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:Unit:$AdditionalUnits")]
    public string? AdditionalUnitsId { get; set; }

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



    [XmlIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [TDLField(Set = "$_FirstAlias")]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    /// <summary>
    ///  in Tally UI - Part No and Part No alias
    /// </summary>
    [XmlArray(ElementName = "MAILINGNAME.LIST")]
    [XmlArrayItem(ElementName = "MAILINGNAME")]
    public List<string> MailingNames { get; set; }

    //[XmlElement(ElementName = "MULTICOMPONENTLIST.LIST")]
    //public List<ComponentsList>? BOMList { get; set; }

    //[XmlElement(ElementName = "GSTDETAILS.LIST")]
    //public List<GSTDetail>? GSTDetails { get; set; }

    //[XmlElement(ElementName = "HSNDETAILS.LIST")]
    //public List<HSNDetail>? HSNDetails { get; set; }


    public override string ToString()
    {
        return $"StockItem - {Name}";
    }
}
[XmlRoot(ElementName = "HSNDETAILS.LIST")]
public class HSNDetail : TallyBaseObject, ICheckNull
{
    [XmlElement(ElementName = "APPLICABLEFROM")]
    public TallyDate? ApplicableFrom { get; set; }

    [XmlElement(ElementName = "HSNCODE")]
    public string? HSNCode { get; set; }

    [XmlElement(ElementName = "HSN")]
    public string? HSNDescription { get; set; }

    [XmlElement(ElementName = "SRCOFHSNDETAILS")]
    public string SourceOfHSNDetails { get; set; }
    public bool IsNull()
    {
        return false;
    }
}