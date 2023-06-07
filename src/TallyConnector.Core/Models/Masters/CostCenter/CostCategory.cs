namespace TallyConnector.Core.Models.Masters.CostCenter;


[XmlRoot(ElementName = "COSTCATEGORY")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.CostCategories)]
public class CostCategory : BasicTallyObject, IAliasTallyObject
{
    public CostCategory()
    {
        LanguageNameList = new();
    }
    public CostCategory(string name)
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

    [XmlElement(ElementName = "ALLOCATEREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? AllocateRevenue { get; set; }

    [XmlElement(ElementName = "ALLOCATENONREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? AllocateNonRevenue { get; set; }

    [XmlIgnore]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }


    public void CreateNamesList()
    {
        if (LanguageNameList.Count == 0)
        {
            LanguageNameList.Add(new LanguageNameList());
            LanguageNameList?[0]?.NameList?.NAMES?.Add(Name);

        }
        if (Alias != null && Alias != string.Empty)
        {
            LanguageNameList![0].LanguageAlias = Alias;
        }
    }
    public new string GetXML(XmlAttributeOverrides? attrOverrides = null, bool indent = false)
    {
        CreateNamesList();
        return base.GetXML(attrOverrides, indent);
    }

    public new void PrepareForExport()
    {
        CreateNamesList();
    }
    public override void RemoveNullChilds()
    {
        base.RemoveNullChilds();
    }
    public override string ToString()
    {
        return $"Cost Category - {Name}";
    }
}
