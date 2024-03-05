using TallyConnector.Core.Models.Interfaces.Masters;

namespace TallyConnector.Core.Models.Masters;

[XmlRoot("GROUP")]
[TallyObjectType(TallyObjectType.Groups)]
public class BaseGroup :  BaseMasterObject, IBaseGroup
{
   

    public BaseGroup()
    {
        
    }

    /// <summary>
    /// Create New Group Under Primary
    /// </summary>
    /// <param name="name">Name Of the Group</param>
    public BaseGroup(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Creates New Group under mention Group group
    /// </summary>
    /// <param name="name">Name Of the Group</param>
    /// <param name="parent">Name of Base Group</param>
    public BaseGroup(string name, string parent)
    {
        Name = name;
        Parent = parent;
    }

    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Parent { get; set; }

    
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

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:Group:$Parent")]
    public string? ParentId { get; set; }


    [XmlElement(ElementName = "PRIMARYGROUP")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [TDLField(Set = "$_PrimaryGroup")]
    public string? PrimaryGroup { get; set; }


    [XmlElement(ElementName = "RESERVENAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? ReserveName { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }
    //Use Old Name Only When you are altering Existing Group
    [XmlElement(ElementName = "OLDNAME")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [TDLField(Set ="$Name")]
    public string? OldName { get; set; }



    [XmlIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [TDLField(Set = "$_FirstAlias", IncludeInFetch = true)]
    public string? Alias { get; set; }
    /// <summary>
    /// Tally Field - Used for Calculation
    /// </summary>
    [XmlElement(ElementName = "BASICGROUPISCALCULABLE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsCalculable { get; set; }

    /// <summary>
    /// Tally Field - Net Debit/Credit Balances for Reporting 
    /// </summary>
    [XmlElement(ElementName = "ISADDABLE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsAddable { get; set; }

    /// <summary>
    /// Tally Field - Method to Allocate when used in purchase invoice
    /// </summary>
    [XmlElement(ElementName = "ADDLALLOCTYPE")]
    [Column(TypeName = "nvarchar(25)")]
    public AdAllocType? AddlAllocType { get; set; }

    [XmlElement(ElementName = "ISSUBLEDGER")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsSubledger { get; set; }

    [XmlElement(ElementName = "ISREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsRevenue { get; set; }

    [XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? IsDeemedPositive { get; set; }

    [XmlElement(ElementName = "AFFECTSGROSSPROFIT")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? AffectGrossProfit { get; set; }


    [XmlElement(ElementName = "CANDELETE")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? CanDelete { get; set; }



}
