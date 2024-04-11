namespace TallyConnector.Core.Models.Masters.CostCenter;


[XmlRoot(ElementName = "COSTCENTRE")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.CostCentres)]
[TDLDefaultFiltersMethodName(FunctionName=nameof(GetDefaultFilters))]
public class CostCentre : BaseMasterObject
{
    public CostCentre()
    {
        //LanguageNameList = new();
        //Category = string.Empty;
    }

    [XmlElement(ElementName = "OLDNAME")]
    [TDLField(Set = "$Name")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string OldName { get; set; }

    [XmlElement(ElementName = "CATEGORY")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [Required]
    public string Category { get; set; }



    [XmlElement(ElementName = "PARENT")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? Parent { get; set; }


    [XmlElement(ElementName = "EMAILID")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? EmailId { get; set; }

    [XmlElement(ElementName = "REVENUELEDFOROPBAL")]
    [Column(TypeName = "nvarchar(3)")]
    public bool? ShowOpeningBal { get; set; }


    [XmlIgnore]
    public string? Alias { get; set; }

    [JsonIgnore]
    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameList> LanguageNameList { get; set; }

    public static Filter[] GetDefaultFilters()
    {
        return [
            new Filter("IsEmployeeGroup", "Not $ISEMPLOYEEGROUP"),
            new Filter("IsPayroll", "Not $FORPAYROLL")
            ];

    }
    public override string ToString()
    {
        return $"Cost Center - {Name}";
    }
}
