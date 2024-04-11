using System.Diagnostics;

namespace TallyConnector.Core.Models.TallyComplexObjects;

[TDLCollection(ExplodeCondition = "NOT $$IsEmpty:{0}")]
[DebuggerDisplay("{ToString(),nq}")]
public class TallyRateField : ITallyComplexObject, IBaseObject
{
    [TDLField(TallyType = "Rate : Price")]
    [XmlElement(ElementName = "BASERATE")]
    [Column(TypeName = "decimal(20,4)")]
    public decimal Rate { get; set; }

    [TDLField(TallyType = "Number", Set = "$$String:{0}:\"Forex\"", Invisible = $"$$Value=#TC_{nameof(TallyRateField)}_{nameof(Rate)}")]
    [Column(TypeName = "decimal(20,4)")]
    [XmlElement(ElementName = "FOREXRATE")]
    public decimal? ForexRate { get; set; }

    [TDLField(TallyType = "Rate : Unit Symbol")]
    [XmlElement(ElementName = "UNIT")]
    public string Unit { get; set; }

    public string ToString(TallyAmountField amount)
    {
        if (ForexRate != null && amount.RateOfExchange != null)
        {
            return $"{amount.ForexCurrency}{ForexRate} = {amount.Currency} {Rate}/{Unit}";
        }
        return $"{Rate}/{Unit}";
    }
    public override string ToString()
    {
        if (ForexRate != null)
        {
            return $"{ForexRate} = {Rate}/{Unit}";
        }
        return $"{Rate}/{Unit}";
    }
}
