namespace TallyConnector.Core.Models.Masters.Inventory;

[XmlRoot(ElementName = "STOCKITEM")]
[XmlType(AnonymousType = true)]
public class StockItem : BasicTallyObject, INamedTallyObject
{
    public StockItem()
    {
        LanguageNameList = new();
    }

    [XmlAttribute(AttributeName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [JsonIgnore]
    public string? OldName { get; set; }

    private string? name;

    [XmlElement(ElementName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [Required]
    public string Name
    {
        get
        {
            name = name == null || name == string.Empty ? OldName : name;
            return name!;
        }
        set => name = value;
    }

    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? StockGroup { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? StockGroupId { get; set; }

    [XmlElement(ElementName = "CATEGORY")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Category { get; set; }

    [XmlElement(ElementName = "CATEGORYID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
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
    public TallyYesNo? IsCostTracking { get; set; }

    [XmlElement(ElementName = "ISCOSTCENTRESON")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsCostCentresOn { get; set; }

    [XmlElement(ElementName = "ISBATCHWISEON")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? MaintainInBranches { get; set; }

    [XmlElement(ElementName = "ISPERISHABLEON")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? UseExpiryDates { get; set; }

    [XmlElement(ElementName = "HASMFGDATE")]
    public TallyYesNo? TrackDateOfManufacturing { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? BaseUnit { get; set; }

    [XmlElement(ElementName = "BASEUNITID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? BaseUnitId { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITS")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? AdditionalUnits { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? AdditionalUnitsId { get; set; }

    [XmlElement(ElementName = "INCLUSIVETAX")]
    public TallyYesNo? InclusiveOfTax { get; set; }

    [XmlElement(ElementName = "DENOMINATOR")]
    public decimal? Denominator { get; set; }

    [XmlElement(ElementName = "CONVERSION")]
    public decimal? Conversion { get; set; }

    [XmlElement(ElementName = "BASICRATEOFEXCISE")]
    public string? RateOfDuty { get; set; }

    [XmlElement(ElementName = "OPENINGBALANCE")]
    public TallyQuantity? OpeningBal { get; set; }

    [XmlElement(ElementName = "OPENINGVALUE")]
    public TallyAmount? OpeningValue { get; set; }

    [XmlElement(ElementName = "OPENINGRATE")]
    public TallyRate? OpeningRate { get; set; }



    [XmlIgnore]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    [XmlElement(ElementName = "MULTICOMPONENTLIST.LIST")]
    public List<ComponentsList>? BOMList { get; set; }

    [XmlElement(ElementName = "GSTDETAILS.LIST")]
    public List<GSTDetail>? GSTDetails { get; set; }

    public void CreateNamesList()
    {
        if (LanguageNameList.Count == 0)
        {
            LanguageNameList.Add(new LanguageNameList());
            LanguageNameList[0].NameList?.NAMES?.Add(Name);

        }
        if (Alias != null && Alias != string.Empty)
        {
            LanguageNameList[0].LanguageAlias = Alias;
        }
    }
    public new string GetXML(XmlAttributeOverrides? attrOverrides = null)
    {
        CreateNamesList();
        return base.GetXML(attrOverrides);
    }

    public new void PrepareForExport()
    {
        if (StockGroup != null && StockGroup.Contains("Primary"))
        {
            StockGroup = null;
        }
        CreateNamesList();
    }

    public override string ToString()
    {
        return $"StockItem - {Name}";
    }
}