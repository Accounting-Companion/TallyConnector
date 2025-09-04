using System.Reflection;
using System.Text.RegularExpressions;

namespace TallyConnector.Services;

public static class XMLToObject
{
    public static Regex XmlIndexRegex = new("[0-9]+", RegexOptions.Compiled);
    //Converts to given object from Xml
    public static Dictionary<string, XmlSerializer> _cache = [];
    public static T GetObjfromXml<T>(string Xml, XMLOverrideswithTracking? attrOverrides = null, ILogger? Logger = null)
    {
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

        try
        {
            T obj = (T)XMLSer.Deserialize(rd);
            return obj;
        }
        catch (InvalidOperationException ex)
        {
            IEnumerable<string>? internalErrors = GetInnerError(ex);
            GroupCollection groups = XmlIndexRegex.Match(ex.Message).Groups;
            string XMLPart = string.Empty;
            string ExceptionMessage = ex.Message;

            if (groups.Count > 0)
            {
                string? value = groups[0].Value;
                if (value != null)
                {
                    Type type = typeof(T);
                    string RootElement = string.Empty;
                    if (type.IsGenericType)
                    {
                        Type? internalType = type.GetGenericArguments().FirstOrDefault();
                        if (internalType != null)
                        {
                            XmlRootAttribute? xmlRootAttribute = internalType.GetCustomAttribute<XmlRootAttribute>();
                            RootElement = xmlRootAttribute?.ElementName ?? string.Empty;
                        }
                    }
                    int StartIndex = int.Parse(value) - 1;
                    string[] Splittedstrings = Xml.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    IEnumerable<string> values;
                    if (!string.IsNullOrEmpty(RootElement))
                    {
                        RootElement = $"<{RootElement} ";
                        var beforeLines = Splittedstrings.Take(StartIndex).Reverse();
                        int linenum = StartIndex;
                        foreach (string line in beforeLines)
                        {

                            linenum--;
                            if (line.Contains(RootElement))
                            {
                                internalErrors = new List<string>(internalErrors) { $"Error in {line.Trim()}" };
                                break;
                            }
                            ;
                        }
                        values = Splittedstrings.Skip(linenum).Take(StartIndex - linenum + 1);
                    }
                    else
                    {
                        values = Splittedstrings.Skip(StartIndex - 10).Take(20);

                    }
                    XMLPart = string.Join(Environment.NewLine, values);
                    ExceptionMessage = $"{ExceptionMessage} - {Splittedstrings[StartIndex].Trim()}";
                }
            }

            TallyXMLParsingException tallyXMLParsingException = new(ExceptionMessage, ex, XMLPart, internalErrors?.Reverse());
            Logger?.LogError(tallyXMLParsingException.Message);
            Logger?.LogError("Error - {errs}", tallyXMLParsingException.ExceptionTrace);
            throw tallyXMLParsingException;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex.Message);
            Logger?.LogError("Error - {errs}", GetInnerError(ex)?.Reverse().Take(2));
            throw;
        }

    }

    private static IEnumerable<string>? GetInnerError(Exception ex, List<string>? errors = null)
    {
        errors ??= [];
        errors.Add(ex.Message);
        if (ex.InnerException is not null)
        {
            GetInnerError(ex.InnerException, errors);
        }

        return errors;
    }

    public static XmlSerializer GetSerializer(Type type, XMLOverrideswithTracking attrOverrides)
    {
        Type[] types = type.GetGenericArguments();
        var hash = type.Name;
        if (types.Length != 0)
        {
            hash = type.Name + types[0].FullName;
        }
        hash += attrOverrides.GetHash();
        _ = _cache.TryGetValue(hash, out XmlSerializer? Serializer);
        if (Serializer == null)
        {
            Serializer = new(type, attrOverrides);
            _cache[hash] = Serializer;
        }
        return Serializer;
    }
    public static string GetHash(this XMLOverrideswithTracking overrides)
    {
        var sb = new StringBuilder();

        foreach (var (type, member, attrs) in overrides.Entries
                                                       .OrderBy(e => e.type.FullName)
                                                       .ThenBy(e => e.member))
        {
            sb.Append(type.FullName).Append(':').Append(member).Append(':');
            AppendAttributes(sb, attrs);
        }
        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes));
    }

    private static void AppendAttributes(StringBuilder sb, XmlAttributes attrs)
    {
        if (attrs.XmlAttribute is XmlAttributeAttribute xmlAttr)
            sb.Append($"Attr:{xmlAttr.AttributeName}:{xmlAttr.Namespace}:{xmlAttr.Type};");

        if (attrs.XmlArray is XmlArrayAttribute xmlArray)
            sb.Append($"Array:{xmlArray.ElementName}:{xmlArray.Namespace}:{xmlArray.IsNullable}:{xmlArray.Order};");

        foreach (XmlArrayItemAttribute arrItem in attrs.XmlArrayItems)
            sb.Append($"ArrayItem:{arrItem.ElementName}:{arrItem.Type}:{arrItem.Namespace}:{arrItem.IsNullable}:{arrItem.NestingLevel};");

        foreach (XmlElementAttribute elem in attrs.XmlElements)
            sb.Append($"Elem:{elem.ElementName}:{elem.Type}:{elem.Namespace}:{elem.IsNullable}:{elem.Order};");

        foreach (XmlAnyElementAttribute anyElem in attrs.XmlAnyElements)
            sb.Append($"AnyElem:{anyElem.Name}:{anyElem.Namespace};");

        if (attrs.XmlAnyAttribute is XmlAnyAttributeAttribute anyAttr)
            sb.Append($"AnyAttr:{anyAttr.TypeId}");

        if (attrs.XmlChoiceIdentifier is XmlChoiceIdentifierAttribute choice)
            sb.Append($"Choice:{choice.MemberName};");

        if (attrs.XmlRoot is XmlRootAttribute root)
            sb.Append($"Root:{root.ElementName}:{root.Namespace}:{root.IsNullable};");

        if (attrs.XmlText is XmlTextAttribute text)
            sb.Append($"Text:{text.Type};");

        if (attrs.XmlType is XmlTypeAttribute typeAttr)
            sb.Append($"Type:{typeAttr.TypeName}:{typeAttr.Namespace}:{typeAttr.IncludeInSchema};");
    }
}
