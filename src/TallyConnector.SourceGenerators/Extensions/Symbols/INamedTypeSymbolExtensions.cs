using System;
using System.Collections.Generic;
using System.Text;

namespace TallyConnector.SourceGenerators.Extensions.Symbols;
public static class INamedTypeSymbolExtensions
{
    public static bool CheckInterface(this INamedTypeSymbol symbol, string FullIntefaceName)
    {
        foreach (var item in symbol.Interfaces)
        {
            bool v = FullIntefaceName == item.OriginalDefinition.ToString();
            if (v) return v;
        }
        foreach (var item in symbol.Interfaces)
        {
            bool v = item.CheckInterface(FullIntefaceName);
            if (v) return true;
        }

        return false;
    }
    public static IEnumerable<ISymbol> GetProperties(this INamedTypeSymbol symbol)
    {
        return symbol.GetMembers().Where(c => c.Kind == SymbolKind.Property);
    }
}
