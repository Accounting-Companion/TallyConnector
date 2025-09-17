namespace TallyConnector.Models.Base.Masters.Inventory;
[XmlRoot(ElementName = "UNIT")]
[XmlType(AnonymousType = true)]
[TDLCollection(Type = "Unit")]
public partial class Unit : BaseMasterObject
{
    //[XmlElement(ElementName = "ORIGINALNAME")]
    //public new string Name { get; set; }

    [XmlElement(ElementName = "BASEUNITS")]
    public string? BaseUnit { get; set; }

    [XmlElement(ElementName = "ADDITIONALUNITS")]
    public string? AdditionalUnits { get; set; }

    [XmlElement(ElementName = "GSTREPUOM")]
    public string? UQC { get; set; }

    [XmlElement(ElementName = "DECIMALPLACES")]
    public int DecimalPlaces { get; set; }



    private bool _IsSimpleUnit;

    [XmlElement(ElementName = "ISSIMPLEUNIT")]
    public bool IsSimpleUnit
    {
        get
        {
            _IsSimpleUnit = IssimpleUnit();
            return _IsSimpleUnit;
        }
        set { _IsSimpleUnit = value; }
    }
    [XmlElement(ElementName = "ISGSTEXCLUDED")]
    public bool? IsGstExcluded { get; set; }

    [XmlElement(ElementName = "CONVERSION")]
    public double? Conversion { get; set; }



    [XmlElement(ElementName = "CANDELETE")]
    public bool? CanDelete { get; set; }

    public bool IssimpleUnit()
    {
        if (AdditionalUnits is null || BaseUnit is null || AdditionalUnits == string.Empty || BaseUnit == string.Empty)
        {
            return true;
        }
        return false;
    }
}
