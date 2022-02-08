namespace TallyConnector.Models.Masters.Inventory;

[XmlRoot(ElementName = "GODOWN")]
public class Godown : TallyXmlJson, ITallyObject
{

    public Godown()
    {
        FAddress = new();
    }

    [XmlElement(ElementName = "MASTERID")]
    public int? TallyId { get; set; }

    [XmlAttribute(AttributeName = "NAME")]
    public string OldName { get; set; }

    private string name;

    [XmlElement(ElementName = "NAME")]
    [Required]
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
    public string Parent { get; set; }
    [JsonIgnore]
    [XmlElement(ElementName = "ADDRESS.LIST")]
    public HAddress FAddress { get; set; }


    [XmlIgnore]
    public string Address
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
    public string PinCode { get; set; }

    [XmlElement(ElementName = "PHONENUMBER")]
    public string PhoneNumber { get; set; }

    [XmlElement(ElementName = "ISEXTERNAL")]
    public string IsExternal { get; set; } // ThirdParty Stock with Us

    [XmlElement(ElementName = "ISINTERNAL")]
    public string IsInternal { get; set; } // Our Stock With Third Party

    [XmlElement(ElementName = "CANDELETE")]
    public string CanDelete { get; set; }


    [XmlIgnore]
    [Column(TypeName = "nvarchar(60)")]
    public string Alias { get; set; }


    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    public List<LanguageNameList> LanguageNameList { get; set; }
    /// <summary>
    /// Accepted Values //Create, Alter, Delete
    /// </summary>
    [JsonIgnore]
    [XmlAttribute(AttributeName = "Action")]
    public YesNo Action { get; set; }

    [XmlElement(ElementName = "GUID")]
    [Column(TypeName = "nvarchar(100)")]
    public string GUID { get; set; }

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
}

[XmlRoot(ElementName = "ENVELOPE")]
public class GodownEnvelope : TallyXmlJson
{

    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public GdwnBody Body { get; set; } = new();
}

[XmlRoot(ElementName = "BODY")]
public class GdwnBody
{
    [XmlElement(ElementName = "DESC")]
    public Description Desc { get; set; } = new();

    [XmlElement(ElementName = "DATA")]
    public GdwnData Data { get; set; } = new();
}

[XmlRoot(ElementName = "DATA")]
public class GdwnData
{
    [XmlElement(ElementName = "TALLYMESSAGE")]
    public GdwnMessage Message { get; set; } = new();

    [XmlElement(ElementName = "COLLECTION")]
    public GodownColl Collection { get; set; } = new GodownColl();


}

[XmlRoot(ElementName = "COLLECTION")]
public class GodownColl
{
    [XmlElement(ElementName = "GODOWN")]
    public List<Godown> Godowns { get; set; }
}

[XmlRoot(ElementName = "TALLYMESSAGE")]
public class GdwnMessage
{
    [XmlElement(ElementName = "GODOWN")]
    public Godown Godown { get; set; }
}
