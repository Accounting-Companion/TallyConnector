using System.Diagnostics;
using System.Globalization;
namespace TallyConnector.Core.Models.TallyComplexObjects;

[TDLCollection(ExplodeCondition = "NOT $$IsEmpty:{0}")]
[DebuggerDisplay("{ToString(),nq}")]
public class TallyAmountField : ITallyComplexObject, IBaseObject
{
    public TallyAmountField()
    {
        Amount = 0;
        IsDebit = false;
    }
    public TallyAmountField(decimal amount, bool isDebit)
    {
        Amount = amount;
        IsDebit = isDebit;
    }

    [TDLField(TallyType = "Number", Set = "$$BaseValue:{0}")]
    [Column(TypeName = "decimal(20,4)")]
    [XmlElement(ElementName = "AMOUNT")]
    public decimal Amount { get; set; }

    [TDLField(Set = "$CurrencyName:Company:##SVCurrentCompany")]
    [XmlElement(ElementName = "CURRENCY")]
    public string Currency { get; set; }

    [TDLField(TallyType = "Number", Set = "$$ForexValue:{0}", Invisible = $"$$Value=#TC_{nameof(TallyAmountField)}_{nameof(Amount)}")]
    [Column(TypeName = "decimal(20,4)")]
    [XmlElement(ElementName = "FOREXAMOUNT")]
    public decimal? ForexAmount { get; set; }

    [TDLField(TallyType = "Amount : Rate", Invisible = $"#TC_{nameof(TallyAmountField)}_{nameof(ForexAmount)}=#TC_{nameof(TallyAmountField)}_{nameof(Amount)}", Format = "Forex,Currency")]
    [XmlElement(ElementName = "FOREXSYMBOL")]
    public string? ForexCurrency { get; set; }

    [TDLField(TallyType = "Number", Set = "$$RatexValue:{0}", Invisible = "$$Value=1")]
    [XmlElement(ElementName = "RATEOFEXCHANGE")]
    [Column(TypeName = "decimal(20,4)")]
    public decimal? RateOfExchange { get; set; }

    [TDLField(Set = $"$$IsDebit:{{0}}")]
    [XmlElement(ElementName = "ISDEBIT")]
    public bool IsDebit { get; set; }



    public override string ToString()
    {
        if (ForexAmount != null)
        {
            return $"{ForexCurrency}{ForexAmount?.ToString(CultureInfo.InvariantCulture)} @ {RateOfExchange?.ToString(CultureInfo.InvariantCulture)}/{ForexCurrency} = {Currency} {Amount}";
        }
        if (IsDebit)
        {
            return (Amount * -1).ToString(CultureInfo.InvariantCulture);
        }
        return Amount.ToString(CultureInfo.InvariantCulture);
    }
}
