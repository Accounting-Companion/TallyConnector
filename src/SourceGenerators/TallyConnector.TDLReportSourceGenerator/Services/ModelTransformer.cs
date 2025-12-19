using TallyConnector.TDLReportSourceGenerator.Models;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Class;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Common;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public class ModelTransformer
{
    private readonly Dictionary<string, ClassData> _symbolsCache = [];

    public static ModelTransformer Instance => new();

    public ModelTransformer()
    {
    }

    public async Task TransformAsync(INamedTypeSymbol symbol,
                                  CancellationToken token = default)
    {
        ClassData modelData = await TransformClassSymbolAsync(symbol, token: token);
    }

    private async Task<ClassData> TransformClassSymbolAsync(INamedTypeSymbol symbol, string prefixPath = "",
                                                            CancellationToken token = default)
    {
        if (_symbolsCache.TryGetValue(symbol.GetClassMetaName(), out var classData))
        {
            return classData;
        }
        classData = new ClassData(symbol);
        ClassAttributesTransformer.TransformAsync(classData, symbol.GetAttributes());
        if (string.IsNullOrEmpty(classData.DTOName)) classData.DTOName = $"{classData.Name}DTO";
        if (string.IsNullOrEmpty(classData.DTOFullName)) classData.DTOFullName = $"{classData.Namespace}.DTO.{classData.Name}DTO";
        _symbolsCache[classData.FullName] = classData;
        if (symbol.BaseType != null && !classData.IsEnum && !symbol.BaseType.HasFullyQualifiedMetadataName("object"))
        {
            classData.BaseData = await TransformClassSymbolAsync(symbol.BaseType, prefixPath, token);

            classData.AllUniqueMembers.AppendDict(classData.BaseData.AllUniqueMembers);
            classData.AllDirectMembers.AppendDict(classData.BaseData.AllDirectMembers);
            classData.AllMembers.AppendDict(classData.BaseData.AllMembers, prefixPath);
            classData.DefaultTDLFunctions.CopyFrom(classData.BaseData.DefaultTDLFunctions);
            classData.DefaultTDLObjects.CopyFrom(classData.BaseData.DefaultTDLObjects);
            classData.TDLFunctions.CopyFrom(classData.BaseData.TDLFunctions);
            classData.IsBaseIRequestableObject = classData.BaseData.Symbol.CheckInterface(Constants.Models.Interfaces.TallyRequestableObjectInterfaceFullName);
            classData.OveriddenProperties.AppendDict(classData.BaseData.OveriddenProperties);

            classData.TDLCollectionData ??= classData.BaseData.TDLCollectionData;
            classData.DefaultTDLFiltersMethod ??= classData.BaseData?.DefaultTDLFiltersMethod;
            
        }
        await TransformMembers(classData, prefixPath, token);
        var values = classData.Members.Values;

        classData.AllUniqueMembers.AppendDict(values.ToDictionary(c => c.UniqueName, c => c), prefixPath);
        classData.AllDirectMembers.AppendDict(classData.Members);

        classData.AllMembers.AppendDict(classData.AllDirectMembers, prefixPath);

        return classData;
    }
    private async Task TransformMembers(ClassData classData,
                                       string prefixPath,
                                       CancellationToken token = default)

    {
        var members = classData.Symbol.GetMembers();
        foreach (var member in members)
        {
            if (member.IsStatic && !classData.IsEnum)
            {
                continue;
            }
            if (member.DeclaredAccessibility != Accessibility.Public || member.Kind != SymbolKind.Property )
            {
                if (!classData.IsEnum || member.Kind != SymbolKind.Field)
                {
                    continue;
                }
            }
            if(member is IPropertySymbol propertySymbol)
            {
                if (propertySymbol.IsReadOnly || propertySymbol.GetMethod == null || propertySymbol.SetMethod == null)
                {
                    continue;
                }
            }
            ClassPropertyData propertyData = TransformMember(member, classData);
            if (propertyData.IsOverridenProperty && propertyData.OverridenProperty != null)
            {
                if (propertyData.OverridenProperty.IsComplex)
                {
                    if (propertyData.OverridenProperty.ClassData != null)
                    {
                        var nestedSimpleOveriddenProperties = propertyData.OverridenProperty.ClassData.AllUniqueMembers.Values.Select(c => c.PropertyData.UniqueName);
                        classData.AllUniqueMembers.RemoveDict(nestedSimpleOveriddenProperties);
                    }
                    foreach (var OverridenXmlData in propertyData.OverridenProperty.XMLData)
                    {
                        if (OverridenXmlData.ClassData == null) continue;
                        var nestedSimpleOveriddenProperties = OverridenXmlData.ClassData.AllUniqueMembers.Values.Select(c => c.PropertyData.UniqueName);
                        classData.AllUniqueMembers.RemoveDict(nestedSimpleOveriddenProperties);
                    }
                }
                classData.AllUniqueMembers.RemoveDict([propertyData.OverridenProperty.UniqueName]);
                classData.OveriddenProperties[propertyData.OverridenProperty.UniqueName] = propertyData.OverridenProperty;

            }
            if (propertyData.IsComplex)
            {
                string PropertyprefixPath = $"{prefixPath}{propertyData.Name}.";
                propertyData.ClassData = await TransformClassSymbolAsync(propertyData.PropertyOriginalType, PropertyprefixPath,
                                                                         token);
                classData.AllMembers.AppendDict(propertyData.ClassData.AllMembers, PropertyprefixPath);
                classData.AllUniqueMembers.AppendDict(propertyData.ClassData.AllUniqueMembers);
                classData.AllUniqueMembers
                    .AppendDict(propertyData.ClassData.Members.Values
                    .ToDictionary(c => c.UniqueName, c => c), PropertyprefixPath);

                classData.DefaultTDLFunctions.CopyFrom(propertyData.ClassData.DefaultTDLFunctions);
                classData.DefaultTDLObjects.CopyFrom(propertyData.ClassData.DefaultTDLObjects);

                propertyData.TDLCollectionData ??= propertyData.ClassData.TDLCollectionData;
                classData.OveriddenProperties.AppendDict(propertyData.ClassData.OveriddenProperties);
                foreach (var xMLData in propertyData.XMLData)
                {
                    if (xMLData.Symbol == null) continue;
                    xMLData.ClassData = await TransformClassSymbolAsync(xMLData.Symbol,
                                                                        $"{PropertyprefixPath}{xMLData.Symbol.Name}.",
                                                                        token);
                    xMLData.FieldName = $"{propertyData.Name}_{Utils.GenerateUniqueNameSuffix($"{xMLData.ClassData.FullName}_{propertyData.ClassData.FullName}_{propertyData.Name}")}";
                    classData.AllUniqueMembers.AppendDict(xMLData.ClassData.AllUniqueMembers);
                    classData.AllUniqueMembers
                        .AppendDict(xMLData.ClassData.Members.Values
                        .ToDictionary(c => c.UniqueName, c => c), $"{PropertyprefixPath}{xMLData.Symbol.Name}.");

                }

            }
            if (propertyData.IsEnum && !classData.IsEnum)
            {
                propertyData.ClassData = await TransformClassSymbolAsync(propertyData.PropertyOriginalType, $"{prefixPath}",
                                                                         token);

            }

            classData.Members[propertyData.Name] = propertyData;

        }
    }

    private ClassPropertyData TransformMember(ISymbol member, ClassData classData)
    {
        var propData = new ClassPropertyData(member, classData);
        PropertyAttributesTransformer.TransformAsync(propData, member.GetAttributes());

        switch (propData.PropertyOriginalType.SpecialType)
        {
            case SpecialType.System_Boolean:
                classData.DefaultTDLFunctions.Add(GetBooleanFromLogicFieldMethodName);
                propData.TDLFieldData!.Set = $"$${GetBooleanFromLogicFieldFunctionName}:{propData.TDLFieldData!.Set}";
                break;
            case SpecialType.System_DateTime:
                classData.DefaultTDLFunctions.Add(GetTransformDateToXSDMethodName);
                propData.TDLFieldData!.Set = $"$${GetTransformDateToXSDFunctionName}:{propData.TDLFieldData!.Set}";
                break;
            default:
                break;
        }

        return propData;
    }

    internal List<ClassData> GetSymbols()
    {
        return [.. _symbolsCache.Values];
    }
}

