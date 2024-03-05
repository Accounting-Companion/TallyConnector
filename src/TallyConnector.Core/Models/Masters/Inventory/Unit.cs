namespace TallyConnector.Core.Models.Masters.Inventory;


[XmlRoot(ElementName = "UNIT")]
[XmlType(AnonymousType = true)]
[TallyObjectType(TallyObjectType.Units)]
public class Unit : BaseMasterObject
{
    [XmlElement(ElementName = "OLDNAME")]
    [TDLField(Set = "$Name")]
    [JsonIgnore]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string OldName { get; set; }

    [XmlElement(ElementName = "ORIGINALNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? OriginalName { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
   
    public string? BaseUnit { get; set; }

    [XmlElement(ElementName = "BASEUNITID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:Unit:$BaseUnits")]
    public string? BaseUnitId { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITS")]
    public string? AdditionalUnits { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    [TDLField(Set = "$GUID:Unit:$AdditionalUnits")]
    public string? AdditionalUnitId { get; set; }


    [XmlElement(ElementName = "GSTREPUOM")]
    public string? UQC { get; set; }

    [XmlElement(ElementName = "DECIMALPLACES")]
    public int DecimalPlaces { get; set; }

    [XmlElement(ElementName = "CANDELETE")]
    [TDLField(IncludeInFetch = true)]
    public bool? CanDelete { get; set; }

    private bool? _isSimpleUnit;

    [XmlElement(ElementName = "ISSIMPLEUNIT")]
    public bool IsSimpleUnit
    {
        get
        {
            _isSimpleUnit = IssimpleUnit();
            return _isSimpleUnit ?? true;
        }
        set { _isSimpleUnit = value; }
    }

    [XmlElement(ElementName = "ISGSTEXCLUDED")]
    public bool? IsGstExcluded { get; set; }

    [XmlElement(ElementName = "CONVERSION")]
    public double Conversion { get; set; }
    public bool IssimpleUnit()
    {
        if (AdditionalUnits is null || BaseUnit is null || AdditionalUnits == string.Empty || BaseUnit == string.Empty)
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return $"Unit - {Name}";
    }

}