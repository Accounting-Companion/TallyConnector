﻿using System.Collections.Immutable;
using System.Xml.Linq;
using TallyConnector.TDLReportSourceGenerator;
using TallyConnector.TDLReportSourceGenerator.Extensions.Symbols;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Execute;
internal class GenerateTDLReportsCommand
{
    private readonly ImmutableArray<INamedTypeSymbol> _symbols;
    private readonly List<Dictionary<string, SymbolData>> _dataList = [];
    private readonly List<Dictionary<string, GenerateSymbolsArgs>> _symbolArgs;
    private readonly ProjectArgs _projectArgs;

    public GenerateTDLReportsCommand(List<Dictionary<string, GenerateSymbolsArgs>> symbolArgs, ProjectArgs projectArgs)
    {
        _symbolArgs = symbolArgs;
        _projectArgs = projectArgs;
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
                INamedTypeSymbol reqEnvelope = value.RequestEnvelope;
                string methodName = value.HelperAttributeData?.MethodNameSuffix ?? item.Key;
                var symbolData = new SymbolData(value.ParentSymbol, symbol, methodName, reqEnvelope);
                symbolData.MethodNameSuffixPlural = value.HelperAttributeData?.MethodNameSuffixPlural ?? $"{methodName}s";
                symbolData.Args = value.HelperAttributeData?.Args ?? [];
                symbolData.GenerationMode = value.HelperAttributeData?.GenerationMode switch
                {
                    1 => GenerationMode.Get,
                    2 => GenerationMode.GetMultiple,
                    3 => GenerationMode.Post,
                    _ => GenerationMode.All,
                };
                if (!_data.ContainsKey(symbolData.FullName))
                {
                    _data.Add(symbolData.FullName, symbolData);
                    ProcessGetSymbol(symbolData, _data);
                }
            }
        }
        foreach (var data in _dataList)
        {
            foreach (var symbol in data)
            {
                SymbolData value = symbol.Value;
                var helper = new Helper(value);
                if (value.GenerationMode is GenerationMode.All or GenerationMode.Get or GenerationMode.GetMultiple)
                {
                    context.AddSource($"{symbol.Key}.{symbol.Value.MainSymbol.Name}.TDLReport.g.cs", helper.GetTDLReportCompilationUnit());
                }
                if (value.GenerationMode is GenerationMode.All or GenerationMode.Post)
                {
                    if (!value.IsEnum)
                    {
                        context.AddSource($"{symbol.Key}.{symbol.Value.MainSymbol.Name}.CreateDTO.g.cs", helper.GetCreateDTOCompilationUnitString());
                    }
                }
            }
            if (data.Count > 0)
            {
                string key = data.Keys.First();
                SymbolData symbolData = data[key];
                var name = symbolData.MainSymbol.Name;
                var nameSpace = symbolData.MainNameSpace;

                string src = GetReportHelper.GetReportResponseEnvelopeCompilationUnit(data, nameSpace, name);
                context.AddSource($"{name}.ReportResponseEnvelope.g.cs", src);
            }
        }

    }

    private object ObjtoPost(SymbolData source)
    {
        return new { source.Name };
    }

    void ProcessGetSymbol(SymbolData symbolData, Dictionary<string, SymbolData> _data, HashSet<string>? complexChildNames = null)
    {
        ParseClassAttributes(symbolData);
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
                    if (childData.IsEnum && !childData.Parent.IsEnum)
                    {
                        symbolData.SimpleFieldsCount++;
                        if (!ComplexChildNames.Contains(childData.ChildTypeFullName))
                        {
                            ComplexChildNames.Add(childData.ChildTypeFullName);
                            if (!_data.ContainsKey(childData.ChildTypeFullName))
                            {
                                SymbolData value = new(symbolData.MainSymbol, childData.ChildType, $"{symbolData.TypeName}Child{childData.ChildType.Name}", symbolData.ReqEnvelopeSymbol, true);
                                _data.Add(childData.ChildTypeFullName, value);
                                ProcessGetSymbol(value, _data, ComplexChildNames);
                                childData.SymbolData = value;
                                ParseAttributes(childData);
                            }
                            else
                            {
                                var value = _data[childData.ChildTypeFullName];
                                childData.SymbolData = value;
                                childData.Exclude = true;
                            }
                        }
                        else
                        {
                            var value = _data[childData.ChildTypeFullName];
                            childData.SymbolData = value;
                            childData.Exclude = true;
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
                            SymbolData value = new(symbolData.MainSymbol, childData.ChildType, $"{symbolData.TypeName}Child{childData.ChildType.Name}", symbolData.ReqEnvelopeSymbol, true, symbolData);
                            value.GenerationMode = symbolData.GenerationMode;
                            _data.Add(childData.ChildTypeFullName, value);
                            ProcessGetSymbol(value, _data, ComplexChildNames);
                            symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                            symbolData.ComplexFieldsIncludedCount += value.ComplexFieldsIncludedCount + 1;
                            symbolData.ComplexFieldsCount += value.ComplexFieldsCount;
                            childData.SymbolData = value;
                        }
                        else
                        {
                            var value = _data[childData.ChildTypeFullName];
                            childData.SymbolData = value;
                            //symbolData.SimpleFieldsCount += Basevalue.SimpleFieldsCount;
                            symbolData.ComplexFieldsIncludedCount += value.ComplexFieldsIncludedCount + 1;
                            symbolData.ComplexFieldsCount += 1;
                        }
                    }
                    else
                    {
                        var value = _data[childData.ChildTypeFullName];
                        childData.SymbolData = value;
                        //symbolData.SimpleFieldsCount += Basevalue.SimpleFieldsCount;
                        symbolData.ComplexFieldsIncludedCount += value.ComplexFieldsIncludedCount + 1;
                        //symbolData.ComplexFieldsCount += 1;
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
            BaseSymbolData Basevalue = new(symbolData.MainSymbol, baseType, symbolData.ReqEnvelopeSymbol, baseType.Name, true);
            symbolData.BaseSymbolData = Basevalue;
            if (!ComplexChildNames.Contains(Basevalue.FullName))
            {
                ComplexChildNames.Add(Basevalue.FullName);
                if (!_data.ContainsKey(Basevalue.FullName))
                {
                    SymbolData value = new(symbolData.MainSymbol, baseType, $"{symbolData.TypeName}Base{baseType.Name}", symbolData.ReqEnvelopeSymbol, true);
                    value.GenerationMode = symbolData.GenerationMode;
                    value.IsBaseSymbol = true;
                    _data.Add(value.FullName, value);
                    ProcessGetSymbol(value, _data, ComplexChildNames);
                    Basevalue.SymbolData = value;
                    symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                    symbolData.ComplexFieldsIncludedCount += value.ComplexFieldsIncludedCount + 1;
                    symbolData.ComplexFieldsCount += value.ComplexFieldsCount;

                }
                else
                {
                    var value = _data[Basevalue.FullName];
                    Basevalue.SymbolData = value;
                    symbolData.ComplexFieldsIncludedCount += value.ComplexFieldsIncludedCount + 1;
                    symbolData.BaseSymbolData = Basevalue;
                    symbolData.ComplexFieldsCount += value.ComplexFieldsCount;
                    symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                }
            }
            else
            {
                var value = _data[Basevalue.FullName];
                Basevalue.SymbolData = value;
                symbolData.BaseSymbolData = Basevalue;
                //symbolData.ComplexFieldsIncludedCount += Basevalue.ComplexFieldsIncludedCount + 1;
                symbolData.BaseSymbolData.Exclude = true;
                //symbolData.ComplexFieldsCount += Basevalue.ComplexFieldsCount;
            }

        }
    }
    private void ParseAttributes(ChildSymbolData childData)
    {
        XMLData? xMLData = null;
        foreach (AttributeData attributeDataAttribute in childData.Attributes)
        {
            string attributeName = attributeDataAttribute.GetAttrubuteMetaName();

            if (attributeName == IgnoreForCreateDTOAttributeName)
            {
                childData.IgnoreForCreateDTO = true;
            }
            if (attributeName == TDLFieldAttributeName)
            {
                childData.TDLFieldDetails = ParseTDLFieldDetails(attributeDataAttribute);
            }
            if (attributeName == XMLElementAttributeName && !childData.Parent.IsEnum)
            {
                xMLData ??= ParseXMLData(attributeDataAttribute);
            }
            if (attributeName == XMLEnumAttributeName && childData.Parent.IsEnum)
            {
                xMLData ??= ParseEnumXMLData(attributeDataAttribute);
            }
            if (attributeName == EnumChoiceAttributeName && childData.Parent.IsEnum)
            {
                xMLData ??= new();
                var choice = ParseEnumChoiceXMLData(attributeDataAttribute);
                if (choice !=null && !string.IsNullOrEmpty(choice))
                {
                    xMLData.EnumChoices.Add(choice);
                }
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
                childData.TDLCollectionDetails = ParseTDLCollectionData(attributeDataAttribute);
            }

        }

        if (childData.IsComplex && childData.SymbolData != null)
        {
            foreach (var attributeDataAttribute in childData.SymbolData.Attributes)
            {
                //if (attributeDataAttribute.GetAttrubuteMetaName() == TDLCollectionAttributeName)
                //{
                //    childData.TDLCollectionDetails ??= ParseTDLCollectionData(attributeDataAttribute);
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
        childData.TDLFieldDetails.FetchText ??= xMLData?.XmlTag ?? upperName;
        childData.XmlTag = xMLData?.XmlTag ?? upperName;
        if (!childData.IsComplex)
        {
            switch (childData.ChildType.SpecialType)
            {
                case SpecialType.System_Boolean:
                    childData.TDLFieldDetails.Set = $"$${GetBooleanFromLogicFieldFunctionName}:{childData.TDLFieldDetails.Set}";
                    childData.TDLFieldDetails.Invisible ??= "$$ISEmpty:$$value";
                    break;
                case SpecialType.System_DateTime:
                    childData.TDLFieldDetails.Set = $"$${GetTransformDateToXSDFunctionName}:{childData.TDLFieldDetails.Set}";
                    break;
                case SpecialType.System_Enum:
                    childData.TDLFieldDetails.Set = $"{string.Format(GetTDLFunctionsMethodName, childData.SymbolData!.Name)}:{childData.TDLFieldDetails.Set}";
                    
                    break;
            }
            if (childData.IsEnum)
            {
                childData.EnumChoices = xMLData?.EnumChoices;
                childData.TDLFieldDetails.Invisible ??= "$$ISEmpty:$$value";
                childData.TDLFieldDetails.Set = $"$${string.Format(GetEnumFunctionName, childData.SymbolData!.Name)}:{childData.TDLFieldDetails.Set}";
            }
        }
        childData.TDLCollectionDetails ??= childData.SymbolData?.TDLCollectionDetails;

    }

    private string? ParseEnumChoiceXMLData(AttributeData attributeDataAttribute)
    {
        if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeDataAttribute.ConstructorArguments;
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        return constructorArguments.First().Value?.ToString();
                    default:
                        break;
                }
            }
        }
        if (attributeDataAttribute.NamedArguments != null && attributeDataAttribute.NamedArguments.Length > 0)
        {
            var namedArguments = attributeDataAttribute.NamedArguments;
            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "Choice":
                        return(string)namedArgument.Value.Value!;
                }
            }

        }
        return null;
    }

    private void ParseClassAttributes(SymbolData symbolData)
    {
        XMLData? xmlData = null;
        foreach (AttributeData attributeDataAttribute in symbolData.Attributes)
        {
            string attrName = attributeDataAttribute.GetAttrubuteMetaName();
            if (attrName == XmlRootAttributeName)
            {
                xmlData = ParseRootXMLData(attributeDataAttribute);
            }
            if (attrName == MaptoDTOAttributeName)
            {
                symbolData.MapToData = ParseMapToData(attributeDataAttribute);
            }
            if (attrName == TDLCollectionAttributeName)
            {
                symbolData.TDLCollectionDetails = ParseTDLCollectionData(attributeDataAttribute);
            }
            if (attrName == TDLFunctionsMethodNameAttributeName)
            {
                addFunction(symbolData,
                            attributeDataAttribute,
                            symbolData.TDLFunctionMethods,
                            symbolData.ParentSymbol!.TDLFunctionMethods);

            }
            if (attrName == TDLNamesetMethodNameAttributeName)
            {
                addFunction(symbolData,
                            attributeDataAttribute,
                            symbolData.TDLNameSetMethods,
                            symbolData.ParentSymbol!.TDLNameSetMethods);
            }
        }
        symbolData.RootXmlTag = xmlData?.XmlTag ?? symbolData.Name.ToUpper();

        void addFunction(SymbolData symbolData,
                         AttributeData attributeDataAttribute,
                         FunctionDetails tDLFunctionMethods,
                         FunctionDetails? parentTDLFunctionMethods = null)
        {
            string? funcName = ParseTDLFunctionName(attributeDataAttribute);
            if (funcName != null)
            {
                ImmutableArray<ISymbol> symbols = symbolData.Symbol.GetMembers(funcName);
                IMethodSymbol? symbol = symbols.OfType<IMethodSymbol>().Where(c => CheckReturnType(c.ReturnType)).FirstOrDefault();
                if (symbol != null)
                {
                    tDLFunctionMethods.Add(symbol, symbolData);
                    parentTDLFunctionMethods?.Add(symbol, symbolData);
                }

            }
        }
    }

    private MapToData? ParseMapToData(AttributeData attributeDataAttribute)
    {
        if (attributeDataAttribute.AttributeClass!.TypeArguments.Length == 1)
        {
            return new MapToData() { TypeSymbol = (INamedTypeSymbol)attributeDataAttribute.AttributeClass.TypeArguments.First() };
        }
        return null;
    }

    private bool CheckReturnType(ITypeSymbol returnType)
    {
        return true;
    }

    private string? ParseTDLFunctionName(AttributeData attributeDataAttribute)
    {
        if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeDataAttribute.ConstructorArguments;
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        string? value = (string?)constructorArguments.First().Value;
                        return value;

                    default:
                        break;
                }
            }
        }
        if (attributeDataAttribute.NamedArguments != null && attributeDataAttribute.NamedArguments.Length > 0)
        {
            var namedArguments = attributeDataAttribute.NamedArguments;

            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "FunctionName":
                        return (string?)namedArgument.Value.Value;
                }
            }
        }
        return null;
    }

    private XMLData? ParseXMLData(AttributeData attributeDataAttribute)
    {
        XMLData? xMLData = null;
        if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeDataAttribute.ConstructorArguments;
            xMLData ??= new();
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:

                        xMLData.XmlTag = constructorArguments.First().Value?.ToString();
                        break;
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
    private XMLData? ParseEnumXMLData(AttributeData attributeDataAttribute)
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
                    case "Name":
                        xMLData.XmlTag = (string)namedArgument.Value.Value!;
                        break;
                }
            }

        }
        return xMLData;
    }
    private TDLCollectionData? ParseTDLCollectionData(AttributeData attributeDataAttribute)
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
                    case "ExplodeCondition":
                        tdlCollectionData.ExplodeCondition = (string)namedArgument.Value.Value!;
                        break;
                    case "Type":
                        tdlCollectionData.Type = (string)namedArgument.Value.Value!;
                        break;
                    case "Exclude":
                        tdlCollectionData.Exclude = (bool)namedArgument.Value.Value!;
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
                    case "Invisible":
                        fieldData.Invisible = (string?)namedArgument.Value.Value;
                        break;
                    case "FetchText":
                        fieldData.FetchText = (string?)namedArgument.Value.Value;
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

    public List<string> EnumChoices { get; set; } = [];
}
internal class MapToData
{
    public INamedTypeSymbol TypeSymbol { get; set; }
}
public class TDLCollectionData
{
    public string CollectionName { get; internal set; }
    public string? ExplodeCondition { get; internal set; }
    public string? Type { get; internal set; }
    public bool? Exclude { get; internal set; }
}


