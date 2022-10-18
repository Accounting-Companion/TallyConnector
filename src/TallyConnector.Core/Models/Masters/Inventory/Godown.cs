namespace TallyConnector.Core.Models.Masters.Inventory;


[XmlRoot(ElementName = "GODOWN")]
[XmlType(AnonymousType = true)]
public class Godown : BasicTallyObject, IAliasTallyObject
{
    public Godown()
    {
        FAddress = new();
        LanguageNameList = new();
    }



    [XmlAttribute(AttributeName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
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
    [JsonIgnore]
    [XmlElement(ElementName = "ADDRESS.LIST")]
    public HAddress FAddress { get; set; }


    [XmlIgnore]
    public string? Address
    {
        get
        {
            return FAddress.FullAddress;
        }

        set
        {
            FAddress = new();
            FAddress.FullAddress = value;

        }

    }

    [XmlElement(ElementName = "PINCODE")]
    public string? PinCode { get; set; }

    [XmlElement(ElementName = "PHONENUMBER")]
    public string? PhoneNumber { get; set; }

    [XmlElement(ElementName = "ISEXTERNAL")]
    [Column(TypeName = "nvarchar(3)")]
    public TallyYesNo? IsExternal { get; set; } // ThirdParty Stock with Us

    [XmlElement(ElementName = "ISINTERNAL")]
    public TallyYesNo? IsInternal { get; set; } // Our Stock With Third Party

    [XmlElement(ElementName = "CANDELETE")]
    public TallyYesNo? CanDelete { get; set; }


    [XmlIgnore]
    [Column(TypeName = "nvarchar(60)")]
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
            Parent = null;
        }
        CreateNamesList();
    }

    public override string ToString()
    {
        return $"Godown - {Name}";
    }
}
