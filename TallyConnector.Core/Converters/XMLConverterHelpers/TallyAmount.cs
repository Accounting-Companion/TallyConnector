using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;

public class TallyAmount : IXmlSerializable
{
    public TallyAmount(decimal amount, bool? isDebit = null)
    {
        Amount = amount;
        if (isDebit is null)
        {
            isDebit = Amount < 0;
        }
        IsDebit = (bool)isDebit;
        Amount = Amount < 0 ? Amount * -1 : Amount;
    }

    public TallyAmount(decimal? forexAmount,
                       decimal? rateOfExchage,
                       string currency, bool? isDebit = null, decimal amount = 0)
    {
        ForexAmount = forexAmount;
        RateOfExchange = rateOfExchage;
        Currency = currency;
        Amount = amount;
        if (isDebit is null)
        {
            isDebit = ForexAmount < 0;
        }
        IsDebit = (bool)isDebit;
        ForexAmount = ForexAmount < 0 ? ForexAmount * -1 : ForexAmount;
        Amount = Amount < 0 ? Amount * -1 : Amount;
    }

    public TallyAmount()
    {
    }

    public decimal Amount { get; set; }

    // public float? BaseAmount { get; set; }
    public decimal? ForexAmount { get; set; }

    public decimal? RateOfExchange { get; set; }

    public string? Currency { get; set; }

    public bool IsDebit { get; set; }

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        bool isEmptyElement = reader.IsEmptyElement;
        if (!isEmptyElement)
        {
            string content = reader.ReadElementContentAsString();

            if (content != null && content != string.Empty)
            {
                if (content[0] == '-')
                {
                    IsDebit = true;
                }
                var matches = Regex.Matches(content, @"[0-9.]+");
                if (matches.Count == 3)
                {
                    ForexAmount = decimal.Parse(matches[0].Value);
                    RateOfExchange = decimal.Parse(matches[1].Value);
                    Amount = decimal.Parse(matches[2].Value);
                    Currency = IsDebit ? content[1].ToString() : content[0].ToString();
                }
                else if (matches.Count == 1)
                {
                    Amount = decimal.Parse(matches[0].Value);
                }
                //if (content.ToString().Contains('='))
                //{
                //    List<string> SplittedValues = content.ToString().Split('=').ToList();
                //    var CleanedAmount = Regex.Match(SplittedValues[1], @"[0-9.]+");
                //    bool Isnegative = SplittedValues[1].Contains('-');

                //    bool sucess = Isnegative ? decimal.TryParse('-' + CleanedAmount.Value, out t_opbal) : decimal.TryParse(CleanedAmount.ToString(), out t_opbal);
                //    Amount = t_opbal;

                //    var ForexInfo = SplittedValues[0].Split('@');
                //    Currency = ForexInfo.Last().Split('/').Last().Trim();
                //    //var BaseamountMatch = Regex.Match(SplittedValues[1].Trim(), @"[0-9.]+").Value;
                //    var ForexAmountMatch = Regex.Match(ForexInfo[0].Trim(), @"[0-9.]+").Value;
                //    var RateOfExchangeMatch = Regex.Match(ForexInfo[1].Trim(), @"[0-9.]+").Value;
                //    //bool Baseamountsucess = float.TryParse(BaseamountMatch, out var Bamount);
                //    bool forexsucess = decimal.TryParse(ForexAmountMatch, out var FAmout);
                //    bool rateofexcahangesucess = decimal.TryParse(RateOfExchangeMatch, out var ROE);
                //    ForexAmount = forexsucess ? FAmout : null;
                //    RateOfExchange = rateofexcahangesucess ? ROE : null;

                //    if (Isnegative)
                //    {
                //        ForexAmount *= -1;
                //    }
                //    //BaseAmount = Baseamountsucess ? Bamount : null;

                //}
                //else
                //{

                //    Amount = decimal.TryParse(content.Trim(), out t_opbal) ? t_opbal : 0;

                //}
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
        if (IsDebit)
        {
            return (Amount * -1).ToString();
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
