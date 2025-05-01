using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using TallyConnector.TDLReportSourceGenerator.Services;

namespace TallyConnector.TDLReportSourceGenerator.Models;
public class ModelData
{
    public ModelData(INamedTypeSymbol symbol)
    {
        Symbol = symbol;
        FullName = symbol.GetClassMetaName();
        Name = symbol.Name;
        Namespace = symbol.ContainingNamespace.ToString();
    }

    public BaseModelData? BaseData { get; set; }


    public Dictionary<string, PropertyData> Properties { get; set; } = [];
    public INamedTypeSymbol Symbol { get; }
    public string FullName { get; }
    public string Name { get; }
    public string Namespace { get; }
    public int SimplePropertiesCount { get; internal set; }
}
public class BaseModelData
{
    public BaseModelData(string fullName, string name)
    {
        FullName = fullName;
        Name = name;
    }

    public bool IsTallyRequestableObject { get; set; }

    public string FullName { get; set; }
    public string Name { get; private set; }
    public ModelData? ModelData { get; set; }


}
public class PropertyData
{
    public string? _fieldSuffix;
    public PropertyData(ISymbol member)
    {
        MemberSymbol = member;
        Name = member.Name;
        PropertyOriginalType = GetChildType();
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
public class PropertyTDLFieldData
{

}