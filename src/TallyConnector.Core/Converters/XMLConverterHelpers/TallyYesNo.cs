using System.Diagnostics;
using System.Xml.Schema;

namespace TallyConnector.Core.Converters.XMLConverterHelpers;

[DebuggerDisplay("{Value}")]
[JsonConverter(typeof(TallyYesNoJsonConverter))]
public class TallyYesNo : IXmlSerializable
{
    public TallyYesNo()
    {
    }

    public TallyYesNo(bool isYes)
    {
        Value = isYes;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool Value { get; set; }
    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        Value = false;
        bool isEmptyElement = reader.IsEmptyElement;
        if (!isEmptyElement)
        {
            string content = reader.ReadElementContentAsString();
            if (content != null)
            {
                Value = content.ToLower() == "yes";
            }

        }

    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteString(Value ? "Yes" : "No");

    }

    XmlSchema? IXmlSerializable.GetSchema()
    {
        throw new NotImplementedException();
    }



    public static implicit operator TallyYesNo(bool IsYes)
    {
        return new(IsYes);
    }

    public static implicit operator TallyYesNo(string IsYes)
    {
        return IsYes == "yes";
    }

    public static implicit operator bool(TallyYesNo tallyYesNo)
    {
        return tallyYesNo.Value;
    }
    public static implicit operator bool?(TallyYesNo? tallyYesNo)
    {
        return tallyYesNo?.Value ?? false;
    }

    public override string ToString()
    {
        return Value ? "yes" : "no";
    }
}

