using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;
[DebuggerDisplay("{ToString()}")]
[JsonConverter(typeof(TallyAmountJsonConverter))]
public class TallyAmount : IXmlSerializable
{


    public TallyAmount(decimal amount, bool? isDebit = null)
    {
        Amount = amount;
        if (isDebit is null)
        {
            isDebit = Amount < 0;
        }
        else
        {
            PreserveAmount = true;
        }
        IsDebit = (bool)isDebit;
        if (!PreserveAmount)
        {
            Amount = Amount < 0 ? Amount * -1 : Amount;
        }
    }

    public TallyAmount(decimal? forexAmount,
                       decimal? rateOfExchage,
                       string currency,decimal amount = 0, bool? isDebit = null )
    {
        ForexAmount = forexAmount;
        RateOfExchange = rateOfExchage;
        Currency = currency;
        Amount = amount;
        if (isDebit is null)
        {
            isDebit = ForexAmount < 0;
        }
        else
        {
            PreserveAmount = true;
        }
        IsDebit = (bool)isDebit;
        ForexAmount = ForexAmount < 0 ? ForexAmount * -1 : ForexAmount;
        if (!PreserveAmount)
        {
            Amount = Amount < 0 ? Amount * -1 : Amount;
        }
    }

    public TallyAmount()
    {
    }

    [Column(TypeName = "decimal(20,6)")]
    public decimal Amount { get; private set; }

    // public float? Amount { get; set; }
    [Column(TypeName = "decimal(20,6)")]
    public decimal? ForexAmount { get; private set; }

    [Column(TypeName = "decimal(20,6)")]
    public decimal? RateOfExchange { get; private set; }

    public string? Currency { get; private set; }

    public bool IsDebit { get; private set; }

    public bool PreserveAmount { get; private set; }

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
                    ForexAmount = decimal.Parse(matches[0].Value, CultureInfo.InvariantCulture);
                    RateOfExchange = decimal.Parse(matches[1].Value, CultureInfo.InvariantCulture);
                    Amount = decimal.Parse(matches[2].Value, CultureInfo.InvariantCulture);
                    Currency = IsDebit ? content[1].ToString() : content[0].ToString();
                }
                else if (matches.Count == 1)
                {
                    Amount = decimal.Parse(matches[0].Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    if (content.Contains('=') && matches.Count == 2)

                    {
                        Amount = decimal.Parse(matches[1].Value, CultureInfo.InvariantCulture);
                        Currency = IsDebit ? content[1].ToString() : content[0].ToString();
                    }
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
            if (!PreserveAmount && IsDebit || ForexAmount < 0)
            {
                return $"-{Currency} {ForexAmount?.ToString(CultureInfo.InvariantCulture)!.Replace("-", "")} @ {RateOfExchange?.ToString(CultureInfo.InvariantCulture)!.Replace("-", "")}";
            }
            return $"{Currency} {ForexAmount?.ToString(CultureInfo.InvariantCulture)} @ {RateOfExchange?.ToString(CultureInfo.InvariantCulture)}";
        }

        if (!PreserveAmount && IsDebit)
        {

            return (Amount * -1).ToString(CultureInfo.InvariantCulture);
        }
        return Amount.ToString(CultureInfo.InvariantCulture);
    }


    public static implicit operator decimal(TallyAmount amount)
    {
        if (amount.IsDebit)
        {
            return amount.Amount * -1;
        }
        else
        {

            return amount.Amount;
        }
    }

    public static implicit operator TallyAmount(decimal amount)
    {
        return new TallyAmount(amount);
    }
}