public class ClassData : IClassAttributeTranfomable
{
    public ClassData(INamedTypeSymbol symbol)
    {
        Symbol = symbol;
        IsEnum = Symbol.TypeKind == TypeKind.Enum;
        IsTallyComplexObject = symbol.CheckInterface(TallyComplexObjectInterfaceName);
        DTOName = string.Empty;
        XMLTag = string.Empty;
        DTOFullName = string.Empty;
    }

    public ClassData? BaseData { get; set; }
    public INamedTypeSymbol Symbol { get; }
    public bool IsEnum { get; private set; }
    public bool IsTallyComplexObject { get; }
    public string FullName { get => Symbol.GetClassMetaName(); }
    public string Name { get => Symbol.Name; }
    public string MetaName { get => $"{Name}Meta"; }
    public string DTOName { get; set; }
    public string Namespace { get => Symbol.ContainingNamespace.ToString(); }

    public bool GenerateITallyRequestableObject { get; set; }
    public Dictionary<string, ClassPropertyData> Members { get; } = [];

    public HashSet<string> DefaultTDLFunctions { get; internal set; } = [];
    public HashSet<string> DefaultTDLObjects { get; internal set; } = [];
    public HashSet<string> TDLFunctions { get; internal set; } = [];
    public string XMLTag { get; internal set; }
    public TDLCollectionData? TDLCollectionData { get; internal set; }
    public Dictionary<string, ClassPropertyData> AllDirectMembers { get; internal set; } = [];
    public Dictionary<string, ClassPropertyData> AllMembers { get; internal set; } = [];

