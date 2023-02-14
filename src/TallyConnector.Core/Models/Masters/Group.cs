namespace TallyConnector.Core.Models.Masters;

[XmlRoot("GROUP")]
public class BaseGroup : BasicTallyObject, IAliasTallyObject
{
    private string? name;

    public BaseGroup()
    {
        LanguageNameList = new();
    }



    /// <summary>
    /// Create New Group Under Primary
    /// </summary>
    /// <param name="name">Name Of the Group</param>
    public BaseGroup(string name)
    {
        Name = name;
        LanguageNameList = new();
    }

    /// <summary>
    /// Creates New Group under mention Parent group
    /// </summary>
    /// <param name="name">Name Of the Group</param>
    /// <param name="parent">Name of Base Group</param>
    public BaseGroup(string name, string parent)
    {
        Name = name;
        LanguageNameList = new();
        Parent = parent;
    }


    //Use Old Name Only When you are altering Existing Group
    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? OldName { get; set; }

    [XmlElement(ElementName = "NAME")]
    [Required]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string Name
    {
        get
        {
            name = name == null || name == string.Empty ? OldName : name;
            return name!;
        }
        set => name = value;
    }


    [XmlIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Alias { get; set; }


    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLXMLSet(Set = "$GUID:Group:$Parent")]
    public string? ParentId { get; set; }

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
            LanguageNameList[0].LanguageAlias = Alias;
        }
    }

    public new string GetXML(XmlAttributeOverrides? attrOverrides = null, bool indent = false)
    {
        CreateNamesList();
        return base.GetXML(attrOverrides, indent);
    }

    public new void PrepareForExport()
    {
        if (Parent != null && Parent.Contains("Primary"))
        {
            Parent = string.Empty;
        }
        if (Name == string.Empty || Name == null)
        {
            Name = OldName!;
        }
        //Creates Names List if Not Exists
        CreateNamesList();
    }


    public override string ToString()
    {
        return $"Group - {Name}";
    }

}
[XmlRoot("GROUP")]
public partial class Group : BaseGroup
{
    public Group() : base()
    {

    }

    public Group(string name) : base(name)
    {
    }
    public Group(string name, string parent) : base(name, parent)
    {
    }

    /// <summary>
    /// Tally Field - Used for Calculation
    /// </summary>
    [XmlElement(ElementName = "BASICGROUPISCALCULABLE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsCalculable { get; set; }

    /// <summary>
    /// Tally Field - Net Debit/Credit Balances for Reporting 
    /// </summary>
    [XmlElement(ElementName = "ISADDABLE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsAddable { get; set; }

    /// <summary>
    /// Tally Field - Method to Allocate when used in purchase invoice
    /// </summary>
    [XmlElement(ElementName = "ADDLALLOCTYPE")]
    [Column(TypeName = "nvarchar(25)")]
    public AdAllocType? AddlAllocType { get; set; }

    [XmlElement(ElementName = "ISSUBLEDGER")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsSubledger { get; set; }

    [XmlElement(ElementName = "ISREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsRevenue { get; set; }

    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsDeemedPositive { get; set; }

    [XmlElement(ElementName = "AFFECTSGROSSPROFIT")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? AffectGrossProfit { get; set; }


    [XmlElement(ElementName = "CANDELETE")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? CanDelete { get; set; }



}