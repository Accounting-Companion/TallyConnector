namespace TallyConnector.Models.Masters.CostCenter;

[XmlRoot(ElementName = "COSTCATEGORY")]
public class CostCategory : TallyXmlJson, ITallyObject
{
    public CostCategory()
    {
        LanguageNameList = new();
    }

    [XmlElement(ElementName = "MASTERID")]
    public int? TallyId { get; set; }

    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
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

    [XmlElement(ElementName = "ALLOCATEREVENUE")]
    public YesNo AllocateRevenue { get; set; }

    [XmlElement(ElementName = "ALLOCATENONREVENUE")]
    public YesNo AllocateNonRevenue { get; set; }

    [XmlElement(ElementName = "GUID")]
    public string GUID { get; set; }

    [XmlIgnore]
    public string Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    public List<LanguageNameList> LanguageNameList { get; set; }
    /// <summary>
    /// Accepted Values //Create, Alter, Delete
    /// </summary>
    [JsonIgnore]
    [XmlAttribute(AttributeName = "Action")]
    public string Action { get; set; }

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

    public void PrepareForExport()
    {
        CreateNamesList();
    }
}
[XmlRoot(ElementName = "ENVELOPE")]
public class CostCatEnvelope : TallyXmlJson
{

    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public CCBody Body { get; set; } = new();
}

[XmlRoot(ElementName = "BODY")]
public class CCBody
{
    [XmlElement(ElementName = "DESC")]
    public Description Desc { get; set; } = new();

    [XmlElement(ElementName = "DATA")]
    public CCData Data { get; set; } = new();
}

[XmlRoot(ElementName = "DATA")]
public class CCData
{
    [XmlElement(ElementName = "TALLYMESSAGE")]
    public CCMessage Message { get; set; } = new();

    [XmlElement(ElementName = "COLLECTION")]
    public CostCategoryColl Collection { get; set; } = new CostCategoryColl();


}

[XmlRoot(ElementName = "COLLECTION")]
public class CostCategoryColl
{
    [XmlElement(ElementName = "COSTCATEGORY")]
    public List<CostCategory> CostCategories { get; set; }
}

[XmlRoot(ElementName = "TALLYMESSAGE")]
public class CCMessage
{
    [XmlElement(ElementName = "COSTCATEGORY")]
    public CostCategory CostCategory { get; set; }
}


