using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;
[DebuggerDisplay("{ToString()}")]
[JsonConverter(typeof(TallyRateJsonConverter))]
public class TallyRate : IXmlSerializable
{
    public TallyRate()
    {
        Unit = string.Empty;
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

    [Column(TypeName = "decimal(20,6)")]
    public decimal RatePerUnit { get; private set; }
    public string Unit { get; private set; }

    [Column(TypeName = "decimal(20,6)")]
    public decimal? ForexAmount { get; set; }

    [Column(TypeName = "decimal(20,6)")]
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
                var matches = Regex.Matches(content, @"\b[0-9.]+\b");
                if (matches.Count == 2)
                {
                    ForexAmount = decimal.Parse(matches[0].Value, CultureInfo.InvariantCulture);
                    RatePerUnit = decimal.Parse(matches[1].Value, CultureInfo.InvariantCulture);
                    ForeignCurrency = content[0].ToString();
                    Unit = content.Split('/').Last();
                }
                else
                {
                    var splittedtext = content.Split('/');
                    RatePerUnit = decimal.Parse(matches[0].Value, CultureInfo.InvariantCulture);
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
            return $"{ForeignCurrency?.ToString(CultureInfo.InvariantCulture)} {ForexAmount?.ToString(CultureInfo.InvariantCulture)} = {RatePerUnit.ToString(CultureInfo.InvariantCulture)}/{Unit?.ToString(CultureInfo.InvariantCulture)}";
        }
        else
        {
            return $"{RatePerUnit.ToString(CultureInfo.InvariantCulture)}/{Unit?.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
