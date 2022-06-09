using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;

public class TallyAmount : IXmlSerializable
{
    public TallyAmount(decimal amount)
    {
        Amount = amount;
    }

    public TallyAmount(decimal? forexAmount,
                       decimal? rateOfExchage,
                       string currency, decimal amount = 0)
    {
        ForexAmount = forexAmount;
        RateOfExchange = rateOfExchage;
        Currency = currency;
        Amount = amount;
    }

    public TallyAmount()
    {
    }

    public decimal Amount { get; set; }

    // public float? BaseAmount { get; set; }
    public decimal? ForexAmount { get; set; }

    public decimal? RateOfExchange { get; set; }

    public string? Currency { get; set; }

    public bool IsDebit => Amount < 0;

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        bool isEmptyElement = reader.IsEmptyElement;
        if (!isEmptyElement)
        {
            decimal t_opbal;
            string content = reader.ReadElementContentAsString();
            if (content != null)
            {
                if (content.ToString().Contains('='))
                {
                    List<string> SplittedValues = content.ToString().Split('=').ToList();
                    var CleanedAmount = Regex.Match(SplittedValues[1], @"[0-9.]+");
                    bool Isnegative = SplittedValues[1].Contains('-');

                    bool sucess = Isnegative ? decimal.TryParse('-' + CleanedAmount.Value, out t_opbal) : decimal.TryParse(CleanedAmount.ToString(), out t_opbal);
                    Amount = t_opbal;

                    var ForexInfo = SplittedValues[0].Split('@');
                    Currency = ForexInfo.Last().Split('/').Last().Trim();
                    //var BaseamountMatch = Regex.Match(SplittedValues[1].Trim(), @"[0-9.]+").Value;
                    var ForexAmountMatch = Regex.Match(ForexInfo[0].Trim(), @"[0-9.]+").Value;
                    var RateOfExchangeMatch = Regex.Match(ForexInfo[1].Trim(), @"[0-9.]+").Value;
                    //bool Baseamountsucess = float.TryParse(BaseamountMatch, out var Bamount);
                    bool forexsucess = decimal.TryParse(ForexAmountMatch, out var FAmout);
                    bool rateofexcahangesucess = decimal.TryParse(RateOfExchangeMatch, out var ROE);
                    ForexAmount = forexsucess ? FAmout : null;
                    RateOfExchange = rateofexcahangesucess ? ROE : null;

                    if (Isnegative)
                    {
                        ForexAmount *= -1;
                    }
                    //BaseAmount = Baseamountsucess ? Bamount : null;

                }
                else
                {

                    Amount = decimal.TryParse(content.Trim(), out t_opbal) ? t_opbal : 0;

                }
            }
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteString(this.ToString());
    }

    public override string ToString()
    {
        if (ForexAmount != null
            && ForexAmount != 0
            && RateOfExchange != null
            && RateOfExchange != 0
            && Currency != null
            && Currency != String.Empty)
        {
            if (IsDebit || ForexAmount < 0)
            {
                return $"-{Currency} {ForexAmount.ToString()!.Replace("-", "")} @ {RateOfExchange.ToString()!.Replace("-", "")}";
            }
            return $"{Currency} {ForexAmount} @ {RateOfExchange}";
        }
        return Amount.ToString();
    }


    public static implicit operator decimal(TallyAmount amount)
    {
        return amount.Amount;
    }

    public static implicit operator TallyAmount(decimal amount)
    {
        return new TallyAmount(amount);
    }
}
