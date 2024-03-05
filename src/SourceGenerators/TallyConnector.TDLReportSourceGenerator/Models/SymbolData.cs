
using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Immutable;
using System.Xml.Serialization;
using TallyConnector.TDLReportSourceGenerator.Execute;
using TallyConnector.TDLReportSourceGenerator.Extensions.Symbols;

namespace TallyConnector.TDLReportSourceGenerator.Models;

internal class SymbolData
{
    public SymbolData(INamedTypeSymbol mainSymbol,
                      INamedTypeSymbol symbol,
                      string methodName,
                      INamedTypeSymbol reqEnvelope,
                      bool isChild = false,
                      SymbolData? parentSymbol = null)
    {
        MainSymbol = mainSymbol;
        Symbol = symbol;
        TypeName = methodName;
        ReqEnvelopeSymbol = reqEnvelope;
        Name = symbol.Name;
        Attributes = Symbol.GetAttributes();
        FullName = symbol.OriginalDefinition.ToString();
        MainNameSpace = mainSymbol.ContainingNamespace.ToString();
        MainFullName = mainSymbol.OriginalDefinition.ToString();
        IsChild = isChild;
        ParentSymbol = parentSymbol;
        IsEnum = Symbol.TypeKind is TypeKind.Enum;
        IsTallyComplexObject = Symbol.HasInterfaceWithFullyQualifiedMetadataName(TallyComplexObjectInterfaceName);
    }

    public INamedTypeSymbol MainSymbol { get; }
    public INamedTypeSymbol Symbol { get; }
    public string TypeName { get; }
    public INamedTypeSymbol ReqEnvelopeSymbol { get; }
    public string Name { get; }
    public string FullName { get; }
    public string MainNameSpace { get; set; }
    public string MainFullName { get; private set; }
    public bool IsChild { get; private set; }
    public bool IsBaseSymbol { get; set; }
    public SymbolData? ParentSymbol { get; private set; }
    public bool IsEnum { get; private set; }
    public bool IsTallyComplexObject { get; private set; }
    public List<ChildSymbolData> Children { get; } = [];
    public int SimpleFieldsCount { get; set; } = 0;
    public int TDLLinesCount { get; set; } = 0;

    public ImmutableArray<AttributeData> Attributes { get; }
    public string RootXmlTag { get; set; }
    public int TDLPartsCount { get; internal set; }
    public BaseSymbolData? BaseSymbolData { get; internal set; }

    public FunctionDetails TDLFunctionMethods { get; set; } = [];
    public FunctionDetails TDLNameSetMethods { get; set; } = [];
    public FunctionDetails TDLCollectionMethods { get; set; } = [];
    public FunctionDetails TDLGetFilterMethods { get; set; } = [];
    public FunctionDetails TDLGetObjectMethods { get; set; } = [];
    public TDLCollectionData? TDLCollectionDetails { get; internal set; }
    public MapToData? MapToData { get; internal set; }
    public string MethodNameSuffixPlural { get; internal set; }
    public GenerationMode GenerationMode { get; internal set; }
    public List<INamedTypeSymbol> Args { get; internal set; }
    public bool IsParentChild { get; set; }

    public override string ToString()
    {
        return $"Symbol - {Name}";
    }
}


internal class FunctionDetails : IEnumerable<KeyValuePair<string, FunctionDetail>>
{
    public Dictionary<string, FunctionDetail> _functiondetails = [];
    public void Add(IMethodSymbol methodSymbol, SymbolData symbolData)
    {
        var detaill = new FunctionDetail(methodSymbol, symbolData);
        Add(detaill);
    }
    public void Add(FunctionDetail detaill)
    {
        if (!_functiondetails.ContainsKey(detaill.FullName))
        {
            _functiondetails.Add(detaill.FullName, detaill);
            Count++;
        }

    }
    public int Count { get; internal set; }

    public IEnumerator<KeyValuePair<string, FunctionDetail>> GetEnumerator()
    {
        return _functiondetails.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
internal class FunctionDetail
{
    public FunctionDetail(IMethodSymbol symbol, SymbolData symbolData)
    {
        Symbol = symbol;
        SymbolData = symbolData;
        FunctionName = symbol.Name;
        FullName = symbol.OriginalDefinition.ToString();
    }

    public string FunctionName { get; set; }
    public string FullName { get; set; }
    public IMethodSymbol Symbol { get; set; }
    public SymbolData SymbolData { get; }
}
internal class BaseSymbolData : SymbolData
{
    public BaseSymbolData(INamedTypeSymbol parentSymbol,
                          INamedTypeSymbol symbol,
                          INamedTypeSymbol reqEnvelope,
                          string methodName,
                          bool isChild = false, SymbolData? parentSymbolData = null) : base(parentSymbol, symbol, methodName, reqEnvelope, isChild, parentSymbolData)
    {
    }
    public bool Exclude { get; set; }

    public SymbolData SymbolData { get; set; }
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
        MainParent = Parent.ParentSymbol;
        IsComplex = ChildType.SpecialType is SpecialType.None && ChildType.TypeKind is not TypeKind.Enum;
        Attributes = childSymbol.GetAttributes();
        ReportVarName = $"{parent.Name}{Name}ReportName";
        IsOverridden = /*childSymbol.DeclaringSyntaxReferences.Any(c => c.GetSyntax() is PropertyDeclarationSyntax propertySyntax && propertySyntax.Modifiers.Any(c => c.IsKind(SyntaxKind.NewKeyword))) || */CheckBaseHasSameProperty();
    }

    private bool CheckBaseHasSameProperty()
    {
        if (Parent.BaseSymbolData !=null)
        {
            OverriddenChild = GetOverriddenChild(Parent.BaseSymbolData!.SymbolData);
            OverriddenChild?.OverriddenBy.Add(this);
            ChildSymbolData? GetOverriddenChild(SymbolData data)
            {
                ChildSymbolData? childSymbolData = data.Children.Where(c => c.Name == Name).FirstOrDefault();
                if (childSymbolData == null && data.BaseSymbolData != null)
                {
                    childSymbolData = GetOverriddenChild(data.BaseSymbolData.SymbolData);
                }
                return childSymbolData;
            }
        }
        
        return OverriddenChild is not null;


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
            case INamedTypeSymbol symbol:
                type = (INamedTypeSymbol)symbol;
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
        IsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
        if (type.IsValueType && IsNullable)
        {
            INamedTypeSymbol typeSymbol = (INamedTypeSymbol)type.TypeArguments[0];
            if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                IsEnum = true;
            }
            return typeSymbol;
        }
        if (type.TypeKind == TypeKind.Enum)
        {
            IsEnum = true;
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
    public SymbolData? MainParent { get; private set; }
    public TDLFieldData? TDLFieldDetails { get; set; }
    public string XmlTag { get; set; }
    public TDLCollectionData? DefaultTDLCollectionDetails { get; internal set; }
    public string? ListXmlTag { get; internal set; }

    public string ReportVarName { get; set; }
    public bool IsOverridden { get; private set; }
    public bool IsNullable { get; private set; }
    public bool IgnoreForCreateDTO { get; internal set; }
    public List<string>? EnumChoices { get; internal set; }
    public List<XMLData> XMLData { get; internal set; }
    public ChildSymbolData? OverriddenChild { get; internal set; }
    public List<ChildSymbolData> OverriddenBy { get; set; } = [];

    public override string ToString()
    {
        return $"Child Symbol - {Name}";
    }
}
