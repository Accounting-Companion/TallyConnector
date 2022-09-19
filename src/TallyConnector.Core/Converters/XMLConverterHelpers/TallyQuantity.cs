using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using TallyConnector.Core.Models.Masters.Inventory;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;
[DebuggerDisplay("{ToString()}")]
[JsonConverter(typeof(TallyQuantityJsonConverter))]
public class TallyQuantity : IXmlSerializable
{
    public TallyQuantity()
    {
    }
    //public TallyQuantity(StockItem stockItem, decimal number)
    //{
    //    Number = number;
    //    if (stockItem.BaseUnit != null && stockItem.BaseUnit != string.Empty)
    //    {
    //        PrimaryUnits = new(number, stockItem.BaseUnit);
    //    }

    //}
    public TallyQuantity(StockItem stockItem, decimal quantity)
    {
        Number = quantity;
        if (stockItem.BaseUnit != null && stockItem.BaseUnit != string.Empty)
        {
            PrimaryUnits = new(quantity, stockItem.BaseUnit);
        }
        if (stockItem.AdditionalUnits != null && stockItem.AdditionalUnits != string.Empty)
        {
            if (stockItem.Conversion != null && stockItem.Denominator != null)
            {
                SecondaryUnits = new(quantity * Math.Round(((decimal)stockItem.Conversion / (decimal)stockItem.Denominator), 2), stockItem.AdditionalUnits);
            }
        }

    }
    public TallyQuantity(decimal quantity, string unit)
    {
        Number = quantity;
        PrimaryUnits = new(quantity, unit);
    }
    public TallyQuantity(decimal quantity,
                         string unit,
                         decimal secondaryQuantity,
                         string secondaryUnit)
    {
        Number = quantity;
        PrimaryUnits = new(quantity, unit);
        SecondaryUnits = new(secondaryQuantity, secondaryUnit);
    }

    [Column(TypeName = "decimal(9,4)")]
    public decimal Number { get; private set; }
    public BaseTallyQuantity? PrimaryUnits { get; private set; }
    public BaseTallyQuantity? SecondaryUnits { get; private set; }
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
                content = content.Trim();
                var matches = Regex.Matches(content, @"[0-9.]+");
                if (matches.Count == 2)
                {
                    Number = decimal.Parse(matches[0].Value);
                    var splittedtext = content.Split('=');

                    PrimaryUnits = new(Number, splittedtext.First().Trim().Split(' ').Last().Trim());
                    SecondaryUnits = new(decimal.Parse(matches[1].Value), splittedtext.Last().Trim().Split(' ').Last().Trim());
                }
                else
                {
                    var splittedtext = content.Split(' ');
                    Number = decimal.Parse(matches[0].Value);
                    PrimaryUnits = new(Number, splittedtext.Last().Trim());
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
        if (SecondaryUnits != null)
        {
            return $"{PrimaryUnits?.Number} {PrimaryUnits?.Unit} = {SecondaryUnits?.Number} {SecondaryUnits?.Unit}";
        }
        else
        {
            return $"{PrimaryUnits?.Number} {PrimaryUnits?.Unit}";
        }
    }
}

public class BaseTallyQuantity
{
    [Column(TypeName = "decimal(9,4)")]
    public decimal Number { get; private set; }

    public string Unit { get; private set; }

    public BaseTallyQuantity(decimal number, string unit)
    {
        Number = number;
        Unit = unit;
    }
}

