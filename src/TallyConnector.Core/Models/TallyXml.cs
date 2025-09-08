namespace TallyConnector.Core.Models;



public class TallyXml 
{
    public string GetXML(XMLOverrideswithTracking? attrOverrides = null, bool indent = false)
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
        XmlSerializerNamespaces ns = new([XmlQualifiedName.Empty]);

        XmlSerializer xmlSerializer = attrOverrides == null ? new(this.GetType()) : new(this.GetType(), attrOverrides);
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, this, ns);
        return textWriter.ToString()!;
    }

}

public class XMLOverrideswithTracking : XmlAttributeOverrides
{
    private readonly Dictionary<(Type type, string member), XmlAttributes> _entries = [];

    public new void Add(Type type, string member, XmlAttributes attributes)
    {
        base.Add(type, member, attributes);

        // overwrite if already exists
        _entries[(type, member)] = attributes;
    }

    /// <summary>
    /// Get all tracked entries.
    /// </summary>
    public IEnumerable<(Type type, string member, XmlAttributes attrs)> Entries =>
        _entries.Select(kvp => (kvp.Key.type, kvp.Key.member, kvp.Value));

    /// <summary>
    /// Checks if an override exists for the given type and member.
    /// </summary>
    public bool Contains(Type type, string member) =>
        _entries.ContainsKey((type, member));

    /// <summary>
    /// Try to get XmlAttributes for a given type and member.
    /// </summary>
    public bool TryGet(Type type, string member, out XmlAttributes attributes) =>
        _entries.TryGetValue((type, member), out attributes);
}