namespace TallyConnector.Models.Masters;

[XmlRoot(ElementName = "GROUP")]
public class Group : TallyXmlJson
{
    [XmlElement(ElementName = "MASTERID")]
    [MaxLength(20)]
    public int? TallyId { get; set; }

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
        get { return name == null || name == string.Empty ? OldName : name; }
        set => name = value;
    }

    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = "nvarchar(60)")]
    public string Parent { get; set; }


    [XmlIgnore]
    [Column(TypeName = "nvarchar(60)")]
    public string Alias { get; set; }

    [XmlElement(ElementName = "GUID")]
    [Column(TypeName = "nvarchar(100)")]
    public string GUID { get; set; }
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


    [XmlElement(ElementName = "CANDELETE")]
    [Column(TypeName = "nvarchar(3)")]
    public YesNo CanDelete { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
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
}

[XmlRoot(ElementName = "ENVELOPE")]
public class GroupEnvelope : TallyXmlJson
{

    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public GBody Body { get; set; } = new();
}

[XmlRoot(ElementName = "BODY")]
public class GBody
{
    [XmlElement(ElementName = "DESC")]
    public Description Desc { get; set; } = new();

    [XmlElement(ElementName = "DATA")]
    public GData Data { get; set; } = new();
}

[XmlRoot(ElementName = "DATA")]
public class GData
{
    [XmlElement(ElementName = "TALLYMESSAGE")]
    public GroupMessage Message { get; set; } = new();

    [XmlElement(ElementName = "COLLECTION")]
    public GroupColl Collection { get; set; } = new GroupColl();


}

[XmlRoot(ElementName = "COLLECTION")]
public class GroupColl
{
    [XmlElement(ElementName = "GROUP")]
    public List<Group> Groups { get; set; }
}

[XmlRoot(ElementName = "TALLYMESSAGE")]
public class GroupMessage
{
    [XmlElement(ElementName = "GROUP")]
    public Group Group { get; set; }
}
