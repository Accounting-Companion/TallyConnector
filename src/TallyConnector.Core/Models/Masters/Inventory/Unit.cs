namespace TallyConnector.Core.Models.Masters.Inventory;


[XmlRoot(ElementName = "UNIT")]
[XmlType(AnonymousType = true)]
public class Unit : BasicTallyObject, INamedTallyObject
{
    [XmlAttribute(AttributeName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    [JsonIgnore]
    public string? OldName { get; set; }

    private string? name;

    [XmlElement(ElementName = "NAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
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


    [XmlElement(ElementName = "ORIGINALNAME")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? OriginalName { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    [Column(TypeName = $"nvarchar({Constants.MaxNameLength})")]
    public string? BaseUnit { get; set; }

    [XmlElement(ElementName = "BASEUNITID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? BaseUnitId { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITS")]
    public string? AdditionalUnits { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? AdditionalUnitId { get; set; }


    [XmlElement(ElementName = "GSTREPUOM")]
    public string? UQC { get; set; }

    [XmlElement(ElementName = "DECIMALPLACES")]
    public int DecimalPlaces { get; set; }

    [XmlElement(ElementName = "CANDELETE")]
    public TallyYesNo? CanDelete { get; set; }

    private bool? _IsSimpleUnit;

    [XmlElement(ElementName = "ISSIMPLEUNIT")]
    public TallyYesNo IsSimpleUnit
    {
        get
        {
            _IsSimpleUnit = IssimpleUnit();
            return _IsSimpleUnit;
        }
        set { _IsSimpleUnit = value; }
    }

    [XmlElement(ElementName = "ISGSTEXCLUDED")]
    public TallyYesNo? IsGstExcluded { get; set; }

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

    public new void PrepareForExport()
    {
    }

    public override string ToString()
    {
        return $"Unit - {Name}";
    }

}