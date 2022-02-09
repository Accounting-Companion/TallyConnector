namespace TallyConnector.Models.Masters.Payroll;

[XmlRoot(ElementName = "ATTENDANCETYPE")]
public class AttendanceType : TallyXmlJson,ITallyObject
{
    public AttendanceType()
    {
        LanguageNameList = new();
    }

    [XmlAttribute(AttributeName = "NAME")]
    [JsonIgnore]
    public string OldName { get; set; }
    [XmlElement(ElementName = "MASTERID")]
    public int? TallyId { get; set; }
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

    [XmlElement(ElementName = "ATTENDANCEPRODUCTIONTYPE")]
    public string ProductionType { get; set; }

    [XmlElement(ElementName = "ATTENDANCEPERIOD")]
    public string Period { get; set; }

    [XmlElement(ElementName = "PARENT")]
    public string Parent { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    public string BaseUnit { get; set; }


    [XmlIgnore]
    public string Alias { get; set; }

    [XmlElement(ElementName = "CANDELETE")]
    public string CanDelete { get; set; } //Ignore This While Creating or Altering

    [XmlElement(ElementName = "GUID")]
    [Column(TypeName = "nvarchar(100)")]
    public string GUID { get; set; }

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
public class AttendanceTypeEnvelope : TallyXmlJson
{

    [XmlElement(ElementName = "HEADER")]
    public Header Header { get; set; }

    [XmlElement(ElementName = "BODY")]
    public AttendanceBody Body { get; set; } = new();
}

[XmlRoot(ElementName = "BODY")]
public class AttendanceBody
{
    [XmlElement(ElementName = "DESC")]
    public Description Desc { get; set; } = new();

    [XmlElement(ElementName = "DATA")]
    public AttendanceData Data { get; set; } = new();
}

[XmlRoot(ElementName = "DATA")]
public class AttendanceData
{
    [XmlElement(ElementName = "TALLYMESSAGE")]
    public AttendanceMessage Message { get; set; } = new();


    [XmlElement(ElementName = "COLLECTION")]
    public AttendanceTypeColl Collection { get; set; } = new AttendanceTypeColl();


}

[XmlRoot(ElementName = "COLLECTION")]
public class AttendanceTypeColl
{
    [XmlElement(ElementName = "ATTENDANCETYPE")]
    public List<AttendanceType> AttendanceTypes { get; set; }
}


[XmlRoot(ElementName = "TALLYMESSAGE")]
public class AttendanceMessage
{
    [XmlElement(ElementName = "ATTENDANCETYPE")]
    public AttendanceType AttendanceType { get; set; }
}

