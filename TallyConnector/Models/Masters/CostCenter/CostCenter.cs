namespace TallyConnector.Models.Masters.CostCenter;

[XmlRoot(ElementName = "COSTCENTRE")]
public class CostCenter : TallyXmlJson, ITallyObject
{
    public CostCenter()
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

    [XmlElement(ElementName = "CATEGORY")]
    public string Category { get; set; }

    [XmlElement(ElementName = "PARENT")]
    public string Parent { get; set; }

    [XmlElement(ElementName = "EMAILID")]
    public string Emailid { get; set; }

    [XmlElement(ElementName = "REVENUELEDFOROPBAL")]
    public string ShowOpeningBal { get; set; }


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

    [XmlElement(ElementName = "GUID")]
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
public class CostCentEnvelope : TallyXmlJson
{

    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public CCentBody Body { get; set; } = new();
}

[XmlRoot(ElementName = "BODY")]
public class CCentBody
{
    [XmlElement(ElementName = "DESC")]
    public Description Desc { get; set; } = new();

    [XmlElement(ElementName = "DATA")]
    public CCentData Data { get; set; } = new();
}

[XmlRoot(ElementName = "DATA")]
public class CCentData
{
    [XmlElement(ElementName = "TALLYMESSAGE")]
    public CCentMessage Message { get; set; } = new();

    [XmlElement(ElementName = "COLLECTION")]
    public CostCentColl Collection { get; set; } = new CostCentColl();


}

[XmlRoot(ElementName = "COLLECTION")]
public class CostCentColl
{
    [XmlElement(ElementName = "COSTCENTRE")]
    public List<CostCenter> CostCenters { get; set; }
}

[XmlRoot(ElementName = "TALLYMESSAGE")]
public class CCentMessage
{
    [XmlElement(ElementName = "COSTCENTRE")]
    public CostCenter CostCenter { get; set; }
}
