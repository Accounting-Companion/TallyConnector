namespace TallyConnector.Core.Models.TallyComplexObjects;
[TDLCollection(ExplodeCondition = "NOT $$IsEmpty:{0}")]
public class Quantity : ITallyComplexObject, ITallyBaseObject
{
    [TDLField(TallyType = "Quantity : UnitSymbol")]
    [XmlElement(ElementName = "UNIT")]
    public string Unit { get; set; }

    [TDLField(TallyType = "Number", Format = "TailUnits")]
    [XmlElement(ElementName = "QUANTITYNUM")]
    public decimal QuantityNum { get; set; }


    public override string ToString()
    {
        return $" {QuantityNum} {Unit}";
    }
}
