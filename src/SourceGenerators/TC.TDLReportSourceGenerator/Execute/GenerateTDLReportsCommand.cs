using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using TC.TDLReportSourceGenerator.Models;

namespace TC.TDLReportSourceGenerator.Execute;
internal class GenerateTDLReportsCommand
{
    private readonly ImmutableArray<INamedTypeSymbol> _symbols;
    private readonly List<Dictionary<string, SymbolData>> _dataList = [];
    private readonly List<Dictionary<string, GenerateSymbolsArgs>> _symbolArgs;


    public GenerateTDLReportsCommand(List<Dictionary<string, GenerateSymbolsArgs>> symbolArgs)
    {
        _symbolArgs = symbolArgs;
    }

    public void Execute(SourceProductionContext context)
    {
        foreach (var args in _symbolArgs)
        {
            Dictionary<string, SymbolData> _data = [];
            _dataList.Add(_data);
            foreach (var item in args)
            {
                var value = item.Value;
                INamedTypeSymbol symbol = value.GetSymbol;
                var symbolData = new SymbolData(value.ParentSymbol, symbol, item.Key);
                if (!_data.ContainsKey(symbolData.FullName))
                {
                    _data.Add(symbolData.FullName, symbolData);
                    ProcessSymbol(symbolData, _data);
                }
            }
        }
        foreach (var data in _dataList)
        {
            foreach (var symbol in data)
            {
                var compilationUnitSyntax = new GenerateTDLReport(symbol.Value).GetCompilationUnit();
                var source = compilationUnitSyntax.ToFullString();
                context.AddSource($"{symbol.Key}.{symbol.Value.ParentSymbol.Name}.TDLReport.g.cs", source);
            }
        }

    }
    void ProcessSymbol(SymbolData symbolData, Dictionary<string, SymbolData> _data, HashSet<string>? complexChildNames = null)
    {
        ParseAttributes(symbolData);
        IEnumerable<ISymbol> childSymbols = symbolData.Symbol.GetMembers();
        INamedTypeSymbol? baseType = symbolData.Symbol.BaseType;
        var ComplexChildNames = complexChildNames ?? [];
        foreach (var childSymbol in childSymbols)
        {
            if (childSymbol.Name == "OtherAttributes" || childSymbol.Name == "OtherFields")
            {
                continue;
            }
            if (childSymbol.DeclaredAccessibility is Accessibility.Public && childSymbol.Kind is SymbolKind.Property || symbolData.Symbol.TypeKind is TypeKind.Enum && childSymbol.Kind is SymbolKind.Field)
            {
                var propertySymbol = childSymbol;
                ChildSymbolData childData = new(propertySymbol, symbolData);

                symbolData.Children.Add(childData);
                if (childData.IsComplex || childData.IsEnum)
                {
                    if (childData.IsEnum)
                    {
                        if (!ComplexChildNames.Contains(childData.ChildTypeFullName))
                        {
                            ComplexChildNames.Add(childData.ChildTypeFullName);
                            if (!_data.ContainsKey(childData.ChildTypeFullName))
                            {
                                SymbolData value = new(symbolData.ParentSymbol, childData.ChildType, childData.ChildType.Name, true);
                                _data.Add(childData.ChildTypeFullName, value);
                                ProcessSymbol(value, _data, ComplexChildNames);
                                ParseAttributes(childData);
                            }
                        }
                        continue;
                    }
                    symbolData.ComplexFieldsCount++;
                    if (!ComplexChildNames.Contains(childData.ChildTypeFullName))
                    {
                        symbolData.ComplexFieldsIncludedCount++;
                        ComplexChildNames.Add(childData.ChildTypeFullName);
                        if (!_data.ContainsKey(childData.ChildTypeFullName))
                        {
                            SymbolData value = new(symbolData.ParentSymbol, childData.ChildType, childData.ChildType.Name, true);
                            _data.Add(childData.ChildTypeFullName, value);
                            ProcessSymbol(value, _data, ComplexChildNames);
                            symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                            symbolData.ComplexFieldsIncludedCount += value.ComplexFieldsIncludedCount;
                            symbolData.ComplexFieldsCount += value.ComplexFieldsCount;
                            childData.SymbolData = value;
                        }
                        else
                        {
                            var value = _data[childData.ChildTypeFullName];
                            childData.SymbolData = value;
                            //symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                            symbolData.ComplexFieldsIncludedCount += value.ComplexFieldsIncludedCount;
                            symbolData.ComplexFieldsCount += value.ComplexFieldsCount;
                        }
                    }
                    else
                    {
                        var value = _data[childData.ChildTypeFullName];
                        childData.SymbolData = value;
                        //symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                        symbolData.ComplexFieldsIncludedCount += value.ComplexFieldsIncludedCount;
                        symbolData.ComplexFieldsCount += value.ComplexFieldsCount;
                        childData.Exclude = true;
                    }
                }
                else
                {
                    if (childData.IsList)
                    {
                        symbolData.ComplexFieldsCount++;
                        symbolData.ComplexFieldsIncludedCount++;
                    }
                    symbolData.SimpleFieldsCount++;
                }
                ParseAttributes(childData);
            }

        }
        if (baseType != null && baseType.SpecialType == SpecialType.None)
        {
            BaseSymbolData value = new(symbolData.ParentSymbol, baseType, baseType.Name, true);
            symbolData.BaseSymbolData = value;
            if (!ComplexChildNames.Contains(value.FullName))
            {
                //symbolData.ComplexFieldsCount++;
                if (!_data.ContainsKey(value.FullName))
                {
                    symbolData.ComplexFieldsIncludedCount++;
                    _data.Add(value.FullName, value);
                    ProcessSymbol(value, _data, ComplexChildNames);

                    symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                    symbolData.ComplexFieldsIncludedCount += value.ComplexFieldsIncludedCount;
                    symbolData.ComplexFieldsCount += value.ComplexFieldsCount;
                    symbolData.BaseSymbolData = value;
                }
            }
            else
            {
                value.Exclude = true;
            }
        }
    }
    private void ParseAttributes(ChildSymbolData childData)
    {
        XMLData? xMLData = null;
        foreach (AttributeData attributeDataAttribute in childData.Attributes)
        {
            string attributeName = attributeDataAttribute.GetAttrubuteMetaName();
            if (attributeName == TDLFieldAttributeName)
            {
                childData.TDLFieldDetails = ParseTDLFieldDetails(attributeDataAttribute);

            }
            if (attributeName == XMLElementAttributeName)
            {
                xMLData ??= ParseXMLData(attributeDataAttribute);
            }
            if (childData.IsList)
            {
                if (attributeName == XMLArrayAttributeName)
                {
                    var listxMLData = ParseXMLData(attributeDataAttribute);
                    childData.ListXmlTag = listxMLData?.XmlTag;
                }
                if (attributeName == XMLArrayItemAttributeName)
                {
                    var listItemXmlTag = ParseXMLData(attributeDataAttribute);
                    if (listItemXmlTag != null)
                    {
                        xMLData = listItemXmlTag;
                    }
                }
            }
            if (attributeName == TDLCollectionAttributeName)
            {
                childData.TDLCollectionDetails = ParseTDLCollectionDataFromProperty(attributeDataAttribute);
            }

        }

        if (childData.IsComplex && childData.SymbolData != null)
        {
            foreach (var attributeDataAttribute in childData.SymbolData.Attributes)
            {
                //if (attributeDataAttribute.GetAttrubuteMetaName() == TDLCollectionAttributeName)
                //{
                //    childData.TDLCollectionDetails ??= ParseTDLCollectionDataFromProperty(attributeDataAttribute);
                //}
            }
        }
        string upperName = childData.Name.ToUpper();

        childData.TDLFieldDetails ??= new()
        {
            Set = $"${xMLData?.XmlTag ?? upperName}",
            IncludeInFetch = false
        };
        if (childData.TDLFieldDetails.Set == null)
        {
            if (childData.Parent.IsTallyComplexObject)
            {
                childData.TDLFieldDetails.Set = "{0}";

            }
            else
            {
                childData.TDLFieldDetails.Set = $"${xMLData?.XmlTag ?? upperName}";
            }
        }

        childData.XmlTag = xMLData?.XmlTag ?? upperName;
        if (!childData.IsComplex)
        {
            switch (childData.ChildType.SpecialType)
            {
                case SpecialType.System_Boolean:
                    childData.TDLFieldDetails.Set = $"$${GetBooleanFromLogicFieldFunctionName}:{childData.TDLFieldDetails.Set}";
                    break;
                case SpecialType.System_Enum:
                    childData.TDLFieldDetails.Set = $"{childData.TDLFieldDetails.Set}";
                    break;
            }
        }
    }

