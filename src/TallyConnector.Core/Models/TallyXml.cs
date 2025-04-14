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