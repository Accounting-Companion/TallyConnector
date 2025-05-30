
using TallyConnector.TDLReportSourceGenerator.Services;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Common;

namespace TallyConnector.TDLReportSourceGenerator.Models;
public class ModelData
{
    public ModelData(INamedTypeSymbol symbol)
    {
        Symbol = symbol;
        FullName = symbol.GetClassMetaName();
        Name = symbol.Name;
        Namespace = symbol.ContainingNamespace.ToString();
        IsRequestableObjectInterface = true;
        IsEnum = Symbol.TypeKind == TypeKind.Enum;
        IsTallyComplexObject = symbol.CheckInterface(TallyComplexObjectInterfaceName);
    }

    public BaseModelData? BaseData { get; set; }


    public Dictionary<string, PropertyData> Properties { get; set; } = [];
    public INamedTypeSymbol Symbol { get; }
    public string FullName { get; }
    public string Name { get; }
    public string Namespace { get; }
    public int SimplePropertiesCount { get; internal set; }
    /// <summary>
    /// this will be false we  fetch class not by attribute (TallyRequestableObjectInterface) but from base or complex properties
    /// </summary>
    public bool IsRequestableObjectInterface { get; set; }
    public bool IsEnum { get; }
    public bool IsTallyComplexObject { get; private set; }
    public int ComplexPropertiesCount { get; internal set; }
    public TDLCollectionData? TDLCollectionData { get; internal set; }
    public string? XMLTag { get; internal set; }
    public int ENumPropertiesCount { get; internal set; }
    public HashSet<string> DefaultTDLFunctions { get; internal set; } = [];
    public HashSet<string> TDLFunctions { get; internal set; } = [];
}
public class BaseModelData
{
    public BaseModelData(string fullName, string name)
    {
        FullName = fullName;
        Name = name;
    }
    public string FullName { get; set; }
    public string Name { get; private set; }
    public ModelData? ModelData { get; set; }
    public bool Exclude { get; internal set; }
}
public class PropertyData
{
    public string? _fieldSuffix;
    public PropertyData(ISymbol member, ModelData modelData)
    {
        MemberSymbol = member;
        Name = member.Name;
        ModelData = modelData;
        PropertyOriginalType = GetChildType();
        IsTallyComplexObject = PropertyOriginalType.CheckInterface(TallyComplexObjectInterfaceName);
        IsComplex = (PropertyOriginalType.SpecialType is SpecialType.None && !DefaultSimpleTypes.Contains(PropertyOriginalType.GetClassMetaName())) && PropertyOriginalType.TypeKind is not TypeKind.Enum;
    }
    public string Name { get; set; }
    public INamedTypeSymbol PropertyOriginalType { get; }
    public bool IsComplex { get; private set; }

    public string FieldName => _fieldSuffix == null ? Name : $"{Name}_{_fieldSuffix}";
    public bool IsOveridden { get; set; }
    public ISymbol MemberSymbol { get; }
    public bool IsEnum { get; private set; }
    public bool IsList { get; private set; }
    public bool IsNullable { get; private set; }

    public PropertyTDLFieldData? TDLFieldData { get; set; }
    public TDLCollectionData? TDLCollectionData { get; internal set; }
    public ModelData? OriginalModelData { get; internal set; }
    public bool Exclude { get; internal set; }
    public bool IsAttribute { get; internal set; }
    public PropertyData? OveriddenProperty { get; internal set; }
    public List<XMLData> XMLData { get; internal set; } = [];
    public XMLData? DefaultXMLData { get; internal set; }
    public string? ListXMLTag { get; internal set; }
    public string? CollectionPrefix { get; internal set; }
    public ModelData ModelData { get; }
    public bool IsTallyComplexObject { get; private set; }

    internal void SetFieldName(string parentClassName) => _fieldSuffix = Utils.GenerateUniqueNameSuffix($"{parentClassName}\0{Name}");



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




    public override string ToString()
    {
        return $"Property - {FieldName}";
    }
}

public class XMLData
{
    public string? XmlTag { get; set; }

    public List<Execute.EnumChoiceData> EnumChoices { get; set; } = [];

    public INamedTypeSymbol? Symbol { get; set; }

    public ModelData? ModelData { get; set; }
    public bool IsAttribute { get; set; }
    public bool Exclude { get; internal set; }
    public string? FieldName { get; internal set; }
    public string? CollectionPrefix { get; internal set; }
}

public class PropertyTDLFieldData
{
    public string Set { get; internal set; }
    public bool ExcludeInFetch { get; internal set; }
    public string? Use { get; internal set; }
    public string? TallyType { get; internal set; }
    public string? Format { get; internal set; }
    public string? Invisible { get; internal set; }
    public string? FetchText { get; internal set; }
}


public class PropertyHolder
{
    public PropertyHolder(IEnumerable<PropertyData> properties)
    {
        Properties = properties;
    }
    public PropertyHolder(IEnumerable<PropertyData> properties, PropertyData? parent)
    {
        Properties = properties;
        Parent = parent;
    }

    public PropertyData? Parent { get; set; }
    public IEnumerable<PropertyData> Properties { get; set; }
}