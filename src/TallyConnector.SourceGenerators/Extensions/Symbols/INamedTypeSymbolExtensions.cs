namespace TallyConnector.SourceGenerators.Extensions.Symbols;
public static class INamedTypeSymbolExtensions
{
    public static bool CheckInterface(this INamedTypeSymbol symbol, string FullIntefaceName)
    {
        foreach (var item in symbol.Interfaces)
        {
            string v1 = item.OriginalDefinition.ToString();
            bool v = FullIntefaceName == v1;
            if (v) return v;
        }
        foreach (var item in symbol.Interfaces)
        {
            bool v = item.CheckInterface(FullIntefaceName);
            if (v) return true;
        }
        if (symbol.BaseType != null)
        {
            bool v = symbol.BaseType.CheckInterface(FullIntefaceName);
            if (v) return true;

        }
        return false;
    }
    public static IEnumerable<ISymbol> GetPropertiesAndFields(this INamedTypeSymbol symbol)
    {


        return symbol
            .GetMembers()
            .Where(c =>
            {
                bool includeField = c.Kind == SymbolKind.Field && c.GetAttributes().Any(c => c.GetAttrubuteMetaName() == "System.Xml.Serialization.XmlElementAttribute");
                bool include = c.Kind == SymbolKind.Property || includeField;
                return include && !c.IsAbstract;
            });
    }
    // Get Properties of current class and Base class 
    public static IEnumerable<ISymbol> GetAllPropertiesAndFields(this INamedTypeSymbol getType, bool onlycurrent = false)
    {
        IEnumerable<ISymbol> info = getType.GetPropertiesAndFields();
        if (getType.BaseType != null && getType.BaseType.OriginalDefinition.ToString() != "object" && !onlycurrent)
        {
            info = info.Concat(GetAllPropertiesAndFields(getType.BaseType));
        }
        return info;
    }
    public static string GetXmlRootFromClassSymbol(this INamedTypeSymbol symbol)
    {
        System.Collections.Immutable.ImmutableArray<AttributeData> attributeDatas = symbol.GetAttributes();
        var Name = symbol.Name.ToUpper();
        foreach (AttributeData attributeData in attributeDatas)
        {
            if (attributeData.GetAttrubuteMetaName() == "System.Xml.Serialization.XmlRootAttribute")
            {
                if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
                {
                    Name = attributeData.ConstructorArguments.FirstOrDefault().Value?.ToString();
                }
            }


        }

        return Name;
    }

    public static string GetClassMetaName(this INamedTypeSymbol namedTypeSymbol)
    {
        if (namedTypeSymbol.IsGenericType)
        {
            string name = namedTypeSymbol.OriginalDefinition.ToString();
            name = name.Split('<').First();
            return name;
        }
        else
        {
            string attributeMetaName = namedTypeSymbol.OriginalDefinition.ToString();
            return attributeMetaName;
        }

    }
}
