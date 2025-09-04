namespace TallyConnector.Core.Models;



public class TallyXml 
{
    public string GetXML(XmlAttributeOverrides? attrOverrides = null, bool indent = false)
    {
        TextWriter textWriter = new StringWriter();
        XmlWriterSettings settings = new()
        {
            OmitXmlDeclaration = true,
            //NewLineChars = "&#13;&#10;", //If /r/n in Xml replace
            NewLineHandling = NewLineHandling.Entitize,
            Encoding = Encoding.Unicode,
            CheckCharacters = false,
            Indent = indent,
        };
        XmlSerializerNamespaces ns = new(new[] { XmlQualifiedName.Empty });

        XmlSerializer xmlSerializer = attrOverrides == null ? new(this.GetType()) : new(this.GetType(), attrOverrides);
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, this, ns);
        return textWriter.ToString()!;
    }

}
public class XMLOverrideswithTracking : XmlAttributeOverrides
{
    private readonly List<(Type type, string member, XmlAttributes attrs)> _entries = new();

    public new void Add(Type type, string member, XmlAttributes attributes)
    {
        base.Add(type, member, attributes);
        _entries.Add((type, member, attributes));
    }

    public IEnumerable<(Type type, string member, XmlAttributes attrs)> Entries => _entries;
}