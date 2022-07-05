

namespace TallyConnector.Core.Models;

public class TallyBaseObject
{
    [NotMapped]
    [JsonIgnore]
    [XmlAnyElement()]
    public XmlElement[]? OtherFields { get; set; }

    [NotMapped]
    [JsonIgnore]
    [XmlAnyAttribute]
    public XmlAttribute[]? OtherAttributes { get; set; }
}

public class TallyXmlJson : TallyBaseObject
{
    /// <summary>
    /// Accepted Values //Create, Alter, Delete
    /// </summary>
    [JsonIgnore]
    [XmlAttribute(AttributeName = "Action")]
    public Action Action { get; set; }

    public string GetJson(bool Indented = false)
    {
        string Json = JsonSerializer.Serialize(this, GetType(), new JsonSerializerOptions()
        {
            WriteIndented = Indented,
            Converters = { new JsonStringEnumConverter(), new TallyDateJsonConverter() }
        });
        return Json;
    }

    public string GetXML(XmlAttributeOverrides? attrOverrides = null)
    {
        TextWriter textWriter = new StringWriter();
        XmlWriterSettings settings = new()
        {
            OmitXmlDeclaration = true,
            //NewLineChars = "&#13;&#10;", //If /r/n in Xml replace
                                         //NewLineHandling = NewLineHandling.Entitize,
            Encoding = Encoding.Unicode,
            CheckCharacters = false,
           Indent = true,
        };
        XmlSerializerNamespaces ns = new(
                     new[] { XmlQualifiedName.Empty });

        XmlSerializer xmlSerializer = attrOverrides == null ? new(this.GetType()) : new(this.GetType(), attrOverrides);
        var writer = XmlWriter.Create(textWriter, settings);
        xmlSerializer.Serialize(writer, this, ns);
        return textWriter.ToString()!;
    }


}

[XmlRoot(ElementName = "OBJECTS")]
public class BasicTallyObject : TallyXmlJson, ITallyObject, IBasicTallyObject
{
    [XmlElement(ElementName = "MASTERID")]
    public int? MasterId { get; set; }

    [XmlElement(ElementName = "GUID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? GUID { get; set; }

    [XmlElement(ElementName = "REMOTEALTGUID")]
    [Column(TypeName = $"nvarchar({Constants.GUIDLength})")]
    public string? RemoteId { get; set; }

    [XmlElement(ElementName = "ALTERID")]
    public int? AlterId { get; set; }

    public void PrepareForExport()
    {
    }

    public void RemoveNullChilds()
    {

    }

    public override string ToString()
    {
        return GUID ?? string.Empty;
    }
}