using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.Schema;
using XmlSourceGenerator.Abstractions;

namespace TallyConnector.Core.Models;
[DebuggerDisplay("{ToString()}")]
public class TallyDate : IXmlSerializable, IXmlStreamable
{
    private protected DateTime Date;

    public string DefaultXmlRootElementName => nameof(TallyDate);

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
#if NET6_0_OR_GREATER
    public static implicit operator TallyDate(DateOnly date)
    {
        return new(date.ToDateTime(TimeOnly.MinValue));
    }
#endif
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

        if (Date != null)
        {
            writer.WriteAttributeString("TYPE", "Date");
            writer.WriteString(ToString());
        }
    }

    public override string? ToString()
    {
        return Date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
    }

    public virtual void ReadFromXml(XElement element, XmlSerializationOptions options = null)
    {
        if (element == null) return;
        string content = element.Value;
        if (content != null)
        {
            if (DateTime.TryParseExact(content, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                Date = date;
                return;
            }
             //Fallback to dd-MM-yyyy if needed or stick to original ReadXml logic
             if (DateTime.TryParseExact(content, "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Date = date;
            }
        }
    }

    public virtual XElement WriteToXml(XmlSerializationOptions options = null)
    {
        // Element name will be replaced by the property's XML name mapping by the generator
        var element = new XElement("Date"); 
        if (Date != DateTime.MinValue)
        {
             element.Add(new XAttribute("TYPE", "Date"));
             element.Value = ToString() ?? "";
        }
        return element;
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

    public override void ReadFromXml(XElement element, XmlSerializationOptions options = null)
    {
        if (element == null) return;
        string content = element.Value;
        if (content != null)
        {
            if (DateTime.TryParseExact(content, "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                Date = date;
            }
        }
    }
}
