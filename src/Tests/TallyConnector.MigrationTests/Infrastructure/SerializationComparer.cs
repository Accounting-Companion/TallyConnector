using System.Text;

namespace TallyConnector.MigrationTests.Infrastructure;

/// <summary>
/// Utility class for comparing XML serialization outputs
/// </summary>
public static class SerializationComparer
{
    /// <summary>
    /// Compares two XML strings semantically, ignoring whitespace and attribute order
    /// </summary>
    public static bool AreXmlEquivalent(string xml1, string xml2, out string? difference)
    {
        try
        {
            var doc1 = XDocument.Parse(xml1);
            var doc2 = XDocument.Parse(xml2);

            // Normalize both documents
            NormalizeXml(doc1.Root);
            NormalizeXml(doc2.Root);

            // Compare
            bool areEqual = XNode.DeepEquals(doc1, doc2);
            
            if (!areEqual)
            {
                difference = GenerateDifference(doc1, doc2);
            }
            else
            {
                difference = null;
            }

            return areEqual;
        }
        catch (Exception ex)
        {
            difference = $"Error parsing XML: {ex.Message}\n\nXML1:\n{xml1}\n\nXML2:\n{xml2}";
            return false;
        }
    }

    /// <summary>
    /// Normalizes XML by removing insignificant whitespace and sorting attributes
    /// </summary>
    private static void NormalizeXml(XElement? element)
    {
        if (element == null) return;

        // Sort attributes by name
        var attributes = element.Attributes().OrderBy(a => a.Name.ToString()).ToList();
        element.RemoveAttributes();
        foreach (var attr in attributes)
        {
            element.Add(attr);
        }

        // Recursively normalize child elements
        foreach (var child in element.Elements())
        {
            NormalizeXml(child);
        }

        // Normalize text content (trim whitespace from text-only nodes)
        if (!element.HasElements && !string.IsNullOrWhiteSpace(element.Value))
        {
            element.Value = element.Value.Trim();
        }
    }

    /// <summary>
    /// Generates a human-readable difference message
    /// </summary>
    private static string GenerateDifference(XDocument doc1, XDocument doc2)
    {
        var sb = new StringBuilder();
        sb.AppendLine("XML documents differ:");
        sb.AppendLine();
        sb.AppendLine("=== XmlSerializer Output ===");
        sb.AppendLine(doc1.ToString());
        sb.AppendLine();
        sb.AppendLine("=== XmlSourceGenerator Output ===");
        sb.AppendLine(doc2.ToString());
        return sb.ToString();
    }

    /// <summary>
    /// Serializes an object using traditional XmlSerializer
    /// </summary>
    public static string SerializeWithXmlSerializer<T>(T obj) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
            Encoding = Encoding.UTF8
        });
        
        serializer.Serialize(xmlWriter, obj);
        return stringWriter.ToString();
    }

    /// <summary>
    /// Deserializes an object using traditional XmlSerializer
    /// </summary>
    public static T DeserializeWithXmlSerializer<T>(string xml) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringReader = new StringReader(xml);
        var result = serializer.Deserialize(stringReader);
        return (T)result!;
    }
}
