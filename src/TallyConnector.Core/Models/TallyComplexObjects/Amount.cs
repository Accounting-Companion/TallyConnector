using static TallyConnector.Core.Models.Constants;
namespace TallyConnector.Core.Models.TallyComplexObjects;
[TDLFunctionsMethodName(FunctionName = nameof(GetTDLFunctions))]
[TDLCollection(ExplodeCondition = "NOT $$IsEmpty:$ClosingBalance")]
public class Amount : ITallyComplexObject, ITallyBaseObject
{
    public Amount()
    {
        BaseAmount = 0;
        IsDebit = false;
    }
    public Amount(decimal amount, bool isDebit)
    {
        BaseAmount = amount;
        IsDebit = isDebit;
    }

    [TDLField(TallyType = "Amount : Base", Set = "$$BaseValue:{0}")]
    [Column(TypeName = "decimal(20,4)")]
    public decimal BaseAmount { get; set; }
    [TDLField(TallyType = "Amount : Forex", Set = "$$ForexValue:{0}", Invisible = "$$Value=#TC_Amount_BaseAmount")]
    [Column(TypeName = "decimal(20,4)")]
    public decimal? ForexAmount { get; set; }
    [TDLField(TallyType = "Amount : Rate", Invisible = "#TC_Amount_ForexAmount=#TC_Amount_BaseAmount", Format = "Forex,Currency")]
    [Column(TypeName = "decimal(20,4)")]
    public string? ForexSymbol { get; set; }

    [TDLField(TallyType = "Number", Set = "$$RatexValue:{0}", Invisible = "$$Value=1")]
    public decimal? ExchangeRate { get; set; }

    [TDLField(Set = $"$$IsDebit:{{0}}")]
    public bool IsDebit { get; set; }


    public static List<TDLFunction> GetTDLFunctions()
    {
        return [];
    }
}
