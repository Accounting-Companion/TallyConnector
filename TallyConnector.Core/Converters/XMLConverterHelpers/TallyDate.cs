using System.Globalization;
using System.Xml.Schema;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;
[JsonConverter(typeof(TallyDateJsonConverter))]
public class TallyDate : IXmlSerializable
{
    private DateTime? Date;

    public TallyDate(DateTime date)
    {
        Date = date;
    }

    public TallyDate()
    {
    }

    public static implicit operator TallyDate(DateTime date)
    {
        return new(date);
    }
    public static implicit operator DateTime?(TallyDate tallyDate)
    {
        return tallyDate.Date;
    }

    public static implicit operator TallyDate?(string v)
    {
        bool IsSucess = DateTime.TryParseExact(v, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
        if (IsSucess)
        {
            return date;
        }
        else return null;

    }


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
            if (content != null)
            {
                bool v = DateTime.TryParseExact(content, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                if (v)
                {
                    Date = date;
                }
            }

        }
    }

    public void WriteXml(XmlWriter writer)
    {
        if (this.Date != null)
        {
            writer.WriteString(this.Date?.ToString("yyyyMMdd"));
        }
    }

    public override string? ToString()
    {
        return Date?.ToString("dd-MM-yyyy");
    }
    public string? ToString(string? format)
    {
        return Date?.ToString(format);
    }
}
