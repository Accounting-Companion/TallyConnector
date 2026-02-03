using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TallyConnector.Core.Models;
using TallyConnector.Services;

namespace TallyConnector.XmlTests;

public static class XmlTestHelper
{
    public static T ParseXml<T>(string xml)
    {
        return XMLToObject.GetObjfromXml<T>(xml);
    }

    /// <summary>
    /// Generates XML using V6 service overrides (default).
    /// </summary>
    public static string GenerateXml<T>(T obj)
    {
        return GenerateXml(obj, new());
    }

    /// <summary>
    /// Generates XML using provided XMLOverrides (for version-specific serialization).
    /// Matches TallyXml.GetXML() settings.
    /// </summary>
    public static string GenerateXml<T>(T obj, XMLOverrideswithTracking overrides)
    {
        // Use actual runtime type to handle DTO types correctly
        var serializer = XMLToObject.GetSerializer(obj!.GetType(), overrides);
        
        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.Entitize,
            Encoding = System.Text.Encoding.Unicode,
            CheckCharacters = false,
            Indent = true
        };
        
        XmlSerializerNamespaces ns = new([System.Xml.XmlQualifiedName.Empty]);
        
        using var stringWriter = new StringWriter();
        using var writer = XmlWriter.Create(stringWriter, settings);
        serializer.Serialize(writer, obj, ns);
        return stringWriter.ToString();
    }
}
