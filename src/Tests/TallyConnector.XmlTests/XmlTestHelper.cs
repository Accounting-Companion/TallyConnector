using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TallyConnector.Core.Models;
using TallyConnector.Services;
using XmlSourceGenerator.Abstractions;

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
            OmitXmlDeclaration = false,
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

    /// <summary>
    /// Generates XML using stream-based serialization (GenericXmlStreamer / IXmlStreamable).
    /// Matches the approach used in PostDTOObjectsAsyncNew.
    /// </summary>
    public static async Task<string> GenerateXmlFromStreamAsync<T>(T obj, XmlSerializationOptions? options = null)
    {
        options ??= new XmlSerializationOptions { Encoding = Encoding.Unicode, IgnoreNullValues = true,WriteIndented=true };
        using var stream = new MemoryStream();
        await GenericXmlStreamer.WriteDataToStreamAsync(stream, obj, options);
        stream.Position = 0;
        using var reader = new StreamReader(stream, options.Encoding ?? Encoding.Unicode);
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Parses XML from stream-based deserialization (GenericXmlStreamer / IXmlStreamable).
    /// Matches the approach used in GetObjectsAsyncNew.
    /// </summary>
    public static T? ParseXmlFromStream<T>(string xml, XmlSerializationOptions? options = null) where T : new()
    {
        var bytes = Encoding.Unicode.GetBytes(xml);
        using var stream = new MemoryStream(bytes);
        return GenericXmlStreamer.ReadDataFromStream<T>(stream, options);
    }
}
