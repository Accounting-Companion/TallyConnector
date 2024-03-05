namespace TallyConnector.Core.Models.Masters.Inventory;

[XmlRoot(ElementName = "STOCKCATEGORY")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.StockCategories)]
public class StockCategory : BaseMasterObject
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


    [XmlElement(ElementName = "OLDNAME")]
    [TDLField(Set = "$Name")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string OldName { get; set; }



    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:StockCategory:$Parent")]
    public string? ParentId { get; set; }

    [XmlIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [TDLField(Set = "$_FirstAlias", IncludeInFetch = true)]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    public override string ToString()
    {
        return $"Stock Category - {Name}";
    }
}
