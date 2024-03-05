namespace TallyConnector.Core.Models.Masters.Inventory;

[XmlRoot(ElementName = "STOCKGROUP")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.StockGroups)]
public class StockGroup : BaseMasterObject
{
    public StockGroup()
    {
        //BaseUnit = "";
        LanguageNameList = new();
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

    [XmlElement(ElementName = "ISADDABLE")]
    public bool? IsAddable { get; set; }  //Should Quantities of Items be Added

    [XmlElement(ElementName = "GSTAPPLICABLE")]
    public string? GSTApplicability { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    public string? BaseUnit { get; set; }

    [XmlIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [TDLField(Set = "$_FirstAlias", IncludeInFetch = true)]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    //[XmlElement(ElementName = "GSTDETAILS.LIST")]
    //public List<GSTDetail>? GSTDetails { get; set; }

    public override string ToString()
    {
        return $"Stock Group - {Name}";
    }
}
