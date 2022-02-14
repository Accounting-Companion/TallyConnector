namespace TallyConnector.Models.Masters;


[XmlRoot(ElementName = "GROUP")]
[XmlType(AnonymousType = true)]
public class Group : BasicTallyObject, ITallyObject
{

    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    [Column(TypeName = "nvarchar(60)")]
    public string OldName { get; set; }

    private string name;

    public Group()
    {
        LanguageNameList = new();
    }

    [XmlElement(ElementName = "NAME")]
    [Required]
    [Column(TypeName = "nvarchar(60)")]
    public string Name
    {
        get
        {
            name = (name == null || name == string.Empty) ? OldName : name;
            return name;
        }
        set => name = value;
    }

    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = "nvarchar(60)")]
    public string Parent { get; set; }


    [XmlIgnore]
    [Column(TypeName = "nvarchar(60)")]
    public string Alias { get; set; }


    /// <summary>
    /// Tally Field - Used for Calculation
    /// </summary>
    [XmlElement(ElementName = "BASICGROUPISCALCULABLE")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo IsCalculable { get; set; }

    /// <summary>
    /// Tally Field - Net Debit/Credit Balances for Reporting 
    /// </summary>
    [XmlElement(ElementName = "ISADDABLE")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo IsAddable { get; set; }

    /// <summary>
    /// Tally Field - Method to Allocate when used in purchase invoice
    /// </summary>
    [XmlElement(ElementName = "ADDLALLOCTYPE")]
    [Column(TypeName = "nvarchar(25)")]
    public AdAllocType AddLAllocType { get; set; }

    [XmlElement(ElementName = "ISSUBLEDGER")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo IsSubledger { get; set; }

    [XmlElement(ElementName = "ISREVENUE")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo IsRevenue { get; set; }

    [XmlElement(ElementName = "AFFECTSGROSSPROFIT")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo AffectGrossProfit { get; set; }


    [XmlElement(ElementName = "CANDELETE")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo CanDelete { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    /// <summary>
    /// Accepted Values //Create, Alter, Delete
    /// </summary>
    [JsonIgnore]
    [XmlAttribute(AttributeName = "Action")]
    public Action Action { get; set; }

    public void CreateNamesList()
    {
        if (LanguageNameList.Count == 0)
        {
            LanguageNameList.Add(new LanguageNameList());
            LanguageNameList[0].NameList.NAMES.Add(Name);

        }
        if (Alias != null && Alias != string.Empty)
        {
            LanguageNameList[0].LanguageAlias = Alias;
        }
    }

    public new string GetXML(XmlAttributeOverrides attrOverrides = null)
    {
        CreateNamesList();
        return base.GetXML(attrOverrides);
    }

    public new void PrepareForExport()
    {
        if (Parent != null && Parent.Contains("Primary"))
        {
            Parent = null;
        }
        if (Name == string.Empty || Name == null)
        {
            Name = OldName;
        }
        //Creates Names List if Not Exists
        CreateNamesList();
    }
}