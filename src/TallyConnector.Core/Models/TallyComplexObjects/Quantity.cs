namespace TallyConnector.Core.Models.TallyComplexObjects;
[TDLCollection(ExplodeCondition = "NOT $$IsEmpty:{0}")]
public class Quantity : ITallyComplexObject, ITallyBaseObject
{
    [TDLField(TallyType = "Quantity : UnitSymbol")]
    public string Unit { get; set; }

    [TDLField(TallyType = "Number", Format = "TailUnits")]
    public decimal QuantityNum { get; set; }

}
