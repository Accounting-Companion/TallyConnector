using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Core.Models.TallyComplexObjects;

[TDLCollection(ExplodeCondition = "NOT $$IsEmpty:{0}")]
[DebuggerDisplay("{ToString(),nq}")]
public class TallyRateField : ITallyComplexObject, ITallyBaseObject
{
    [TDLField(TallyType = "Rate : Price")]
    [XmlElement(ElementName = "BASERATE")]
    public decimal BaseRate { get; set; }

    [TDLField(TallyType = "Number", Set = "$$String:{0}:\"Forex\"", Invisible = $"$$Value=#TC_{nameof(TallyRateField)}_{nameof(BaseRate)}")]
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
            return $"{amount.ForexCurrency}{ForexRate} = {amount.Currency} {BaseRate}/{Unit}";
        }
        return $"{BaseRate}/{Unit}";
    }
    public override string ToString()
    {
        if (ForexRate != null)
        {
            return $"{ForexRate} = {BaseRate}/{Unit}";
        }
        return $"{BaseRate}/{Unit}";
    }
}
