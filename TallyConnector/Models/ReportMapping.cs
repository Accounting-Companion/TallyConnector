namespace TallyConnector.Models;

public class ReportField
{
    public ReportField(string XmlTag)
    {
        FieldName = XmlTag;
        XMLTag = $"{XmlTag}";
    }
    public ReportField(string XmlTag, List<ReportField> subFields = null)
    {
        SubFields = subFields;
        XMLTag = XmlTag;
    }

    public ReportField(string XmlTag, string colName) : this(XmlTag)
    {
        CollectionName = colName;
    }

    public ReportField(string XmlTag, string colName, string colType) : this(XmlTag, colName)
    {
        CollectionType = colType;
    }

    public string XMLTag { get; set; }
    public string FieldName { get; set; }

    public List<ReportField> SubFields { get; set; } = new();

    public List<string> Atrributes { get; set; } = new();

    public string CollectionName { get; set; }

    public string CollectionType { get; set; }
}


