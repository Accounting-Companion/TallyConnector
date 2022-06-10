using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;
public class TallyRate : IXmlSerializable
{
    public TallyRate()
    {
    }
    public TallyRate(decimal ratePerUnit, string unit)
    {
        RatePerUnit = ratePerUnit;
        Unit = unit;
    }
    public TallyRate(string unit,
                     decimal forexAmount,
                     decimal rateOfExchange,
                     string currency)
    {
        RatePerUnit = forexAmount * rateOfExchange;
        Unit = unit;
        ForexAmount = forexAmount;
        RateOfExchange = rateOfExchange;
        ForeignCurrency = currency;
    }

    public decimal RatePerUnit { get; private set; }
    public string Unit { get; private set; }

    public decimal? ForexAmount { get; set; }

    public decimal? RateOfExchange { get; set; }

    public string? ForeignCurrency { get; set; }

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
                var matches = Regex.Matches(content, @"[0-9.]+");
                if (matches.Count == 2)
                {
                    ForexAmount = decimal.Parse(matches[0].Value);
                    RatePerUnit = decimal.Parse(matches[1].Value);
                    ForeignCurrency = content[0].ToString();
                    Unit = content.Split('/').Last();
                }
                else
                {
                    var splittedtext = content.Split('/');
                    RatePerUnit = decimal.Parse(matches[0].Value);
                    Unit = splittedtext.Last().Trim();
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
        if (ForexAmount != null && ForexAmount != 0)
        {
            return $"{ForeignCurrency} {ForexAmount} = {RatePerUnit}/{Unit}";
        }
        else
        {
            return $"{RatePerUnit}/{Unit}";
        }
    }
}
