namespace TallyConnector.Core.Models.Masters.Payroll;



[XmlRoot(ElementName = "ATTENDANCETYPE")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.AttendanceTypes)]
public class AttendanceType : BaseMasterObject, IAliasTallyObject
{
    public AttendanceType()
    {
        LanguageNameList = new();
    }

    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    public string? OldName { get; set; }



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

    public override string ToString()
    {
        return $"{Name}";
    }
}