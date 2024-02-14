namespace TallyConnector.Core.Models.Masters.Inventory;

[XmlRoot(ElementName = "STOCKGROUP")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.StockGroups)]
public class StockGroup : BasicTallyObject, IAliasTallyObject
{
    public StockGroup()
    {
        //BaseUnit = "";
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
    public string? Parent { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? ParentId { get; set; }

    [XmlElement(ElementName = "ISADDABLE")]
    public TallyYesNo? IsAddable { get; set; }  //Should Quantities of Items be Added

    [XmlElement(ElementName = "GSTAPPLICABLE")]
    public string? GSTApplicability { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    public string? BaseUnit { get; set; }

    [XmlIgnore]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    [XmlElement(ElementName = "GSTDETAILS.LIST")]
    public List<GSTDetail>? GSTDetails { get; set; }
    
    public override string ToString()
    {
        return $"Stock Group - {Name}";
    }
}
