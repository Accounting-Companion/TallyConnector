using System.Collections.Immutable;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public static class Utils
{
    public static string GenerateUniqueNameSuffix(string combinedInput, int length = 4)
    {

        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedInput));
        var hash = Convert.ToBase64String(hashBytes);
        StringBuilder sb = new(length);
        hash = hash.Substring(0, length);
        foreach (char c in hash)
        {
            switch (c)
            {
                case '+':
                    sb.Append('P'); // Replace '+' with 'P' (or another valid char like '_')
                    break;
                case '/':
                    sb.Append('S'); // Replace '/' with 'S' (or another valid char like '_')
                    break;
                case '=':
                    // Skip Base64 padding character
                    break;
                default:
                    // Append only if it's a letter or digit to keep the suffix clean
                    if (char.IsLetterOrDigit(c))
                    {
                        sb.Append(c);
                    }
                    // Other characters are ignored
                    break;
            }
        }
        var finalHash = sb.ToString();
        if (finalHash.Length < 4)
        {
            finalHash = finalHash.PadRight(4, 'X');
        }
        return finalHash.ToUpper();
    }
    public static void SafeAdd(this List<SyntaxNodeOrToken> nodesAndTokens, SyntaxNodeOrToken item)
    {
        if (nodesAndTokens.Count != 0)
        {
            nodesAndTokens.Add(Token(SyntaxKind.CommaToken));
        }
        nodesAndTokens.Add(item);
    }

    public static void SafeAddArgument(this List<SyntaxNodeOrToken> nodesAndTokens, ExpressionSyntax item)
    {
        nodesAndTokens.SafeAdd(Argument(item));
    }
    public static void SafeAddExpressionElement(this List<SyntaxNodeOrToken> nodesAndTokens, ExpressionSyntax item)
    {
        nodesAndTokens.SafeAdd(ExpressionElement(item));
    }

    public static IEnumerable<PropertyData> GetAllProperties(this ModelData modelData, HashSet<string>? visited = null)
    {
        visited ??= [];
        List<PropertyData> properties = [];
        properties.AddRange(modelData.Properties.Values);
        foreach (var node in modelData.Properties.Values)
        {
            if (node.OriginalModelData != null && visited.Add(node.OriginalModelData.FullName))
            {
                properties.AddRange(GetAllProperties(node.OriginalModelData, visited));
            }
        }
        for (BaseModelData? currentSymbol = modelData.BaseData; currentSymbol != null && currentSymbol.ModelData != null; currentSymbol = currentSymbol.ModelData.BaseData)
        {
            if (visited.Add(currentSymbol.ModelData.FullName))
            {
                //properties.AddRange(currentSymbol.ModelData.Properties.Values);
                properties.AddRange(GetAllProperties(currentSymbol.ModelData, visited));
            }
        }
        return properties;
    }

    public static IEnumerable<PropertyData> GetAllProperties(IEnumerable<PropertyData> properties,
                                                             HashSet<string>? visited = null)
    {
        visited ??= [];
        List<PropertyData> allProperties = [];
        foreach (var property in properties)
        {
            if (property.OriginalModelData != null && visited.Add(property.OriginalModelData.FullName))
            {
                List<PropertyData> directProperties = GetAllDirectProperties(property.OriginalModelData,
                                                                             visited);
                allProperties.AddRange(directProperties);
                allProperties.AddRange(GetAllProperties(directProperties,visited));
                
            }
            foreach (var xMLData in property.XMLData)
            {
                if (xMLData.ModelData != null && visited.Add(xMLData.ModelData.FullName))
                {
                    List<PropertyData> innerdirectProperties = GetAllDirectProperties(xMLData.ModelData,
                                                                         visited);
                    allProperties.AddRange(innerdirectProperties);
                    allProperties.AddRange(GetAllProperties(innerdirectProperties, visited));
                }
            }
        }
        return allProperties;
    }

    public static List<PropertyData> GetAllDirectProperties(this ModelData modelData,
                                                            HashSet<string>? visited = null)
    {
        List<PropertyData> properties = [];
        visited ??= [];
        if (modelData.IsEnum) return properties;
        for (BaseModelData? currentSymbol = modelData.BaseData; currentSymbol != null && currentSymbol.ModelData != null; currentSymbol = currentSymbol.ModelData.BaseData)
        {
            if (visited.Add(currentSymbol.ModelData.FullName))
            {
                properties.AddRange(GetAllDirectProperties(currentSymbol.ModelData, visited));
            }
        }
        
        properties.AddRange(modelData.Properties.Values);
        return properties;
    }


    public static void CopyFrom(this HashSet<string> dest,HashSet<string> src)
    {
        foreach (var item in src)
        {
            dest.Add(item);
        }
    }
}

