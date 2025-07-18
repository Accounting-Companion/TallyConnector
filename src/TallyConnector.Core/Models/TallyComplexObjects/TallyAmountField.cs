using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using TallyConnector.Abstractions.Attributes;
namespace TallyConnector.Core.Models.TallyComplexObjects;


[DebuggerDisplay("{ToString(),nq}")]
[GenerateMeta]
public partial class TallyAmountField : ITallyComplexObject, IBaseObject
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
    public string? Currency { get; set; }

    [TDLField(TallyType = "Number", Set = "$$ForexValue:{0}", Invisible = $"$$Value=#AMOUNT_PWS0")]
    [Column(TypeName = "decimal(20,4)")]
    [XmlElement(ElementName = "FOREXAMOUNT")]
    public decimal? ForexAmount { get; set; }

    [TDLField(TallyType = "Amount : Rate", Invisible = $"#ForexAmount_MT3L=#AMOUNT_PWS0", Format = "Forex,Currency")]
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
            return $"{ForexCurrency}{ForexAmount?.ToString(CultureInfo.InvariantCulture)} @ {RateOfExchange?.ToString(CultureInfo.InvariantCulture)}/{ForexCurrency} = {Currency} {Amount.ToString(CultureInfo.InvariantCulture)}";
        }
        if (IsDebit)
        {
            return (Amount * -1).ToString(CultureInfo.InvariantCulture);
        }
        return Amount.ToString(CultureInfo.InvariantCulture);
    }
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
    {
        if (ForexAmount != null)
        {
            return $"{ForexCurrency}{ForexAmount?.ToString(format,CultureInfo.InvariantCulture)} @ {RateOfExchange?.ToString(format, CultureInfo.InvariantCulture)}/{ForexCurrency} = {Currency} {Amount.ToString(format,CultureInfo.InvariantCulture)}";
        }
        if (IsDebit)
        {
            return (Amount * -1).ToString(format, CultureInfo.InvariantCulture);
        }
        return Amount.ToString(format,CultureInfo.InvariantCulture);
    }
 
}
