namespace TallyConnector.SourceGenerators.Models;
public class XmlWriterArgs : IXmlWriterArg
{
    public string XmlTag { get; set; }
    public List<XmlWriterElement> Elements { get; set; } = new();
    public List<XmlWriterArgs> ComplexElements { get; set; } = new();

    public string Name { get; set; }

    public Dictionary<string, string> Attributes { get; set; }
    public int SortOrder { get; set; }
}
public interface IXmlWriterArg
{
    string XmlTag { get; set; }
    string Name { get; set; }
    int SortOrder { get; set; }
}
public class XmlWriterElement : IXmlWriterArg
{
    public string XmlTag { get; set; }

    public string Name { get; set; }
    public int SortOrder { get; set; }

}