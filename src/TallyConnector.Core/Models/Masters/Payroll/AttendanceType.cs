namespace TallyConnector.Core.Models.Masters.Payroll;



[XmlRoot(ElementName = "ATTENDANCETYPE")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.AttendanceTypes)]
public class AttendanceType : BasicTallyObject, IAliasTallyObject
{
    public AttendanceType()
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

    [XmlElement(ElementName = "ATTENDANCEPRODUCTIONTYPE")]
    public string? ProductionType { get; set; }

    [XmlElement(ElementName = "ATTENDANCEPERIOD")]
    public string? Period { get; set; }

    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "PARENTID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? ParentId { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    public string? BaseUnit { get; set; }


    [XmlIgnore]
    public string? Alias { get; set; }

    [XmlElement(ElementName = "CANDELETE")]
    public string? CanDelete { get; set; } //Ignore This While Creating or Altering


    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    public void CreateNamesList()
    {
        if (LanguageNameList.Count == 0)
        {
            LanguageNameList.Add(new LanguageNameList());
            LanguageNameList[0].NameList.NAMES?.Add(Name);

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
        CreateNamesList();
    }

    public override string ToString()
    {
        return $"{Name}";
    }
}