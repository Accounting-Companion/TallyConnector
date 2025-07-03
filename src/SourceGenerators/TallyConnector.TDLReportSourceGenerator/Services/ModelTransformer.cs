namespace TallyConnector.TDLReportSourceGenerator.Services;
public class ModelTransformer
{
    private Dictionary<string, ClassData> _symbolsCache = [];

    public static ModelTransformer Instance => new();

    public ModelTransformer()
    {
    }

    public async Task TransformAsync(INamedTypeSymbol symbol,
                                  CancellationToken token = default)
    {
        ClassData modelData = await TransformClassSymbolAsync(symbol, token: token);
        modelData.GenerateITallyRequestableObectAttribute = modelData.Symbol
            .GetAttributes()
            .Any(c => c.HasFullyQualifiedMetadataName(Attributes.Abstractions.GenerateITallyRequestableObectAttributeeName));
    }

    private async Task<ClassData> TransformClassSymbolAsync(INamedTypeSymbol symbol, string prefixPath = "",
                                                            CancellationToken token = default)
    {
        if (_symbolsCache.TryGetValue(symbol.GetClassMetaName(), out var classData))
        {
            return classData;
        }
        classData = new ClassData(symbol);
        _symbolsCache[classData.FullName] = classData;
        if (symbol.BaseType != null && !classData.IsEnum && !symbol.BaseType.HasFullyQualifiedMetadataName("object"))
        {
            classData.BaseData = await TransformClassSymbolAsync(symbol.BaseType, prefixPath, token);
            classData.AllUniqueMembers.AppendDict(classData.BaseData.AllUniqueMembers);
        }
        await TransformMembers(classData, prefixPath, token);
        var values = classData.Members.Values;

        classData.AllUniqueMembers.AppendDict(values.ToDictionary(c => c.UniqueName, c => c), prefixPath);

        return classData;
    }
    private async Task TransformMembers(ClassData classData,
                                       string prefixPath,
                                       CancellationToken token = default)

    {
        var members = classData.Symbol.GetMembers();
        foreach (var member in members)
        {
            if (member.DeclaredAccessibility != Accessibility.Public || member.Kind != SymbolKind.Property)
            {
                if (!classData.IsEnum || member.Kind != SymbolKind.Field)
                {
                    continue;
                }
            }
            ClassPropertyData propertyData = TransformMember(member, classData);
            if (propertyData.IsComplex)
            {
                propertyData.ClassData = await TransformClassSymbolAsync(propertyData.PropertyOriginalType, $"{prefixPath}",
                                                                         token);
                classData.AllUniqueMembers
                    .AppendDict(propertyData.ClassData.Members.Values
                    .ToDictionary(c => c.UniqueName, c => c), $"{prefixPath}{propertyData.Name}.");
            }
            if (propertyData.IsOverridenProperty && propertyData.OverridenProperty != null)
            {
                if (propertyData.OverridenProperty.IsComplex)
                {
                    if (propertyData.OverridenProperty.ClassData != null)
                    {
                        var nestedSimpleOveriddenProperties = propertyData.OverridenProperty.ClassData.AllUniqueMembers.Values.Select(c => c.PropertyData.UniqueName);
                        classData.AllUniqueMembers.RemoveDict(nestedSimpleOveriddenProperties);
                    }
                }
                classData.AllUniqueMembers.RemoveDict([propertyData.OverridenProperty.UniqueName]);

            }
            classData.Members[propertyData.Name] = propertyData;

        }
    }

    private ClassPropertyData TransformMember(ISymbol member, ClassData classData)
    {
        var propData = new ClassPropertyData(member, classData);
        return propData;
    }

    internal List<ClassData> GetSymbols()
    {
        return [.. _symbolsCache.Values];
    }
}

public class ClassData
{
    public ClassData(INamedTypeSymbol symbol)
    {
        Symbol = symbol;
        IsEnum = Symbol.TypeKind == TypeKind.Enum;
    }

    public ClassData? BaseData { get; set; }
    public INamedTypeSymbol Symbol { get; }
    public bool IsEnum { get; private set; }
    public string FullName { get => Symbol.GetClassMetaName(); }
    public string Name { get => Symbol.Name; }
    public string MetaName { get => $"{Name}Meta"; }
    public string Namespace { get => Symbol.ContainingNamespace.ToString(); }

    public bool GenerateITallyRequestableObectAttribute { get; set; }
    public Dictionary<string, ClassPropertyData> Members { get; } = [];
    public Dictionary<string, UniqueMember> AllUniqueMembers { get; } = [];
    public HashSet<string> DefaultTDLFunctions { get; internal set; } = [];
    public HashSet<string> TDLFunctions { get; internal set; } = [];

    public override string ToString()
    {
        return $"ClassData : {FullName}";
    }
}
public class ClassPropertyData
{
    public ClassPropertyData(ISymbol member, ClassData classData)
    {
        MemberSymbol = member;
        ParentData = classData;
        Name = member.Name;
        PropertyOriginalType = GetChildType();
        UniqueName = $"{Name}_{Utils.GenerateUniqueNameSuffix($"{classData.FullName}_{PropertyOriginalType.GetClassMetaName()}_{Name}")}";
        IsComplex = (PropertyOriginalType.SpecialType is SpecialType.None && !DefaultSimpleTypes.Contains(PropertyOriginalType.GetClassMetaName())) && PropertyOriginalType.TypeKind is not TypeKind.Enum;
        OverridenProperty = classData.GetOveriddenProperty(Name);
        IsOverridenProperty = OverridenProperty != null;


    }

    public ISymbol MemberSymbol { get; }
    public string Name { get; }
    public string UniqueName { get; }
    public INamedTypeSymbol PropertyOriginalType { get; }
    public bool IsComplex { get; }
    public bool IsList { get; private set; }
    public bool IsNullable { get; private set; }
    public bool IsEnum { get; private set; }
    public string? XMLTag { get; private set; }
    public ClassData? ClassData { get; internal set; }
    public ClassData ParentData { get; internal set; }
    public bool IsOverridenProperty { get; }
    public ClassPropertyData? OverridenProperty { get; }

    private INamedTypeSymbol GetChildType()
    {
        INamedTypeSymbol? type = null;
        switch (MemberSymbol)
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
}

public struct UniqueMember
{
    public string Path;
    public ClassPropertyData PropertyData;

    public UniqueMember(string path, ClassPropertyData propertyData)
    {
        Path = path;
        PropertyData = propertyData;
    }
}
public class TDLClassReport
{
    public List<TDLClassProperty> TDLClassProperties { get; set; } = [];
    public int ComplexFieldsCount { get; internal set; }
    public int SimpleFieldsCount { get; internal set; }
}
public class TDLClassProperty
{
    public string VariableName { get; set; }

    public string Value { get; set; }

    public bool IsNumber { get; set; }
}