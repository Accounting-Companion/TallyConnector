using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

  



    public static void CopyFrom(this HashSet<string> dest, HashSet<string> src)
    {
        foreach (var item in src)
        {
            dest.Add(item);
        }
    }

    public static ClassPropertyData? GetOveriddenProperty(this ClassData classData, string propName)
    {
        if (classData.BaseData == null) return null;
        if (classData.BaseData.Members.TryGetValue(propName, out var member)) return member;
        return classData.BaseData.GetOveriddenProperty(propName);
    }
    public static void AddText(this List<InterpolatedStringContentSyntax> interpolatedStringContentSyntaxes, string text)
    {
        interpolatedStringContentSyntaxes.Add(InterpolatedStringText()
                                        .WithTextToken(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.InterpolatedStringTextToken,
                                                text.Replace("\"", "\\\""),
                                                text,
                                                TriviaList())));
    }
    public static void AddIdentifier(this List<InterpolatedStringContentSyntax> interpolatedStringContentSyntaxes, string identifier)
    {
        interpolatedStringContentSyntaxes.Add(Interpolation(
                                            IdentifierName(identifier)));
    }
    public static HashSet<string> GetUniqueMemberNames(this ClassData modelData, HashSet<string>? visited = null)
    {
        List<string> properties = [];
        visited ??= [];

        for (ClassData? currentSymbol = modelData; currentSymbol != null; currentSymbol = currentSymbol.BaseData)
        {
            if (visited.Add(currentSymbol.FullName))
            {
                properties.InsertRange(0, currentSymbol.Members.Values.Where(c => !(c.IsComplex || c.IsList)).Select(c => c.Name));
            }
        }
        return [.. properties];
    }
    public static List<ClassPropertyData> GetClassMembers(this ClassData modelData, HashSet<string>? visited = null)
    {
        List<ClassPropertyData> properties = [];
        visited ??= [];

        for (ClassData? currentSymbol = modelData; currentSymbol != null; currentSymbol = currentSymbol.BaseData)
        {
            if (visited.Add(currentSymbol.FullName))
            {
                var allProps = currentSymbol.Members.Values;
                foreach (var item in allProps.Where(c => c.IsComplex))
                {
                    if (item.ClassData == null)
                    {
                        continue;
                    }
                    properties.InsertRange(0, item.ClassData.GetClassMembers(visited));
                }
                ;
                properties.InsertRange(0, allProps);
            }
        }
        return [.. properties];
    }
    public static void AppendDict(this Dictionary<string, UniqueMember> src,
                                  Dictionary<string, ClassPropertyData> src2,
                                  string prefixPath = "")
    {
        foreach (var item in src2)
        {
            src[item.Key] = new(prefixPath, item.Value);
        }
    }
    public static Dictionary<string, ClassPropertyData> ToDict(this IEnumerable<ClassPropertyData> src2, Func<ClassPropertyData, string>? selector = null)
    {
        selector ??= c => c.UniqueName;
        Dictionary<string, ClassPropertyData> src = [];
        foreach (var item in src2)
        {
            src[selector(item)] = item;
        }
        return src;
    }
    public static void AppendDict(this Dictionary<string, UniqueMember> src,
                                  Dictionary<string, UniqueMember> src2)
    {
        foreach (var item in src2)
        {
            src[item.Key] = item.Value;
        }
    }

    public static void RemoveDict(this Dictionary<string, UniqueMember> src, IEnumerable<string> keys)
    {
        foreach (var item in keys)
        {
            src.Remove(item);
        }
    }

    public static void AppendDict(this Dictionary<string, ClassPropertyData> src,
                                      Dictionary<string, ClassPropertyData> src2)
    {
        foreach (var item in src2)
        {
            src[item.Key] = item.Value;
        }
    }
    public static void AppendDict(this Dictionary<string, ClassPropertyData> src,
                                      Dictionary<string, ClassPropertyData> src2, string prefixPath = "")
    {
        foreach (var item in src2)
        {
            src[$"{prefixPath}{item.Value.Name}"] = item.Value;
        }
    }
}

