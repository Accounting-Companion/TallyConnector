namespace TallyConnector.TDLReportSourceGenerator.Extensions.Symbols;
/// <summary>
/// Extension methods for the <see cref="INamedTypeSymbol"/> type.
/// </summary>
public static class INamedTypeSymbolExtensions
{
    /// <summary>
    /// Checks whether or not a given type symbol has a specified fully qualified metadata name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name to check.</param>
    /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
    public static bool HasFullyQualifiedMetadataName(this ITypeSymbol symbol, string name)
    {
        return symbol.OriginalDefinition.ToString() == name;

    }
    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> implements an interface with a specified name.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for interface implementation.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an interface with the specified name.</returns>
    public static bool HasInterfaceWithFullyQualifiedMetadataName(this ITypeSymbol typeSymbol, string name)
    {
        foreach (INamedTypeSymbol interfaceType in typeSymbol.AllInterfaces)
        {
            if (interfaceType.HasFullyQualifiedMetadataName(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for inheritance.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> is or inherits from <paramref name="name"/>.</returns>
    public static bool HasOrInheritsFromFullyQualifiedMetadataName(this ITypeSymbol typeSymbol, string name)
    {
        for (ITypeSymbol? currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.HasFullyQualifiedMetadataName(name))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gets all member symbols from a given <see cref="INamedTypeSymbol"/> instance, including inherited ones.
    /// </summary>
    /// <param name="symbol">The input <see cref="INamedTypeSymbol"/> instance.</param>
    /// <returns>A sequence of all member symbols for <paramref name="symbol"/>.</returns>
    public static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol symbol)
    {
        for (INamedTypeSymbol? currentSymbol = symbol; currentSymbol is { SpecialType: not SpecialType.System_Object }; currentSymbol = currentSymbol.BaseType)
        {
            foreach (ISymbol memberSymbol in currentSymbol.GetMembers())
            {
                yield return memberSymbol;
            }
        }
    }

    /// <summary>
    /// Gets all member symbols from a given <see cref="INamedTypeSymbol"/> instance, including inherited ones.
    /// </summary>
    /// <param name="symbol">The input <see cref="INamedTypeSymbol"/> instance.</param>
    /// <param name="name">The name of the members to look for.</param>
    /// <returns>A sequence of all member symbols for <paramref name="symbol"/>.</returns>
    public static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol symbol, string name)
    {
        for (INamedTypeSymbol? currentSymbol = symbol; currentSymbol is { SpecialType: not SpecialType.System_Object }; currentSymbol = currentSymbol.BaseType)
        {
            foreach (ISymbol memberSymbol in currentSymbol.GetMembers(name))
            {
                yield return memberSymbol;
            }
        }
    }
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
    public static bool CheckBaseClass(this INamedTypeSymbol symbol, string FullClassName)
    {
        if (symbol.BaseType != null)
        {
            string v1 = symbol.BaseType.OriginalDefinition.ToString();
            bool v = FullClassName == v1;
            if (v) { return v; }
            return symbol.BaseType.CheckBaseClass(FullClassName);
        }

        return false;
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
