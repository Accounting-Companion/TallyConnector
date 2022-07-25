using System.Xml.Xsl;

namespace TallyConnector.Services;
public static class XMLToObject
{
    //Converts to given object from Xml
    public static Dictionary<string, XmlSerializer> _cache = new();
    public static T? GetObjfromXml<T>(string Xml, XmlAttributeOverrides? attrOverrides = null, ILogger? Logger = null)
    {
        string re = @"(?!₹)[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
        //string re = @"[^\x0\]";
        Xml = System.Text.RegularExpressions.Regex.Replace(Xml, re, "");
        XmlSerializer XMLSer = attrOverrides == null ? new(typeof(T)) : GetSerializer(typeof(T), attrOverrides);

        NameTable nt = new();
        XmlNamespaceManager nsmgr = new(nt);
        nsmgr.AddNamespace("UDF", "TallyUDF");
        XmlParserContext context = new(null, nsmgr, null, XmlSpace.None, Encoding.Unicode);

        XmlReaderSettings xset = new()
        {
            CheckCharacters = false,
            ConformanceLevel = ConformanceLevel.Fragment
        };
        XmlReader rd = XmlReader.Create(new StringReader(Xml), xset, context);
        //StringReader XmlStream = new StringReader(Xml);
        if (typeof(T).Name.Contains("VoucherEnvelope"))
        {
            XmlReader xslreader = XmlReader.Create(new StringReader("<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:template match=\"@*|node()\">    <xsl:copy>        <xsl:apply-templates select=\"@*|node()\" />    </xsl:copy></xsl:template><xsl:template match=\"/ENVELOPE/BODY/DATA/TALLYMESSAGE/VOUCHER/LEDGERENTRIES.LIST\">		<ALLLEDGERENTRIES.LIST><xsl:apply-templates select=\"@*|node()\" /></ALLLEDGERENTRIES.LIST></xsl:template>   <xsl:template match=\"/ENVELOPE/BODY/DATA/TALLYMESSAGE/VOUCHER/INVENTORYENTRIES.LIST\">		   <ALLINVENTORYENTRIES.LIST><xsl:apply-templates select=\"@*|node()\" /></ALLINVENTORYENTRIES.LIST>	   </xsl:template></xsl:stylesheet>"));
            XslCompiledTransform xslTransform = new();
            xslTransform.Load(xslreader);
            StringWriter textWriter = new();
            XmlWriter xmlwriter = XmlWriter.Create(textWriter, new XmlWriterSettings() { OmitXmlDeclaration = true, Encoding = Encoding.Unicode });
            xslTransform.Transform(rd, null, xmlwriter);
            rd = XmlReader.Create(new StringReader(textWriter.ToString()), xset, context);
        }
        try
        {
            T? obj = (T?)XMLSer.Deserialize(rd);
            return obj;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex.Message);
            Logger?.LogError("Error - XML {xml}", Xml);
            return default;
        }

    }

    public static XmlSerializer GetSerializer(Type type, XmlAttributeOverrides attrOverrides)
    {
        var hash = type.Name + type.GetGenericArguments()[0].Name;
        _ = _cache.TryGetValue(hash, out XmlSerializer? Serializer);
        if (Serializer == null)
        {
            Serializer = new(type, attrOverrides);
            _cache[hash] = Serializer;
        }
        return Serializer;
    }
}
