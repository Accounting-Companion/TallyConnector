using System.Diagnostics;
using System.Globalization;
using System.Xml.Schema;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;
[DebuggerDisplay("{ToString()}")]
[JsonConverter(typeof(TallyDateJsonConverter))]
public class TallyDate : IXmlSerializable
{
    private protected DateTime Date;

    public TallyDate(DateTime date)
    {
        Date = date;
    }

    public TallyDate()
    {
    }

    public static implicit operator TallyDate?(DateTime? date)
    {
        if (date != null)
        {
            return new((DateTime)date);
        }
        return null;
    }
    public static implicit operator TallyDate(DateTime date)
    {
        return new(date);
    }
    public static implicit operator DateTime?(TallyDate? tallyDate)
    {
        return tallyDate?.Date;
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
            writer.WriteAttributeString("TYPE", "Date");
            writer.WriteString(ToString());
        }
    }

    public override string? ToString()
    {
        return Date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
    }
    
}
public class TallyDMYYYYDate : TallyDate, IXmlSerializable
{
    public new void ReadXml(XmlReader reader)
    {
        bool isEmptyElement = reader.IsEmptyElement;
        if (!isEmptyElement)
        {
            string content = reader.ReadElementContentAsString();
            if (content != null)
            {
                bool v = DateTime.TryParseExact(content, "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                if (v)
                {
                    Date = date;
                }
            }

        }
    }
}
