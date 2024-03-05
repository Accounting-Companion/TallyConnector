namespace TallyConnector.Core.Models.Masters.CostCenter;


[XmlRoot(ElementName = "COSTCATEGORY")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.CostCategories)]
public class CostCategory : BaseMasterObject
{
    public CostCategory()
    {
        LanguageNameList = [];
    }
    public CostCategory(string name)
    {
        LanguageNameList = [];
        Name = name;
    }

    [XmlElement(ElementName = "OLDNAME")]
    [TDLField(Set = "$Name")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string OldName { get; set; }

 

    [XmlElement(ElementName = "ALLOCATEREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? AllocateRevenue { get; set; }

    [XmlElement(ElementName = "ALLOCATENONREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? AllocateNonRevenue { get; set; }

    [XmlIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [TDLField(Set = "$_FirstAlias",IncludeInFetch =true)]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    public override string ToString()
    {
        return $"Cost Category - {Name}";
    }
}
