
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using TC.TDLReportSourceGenerator.Execute;

namespace TC.TDLReportSourceGenerator.Models;

internal class SymbolData
{
    public SymbolData(INamedTypeSymbol parentSymbol,
                      INamedTypeSymbol symbol,
                      string methodName,
                      bool isChild = false)
    {
        ParentSymbol = parentSymbol;
        Symbol = symbol;
        TypeName = methodName;
        Name = symbol.Name;
        Attributes = Symbol.GetAttributes();
        FullName = symbol.OriginalDefinition.ToString();
        ParentNameSpace = parentSymbol.ContainingNamespace.ToString();
        ParentFullName = parentSymbol.OriginalDefinition.ToString();
        IsChild = isChild;
        IsEnum = Symbol.TypeKind is TypeKind.Enum;
        IsTallyComplexObject = Symbol.HasInterfaceWithFullyQualifiedMetadataName(TallyComplexObjectInterfaceName);
    }

    public INamedTypeSymbol ParentSymbol { get; }
    public INamedTypeSymbol Symbol { get; }
    public string TypeName { get; }
    public string Name { get; }
    public string FullName { get; }
    public string ParentNameSpace { get; set; }
    public string ParentFullName { get; private set; }
    public bool IsChild { get; private set; }
    public bool IsEnum { get; private set; }
    public bool IsTallyComplexObject { get; private set; }
    public List<ChildSymbolData> Children { get; } = [];
    public int SimpleFieldsCount { get; set; } = 0;
    public int ComplexFieldsIncludedCount { get; set; } = 0;

    public ImmutableArray<AttributeData> Attributes { get; }
    public string RootXmlTag { get; set; }
    public int ComplexFieldsCount { get; internal set; }
    public BaseSymbolData? BaseSymbolData { get; internal set; }
}
internal class BaseSymbolData : SymbolData
{
    public BaseSymbolData(INamedTypeSymbol parentSymbol,
                          INamedTypeSymbol symbol,
                          string methodName,
                          bool isChild = false) : base(parentSymbol, symbol, methodName, isChild)
    {
    }
    public bool Exclude { get; set; }
}
internal class ChildSymbolData
{
    public ChildSymbolData(ISymbol childSymbol, SymbolData parent)
    {
        Parent = parent;
        ChildSymbol = childSymbol;
        ChildType = GetChildType();
        ChildTypeFullName = ChildType.OriginalDefinition.ToString();
        Name = childSymbol.Name;
        IsComplex = ChildType.SpecialType is SpecialType.None && ChildType.TypeKind is not TypeKind.Enum;
        Attributes = childSymbol.GetAttributes();
    }


    private INamedTypeSymbol GetChildType()
    {
        INamedTypeSymbol? type = null;
        switch (ChildSymbol)
        {
            case IPropertySymbol propertySymbol:
                type = (INamedTypeSymbol)propertySymbol.Type;
                break;
            case IFieldSymbol fieldSymbol:
                type = (INamedTypeSymbol)fieldSymbol.Type;
                break;
            default:
                break;
        }
        if (type == null)
        {
            throw new Exception($"{nameof(type)} cannot be null in {nameof(GetChildType)} method");
        }
        if (type.IsGenericType && type.HasInterfaceWithFullyQualifiedMetadataName(IEnumerableInterfaceName))
        {
            IsList = true;
            return (INamedTypeSymbol)type.TypeArguments[0];
        }
        if (type.IsValueType && type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            INamedTypeSymbol typeSymbol = (INamedTypeSymbol)type.TypeArguments[0];
            if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                IsEnum = true;
            }
            return typeSymbol;
        }
        return type;
    }

    public bool IsComplex { get; }

    public bool IsList { get; private set; }
    public bool IsEnum { get; private set; }

    public string Name { get; }
    public ISymbol ChildSymbol { get; }

    public ImmutableArray<AttributeData> Attributes { get; }
    public INamedTypeSymbol ChildType { get; private set; }
    public string ChildTypeFullName { get; private set; }
    public bool Exclude { get; set; }

    public SymbolData Parent { get; }
    public SymbolData? SymbolData { get; set; }
    public TDLFieldData? TDLFieldDetails { get; set; }
    public string XmlTag { get; set; }
    public TDLCollectionData? TDLCollectionDetails { get; internal set; }
    public string? ListXmlTag { get; internal set; }
}