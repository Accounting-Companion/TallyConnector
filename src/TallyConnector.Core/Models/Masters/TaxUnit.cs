namespace TallyConnector.Core.Models.Masters;
[XmlRoot("TAXUNIT")]
[TallyObjectType(TallyObjectType.TaxUnits)]
public class TaxUnit : BasicTallyObject, IAliasTallyObject
{
    private string? name;


    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? OldName { get; set; }
    /// <summary>
    /// Name of Ledger
    /// </summary>
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
    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    [XmlElement(ElementName = "GSTREGNUMBER")]
    public string? GSTIN { get; set; }
    public void CreateNamesList()
    {
        if (LanguageNameList.Count == 0)
        {
            LanguageNameList.Add(new LanguageNameList());
            LanguageNameList?[0]?.NameList?.NAMES?.Add(Name);

        }
        if (Alias != null && Alias != string.Empty)
        {
            LanguageNameList![0].LanguageAlias = Alias;
        }
    }

    public new string GetXML(XmlAttributeOverrides? attrOverrides = null, bool indent = false)
    {
        CreateNamesList();
        return base.GetXML(attrOverrides, indent);
    }
    public override string ToString()
    {
        return $"TaxUnit - {Name}";
    }
}
