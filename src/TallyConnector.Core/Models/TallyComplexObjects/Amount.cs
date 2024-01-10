using static TallyConnector.Core.Models.Constants;
namespace TallyConnector.Core.Models.TallyComplexObjects;
public class Amount : ITallyComplexObject,ITallyBaseObject
{
    [TDLField(TallyType = "Amount : Base", Format = "Symbol, No Zero")]
    public decimal BaseAmount { get; set; }
    [TDLField(TallyType = "Amount : Forex", Format = "Symbol, No Zero")]
    public decimal ForexAmount { get; set; }

    [TDLField(TallyType = "Amount : Rate", Format = "Symbol, No Zero")]
    public decimal ExchangeRate { get; set; }

    [TDLField(Set = $"$$IsDebit:{{0}}")]
    public bool IsDebit { get; set; }

}
