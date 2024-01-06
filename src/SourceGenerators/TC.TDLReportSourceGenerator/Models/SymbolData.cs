
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using TC.TDLReportSourceGenerator.Execute;

namespace TC.TDLReportSourceGenerator.Models;

internal class SymbolData
{
    public SymbolData(INamedTypeSymbol symbol,bool isChild = false)
    {
        Symbol = symbol;
        Name = symbol.Name;
        Attributes = Symbol.GetAttributes();
        FullName = symbol.OriginalDefinition.ToString();
        NameSpace = symbol.ContainingNamespace.ToString();
        IsChild = isChild;
    }

    public INamedTypeSymbol Symbol { get; }
    public string Name { get; }
    public string FullName { get; }
    public string NameSpace { get; private set; }
    public bool IsChild { get; private set; }
    public List<ChildSymbolData> Children { get; } = [];
    public int SimpleFieldsCount { get; set; } = 0;
    public int ComplexFieldsIncludedCount { get; set; } = 0;

    public ImmutableArray<AttributeData> Attributes { get; }
    public string RootXmlTag { get;  set; }
    public int ComplexFieldsCount { get; internal set; }
}

internal class ChildSymbolData
{
    public ChildSymbolData(IPropertySymbol childSymbol, SymbolData parent)
    {
        Parent = parent;
        ChildSymbol = childSymbol;
        ChildType = GetChildType();
        ChildTypeFullName = ChildType.OriginalDefinition.ToString();
        Name = childSymbol.Name;
        IsComplex = ChildType.SpecialType is SpecialType.None;
        Attributes = childSymbol.GetAttributes();
    }

    private INamedTypeSymbol GetChildType()
    {
        INamedTypeSymbol type = (INamedTypeSymbol)ChildSymbol.Type;
        if (type.IsGenericType && type.HasInterfaceWithFullyQualifiedMetadataName(IEnumerableInterfaceName))
        {
            IsList = true;
            return (INamedTypeSymbol)type.TypeArguments[0];
        }
        return type;
    }

    public bool IsComplex { get; }
    
    public bool IsList { get; private set; }

    public string Name { get; }
    public IPropertySymbol ChildSymbol { get; }

    public ImmutableArray<AttributeData> Attributes { get;  }
    public INamedTypeSymbol ChildType { get; private set; }
    public string ChildTypeFullName { get; private set; }
    public bool Exclude { get; set; }

    public SymbolData Parent { get; }
    public SymbolData? SymbolData { get; set; }
    public TDLFieldData? TDLFieldDetails { get;  set; }
    public string XmlTag { get;  set; }
    public TDLCollectionData? TDLCollectionDetails { get; internal set; }
}