using System.Reflection;
using System.Text.RegularExpressions;

namespace TallyConnector.Services;
public static class XMLToObject
{
    public static Regex XmlIndexRegex = new("[0-9]+", RegexOptions.Compiled);
    //Converts to given object from Xml
    public static Dictionary<string, XmlSerializer> _cache = [];
    public static T GetObjfromXml<T>(string Xml, XmlAttributeOverrides? attrOverrides = null, ILogger? Logger = null)
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
            throw ;
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

    public static XmlSerializer GetSerializer(Type type, XmlAttributeOverrides attrOverrides)
    {
        Type[] types = type.GetGenericArguments();
        var hash = type.Name;
        if (types.Length != 0)
        {
            hash = type.Name + types[0].FullName;
        }
        _ = _cache.TryGetValue(hash, out XmlSerializer? Serializer);
        if (Serializer == null)
        {
            Serializer = new(type, attrOverrides);
            _cache[hash] = Serializer;
        }
        return Serializer;
    }
}
