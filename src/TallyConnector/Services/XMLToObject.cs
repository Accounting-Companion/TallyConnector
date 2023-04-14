using System.Text.RegularExpressions;
using System.Xml.Xsl;

namespace TallyConnector.Services;
public static class XMLToObject
{
    public static Regex XmlIndexRegex = new("[0-9]+", RegexOptions.Compiled);
    //Converts to given object from Xml
    public static Dictionary<string, XmlSerializer> _cache = new();
    public static T GetObjfromXml<T>(string Xml, XmlAttributeOverrides? attrOverrides = null, ILogger? Logger = null)
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
        StringReader stringReader = new StringReader(Xml);
        XmlReader rd = XmlReader.Create(stringReader, xset, context);
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
            T obj = (T)XMLSer.Deserialize(rd);
            return obj;
        }
        catch (InvalidOperationException ex)
        {
            GroupCollection groups = XmlIndexRegex.Match(ex.Message).Groups;
            string XMLPart = string.Empty;
            string ExceptionMessage = ex.Message;
            if (groups.Count > 0)
            {
                string? value = groups[0].Value;
                if (value != null)
                {
                    int StartIndex = int.Parse(value) - 1;
                    string[] Splittedstrings = Xml.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    IEnumerable<string> values = Splittedstrings.Skip(StartIndex - 10).Take(20);
                    XMLPart = string.Join(Environment.NewLine, values);
                    ExceptionMessage = $"{ExceptionMessage} - {Splittedstrings[StartIndex]}";
                }
            }
            TallyXMLParsingException tallyXMLParsingException = new(ExceptionMessage, ex, XMLPart, GetInnerError(ex).Reverse());
            Logger?.LogError(tallyXMLParsingException.Message);
            Logger?.LogError("Error - {errs}", tallyXMLParsingException.ExceptionTrace);
            throw tallyXMLParsingException;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex.Message);
            Logger?.LogError("Error - {errs}", GetInnerError(ex).Reverse().Take(2));
            throw ex;
        }

    }

    private static IEnumerable<string>? GetInnerError(Exception ex, List<string>? errors = null)
    {
        errors ??= new List<string>();
        errors.Add(ex.Message);
        if (ex.InnerException is not null)
        {
            GetInnerError(ex.InnerException, errors);
        }

        return errors;
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
