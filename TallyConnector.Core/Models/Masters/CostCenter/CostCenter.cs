namespace TallyConnector.Core.Models.Masters.CostCenter;


[XmlRoot(ElementName = "COSTCENTRE")]
[XmlType(AnonymousType = true)]
public class CostCenter : BasicTallyObject, ITallyObject
{
    public CostCenter()
    {
        LanguageNameList = new();
    }

    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    public string? OldName { get; set; }

    private string? name;

    [XmlElement(ElementName = "NAME")]
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
    [Required]
    public string Category { get; set; }

    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "EMAILID")]
    public string? Emailid { get; set; }

    [XmlElement(ElementName = "REVENUELEDFOROPBAL")]
    public string? ShowOpeningBal { get; set; }


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
