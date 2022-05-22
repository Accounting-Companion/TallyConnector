namespace TallyConnector.Core.Models.Masters.Inventory;


[XmlRoot(ElementName = "UNIT")]
[XmlType(AnonymousType = true)]
public class Unit : BasicTallyObject, ITallyObject
{
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


    [XmlElement(ElementName = "ORIGINALNAME")]
    public string? OriginalName { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    public string? BaseUnit { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITS")]
    public string? AdditionalUnits { get; set; }


    [XmlElement(ElementName = "GSTREPUOM")]
    public string? UQC { get; set; }

    [XmlElement(ElementName = "DECIMALPLACES")]
    public int DecimalPlaces { get; set; }

    [XmlElement(ElementName = "CANDELETE")]
    public string? CanDelete { get; set; }

    private string? _IsSimpleUnit;

    [XmlElement(ElementName = "ISSIMPLEUNIT")]
    public string IsSimpleUnit
    {
        get
        {
            _IsSimpleUnit = IssimpleUnit();
            return _IsSimpleUnit;
        }
        set { _IsSimpleUnit = value; }
    }

    [XmlElement(ElementName = "ISGSTEXCLUDED")]
    public string? IsGstExcluded { get; set; }

    [XmlElement(ElementName = "CONVERSION")]
    public double Conversion { get; set; }
    public string IssimpleUnit()
    {
        if (AdditionalUnits is null || BaseUnit is null || AdditionalUnits == string.Empty || BaseUnit == string.Empty)
        {
            return "YES";
        }
        return "NO";
    }

    public new void PrepareForExport()
    {
    }

    public override string ToString()
    {
        return $"Unit - {Name}";
    }

}