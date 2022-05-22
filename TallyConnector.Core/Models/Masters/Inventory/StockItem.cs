namespace TallyConnector.Core.Models.Masters.Inventory;

[XmlRoot(ElementName = "STOCKITEM")]
[XmlType(AnonymousType = true)]
public class StockItem : BasicTallyObject, ITallyObject
{
    public StockItem()
    {
        LanguageNameList = new();
    }

    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    public string? OldName { get; set; }

    private string? name;

    [XmlElement(ElementName = "NAME")]
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
    public string? StockGroup { get; set; }

    [XmlElement(ElementName = "CATEGORY")]
    public string? Category { get; set; }

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

    [XmlElement(ElementName = "ISCOSTCENTRESON")]
    public string? IsCostTracking { get; set; }

    [XmlElement(ElementName = "ISBATCHWISEON")]
    public string? MaintainInBranches { get; set; }

    [XmlElement(ElementName = "ISPERISHABLEON")]
    public string? UseExpiryDates { get; set; }

    [XmlElement(ElementName = "HASMFGDATE")]
    public string? TrackDateOfManufacturing { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    public string? BaseUnit { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITS")]
    public string? AdditionalUnits { get; set; }

    [XmlElement(ElementName = "INCLUSIVETAX")]
    public string? InclusiveOfTax { get; set; }


    [XmlElement(ElementName = "CONVERSION")]
    public string? Conversion { get; set; }

    [XmlElement(ElementName = "BASICRATEOFEXCISE")]
    public string? RateOfDuty { get; set; }

    [XmlElement(ElementName = "OPENINGBALANCE")]
    public string? OpeningBal { get; set; }

    [XmlElement(ElementName = "OPENINGVALUE")]
    public string? OpeningValue { get; set; }

    [XmlElement(ElementName = "OPENINGRATE")]
    public string? OpeningRate { get; set; }



    [XmlIgnore]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    [XmlElement(ElementName = "MULTICOMPONENTLIST.LIST")]
    public List<ComponentsList>? BOMList { get; set; }

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
        return $"{Name}";
    }
}