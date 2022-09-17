namespace TallyConnector.Core.Models.Masters.CostCenter;


[XmlRoot(ElementName = "COSTCENTRE")]
[XmlType(AnonymousType = true)]
public class CostCenter : BasicTallyObject, INamedTallyObject
{
    public CostCenter()
    {
        LanguageNameList = new();
        CategoryId = string.Empty;
        Category = string.Empty;
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

    [XmlElement(ElementName = "CATEGORY")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [Required]
    public string Category { get; set; }

    [XmlElement(ElementName = "CATEGORYID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string CategoryId { get; set; }

    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? ParentId { get; set; }

    [XmlElement(ElementName = "EMAILID")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? EmailId { get; set; }

    [XmlElement(ElementName = "REVENUELEDFOROPBAL")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? ShowOpeningBal { get; set; }


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
            LanguageNameList[0].NameList?.NAMES?.Add(Name);

        }
        if (Alias != null && Alias != string.Empty)
        {
            LanguageNameList![0].LanguageAlias = Alias;
        }
    }
    public new string GetXML(XmlAttributeOverrides? attrOverrides = null)
    {
        if (Parent != null && Parent.Contains("Primary"))
        {
            Parent = null;
        }
        CreateNamesList();
        return base.GetXML(attrOverrides);
    }

    public new void PrepareForExport()
    {
        CreateNamesList();
    }

    public override string ToString()
    {
        return $"Cost Center - {Name}";
    }
}