    public Dictionary<string, UniqueMember> AllUniqueMembers { get; } = [];
    public bool IsBaseIRequestableObject { get; internal set; }
    public Dictionary<string, ClassPropertyData> OveriddenProperties { get; internal set; } = [];
    public bool IgnoreForGenerateDTO { get; internal set; }
    public string DTOFullName { get; internal set; }
    public INamedTypeSymbol? DTOSymbol { get; internal set; }
    public GenerationMode GenerationMode { get; internal set; }
    public string? DefaultTDLFiltersMethod { get; internal set; }

    public override string ToString()
    {
        return $"ClassData : {FullName}";
    }
}

public interface IClassAttributeTranfomable
{
    //  Dictionary<string, UniqueMember> AllUniqueMembers { get; }
    HashSet<string> DefaultTDLFunctions { get; }
    HashSet<string> DefaultTDLObjects { get; }
    HashSet<string> TDLFunctions { get; }
    string XMLTag { get; }
    TDLCollectionData? TDLCollectionData { get; }
    string Name { get; }
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
        IsTallyComplexObject = PropertyOriginalType.CheckInterface(TallyComplexObjectInterfaceName);
    }

    public ISymbol MemberSymbol { get; }
    public string Name { get; }
    public string UniqueName { get; }
    public INamedTypeSymbol PropertyOriginalType { get; }
    public bool IsComplex { get; }
    public bool IsList { get; private set; }
    public bool IsNullable { get; private set; }
    public bool IsEnum { get; private set; }
    public ClassData? ClassData { get; internal set; }
    public ClassData ParentData { get; internal set; }
    public bool IsOverridenProperty { get; }
    public bool IsTallyComplexObject { get; }
    public ClassPropertyData? OverridenProperty { get; }
    public PropertyTDLFieldData? TDLFieldData { get; internal set; }
    public List<XMLData> XMLData { get; internal set; } = [];
    public XMLData? DefaultXMLData { get; internal set; }
    public string? ListXMLTag { get; internal set; }
    public TDLCollectionData? TDLCollectionData { get; internal set; }
    public bool XmlIgnore { get; internal set; }
    public bool IsAttribute { get; internal set; }
    public bool IgnoreForDTO { get; internal set; }

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
    public string VariableName { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public bool IsNumber { get; set; }
}