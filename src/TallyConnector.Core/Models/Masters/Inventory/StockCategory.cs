namespace TallyConnector.Core.Models.Masters.Inventory;

[XmlRoot(ElementName = "STOCKCATEGORY")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.StockCategories)]
public class StockCategory : BasicTallyObject, IAliasTallyObject
{
    public StockCategory()
    {
        LanguageNameList = new();
    }

    public StockCategory(string name)
    {
        LanguageNameList = new();
        Name = name;
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

    [XmlIgnore]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }


    public override void RemoveNullChilds()
    {
        Name = name!;
    }
    public override string ToString()
    {
        return $"Stock Category - {Name}";
    }
}