    private void ParseAttributes(SymbolData symbolData)
    {
        XMLData? xmlData = null;
        foreach (AttributeData attributeDataAttribute in symbolData.Attributes)
        {
            if (attributeDataAttribute.GetAttrubuteMetaName() == XMLElementAttributeName)
            {
                xmlData = ParseRootXMLData(attributeDataAttribute);

            }

        }
        symbolData.RootXmlTag = xmlData?.XmlTag ?? symbolData.Name.ToUpper();
    }
    private XMLData? ParseXMLData(AttributeData attributeDataAttribute)
    {
        XMLData? xMLData = null;
        if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeDataAttribute.ConstructorArguments;
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:

                    default:
                        break;
                }
            }
        }
        if (attributeDataAttribute.NamedArguments != null && attributeDataAttribute.NamedArguments.Length > 0)
        {
            var namedArguments = attributeDataAttribute.NamedArguments;
            xMLData ??= new();
            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "ElementName":
                        xMLData.XmlTag = (string)namedArgument.Value.Value!;
                        break;
                }
            }

        }
        return xMLData;
    }
    private TDLCollectionData? ParseTDLCollectionDataFromProperty(AttributeData attributeDataAttribute)
    {
        TDLCollectionData? tdlCollectionData = null;
        if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
        {
            ImmutableArray<TypedConstant> constructorArguments = attributeDataAttribute.ConstructorArguments;
            tdlCollectionData = new();
            tdlCollectionData.CollectionName = (string)constructorArguments.First().Value!;
        }
        if (attributeDataAttribute.NamedArguments != null && attributeDataAttribute.NamedArguments.Length > 0)
        {
            ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = attributeDataAttribute.NamedArguments;

            tdlCollectionData ??= new();
            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "CollectionName":
                        tdlCollectionData.CollectionName = (string)namedArgument.Value.Value!;
                        break;
                }
            }
        }
        return tdlCollectionData;
    }
    private XMLData? ParseRootXMLData(AttributeData attributeDataAttribute)
    {
        XMLData? xMLData = null;
        if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeDataAttribute.ConstructorArguments;
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:
                    default:
                        break;
                }
            }
        }
        if (attributeDataAttribute.NamedArguments != null && attributeDataAttribute.NamedArguments.Length > 0)
        {
            var namedArguments = attributeDataAttribute.NamedArguments;
        }
        return xMLData;
    }
    private TDLFieldData? ParseTDLFieldDetails(AttributeData attributeDataAttribute)
    {
        TDLFieldData? fieldData = null;
        if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
        {
            ImmutableArray<TypedConstant> constructorArguments = attributeDataAttribute.ConstructorArguments;
            fieldData = new();
            fieldData.Set = (string)constructorArguments.First().Value!;
            if (constructorArguments.Length == 2)
            {
                fieldData.IncludeInFetch = (bool)constructorArguments.Skip(1).First().Value!;
            }

        }
        if (attributeDataAttribute.NamedArguments != null && attributeDataAttribute.NamedArguments.Length > 0)
        {
            ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = attributeDataAttribute.NamedArguments;

            fieldData ??= new();
            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "Set":
                        fieldData.Set = (string)namedArgument.Value.Value!;
                        break;
                    case "IncludeInFetch":
                        fieldData.IncludeInFetch = (bool?)namedArgument.Value.Value ?? false;
                        break;
                    case "Use":
                        fieldData.Use = (string?)namedArgument.Value.Value;
                        break;
                    case "TallyType":
                        fieldData.TallyType = (string?)namedArgument.Value.Value;
                        break;
                    case "Format":
                        fieldData.Format = (string?)namedArgument.Value.Value;
                        break;
                }
            }
        }

        return fieldData;
    }
}

public class XMLData
{
    public string? XmlTag { get; set; }
}
public class TDLCollectionData
{
    public string CollectionName { get; internal set; }
}