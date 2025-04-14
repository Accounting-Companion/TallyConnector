using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
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
                var symbolData = new SymbolData(value.ParentSymbol, symbol, methodName, reqEnvelope) { ActivitySourceName = value.ActivitySourceName };
                symbolData.MethodNameSuffixPlural = value.HelperAttributeData?.MethodNameSuffixPlural ?? $"{methodName}s";
                symbolData.Args = value.HelperAttributeData?.Args ?? [];
                symbolData.ServiceFieldName = value.FieldName;
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
                try
                {
                    SymbolData value = symbol.Value;
                    var helper = new Helper(value);
                    if (value.GenerationMode is GenerationMode.All or GenerationMode.Get or GenerationMode.GetMultiple)
                    {
                        string source = helper.GetTDLReportCompilationUnit();
                        context.AddSource($"{symbol.Key}.{symbol.Value.MainSymbol.Name}.TDLReport.g.cs", source);
                        foreach (var diagnostic in helper.Diagnostics)
                        {
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                    if (value.GenerationMode is GenerationMode.All or GenerationMode.Post)
                    {
                        if (!value.IsEnum)
                        {
                            string source = helper.GetCreateDTOCompilationUnitString();
                            context.AddSource($"{symbol.Key}.{symbol.Value.MainSymbol.Name}.CreateDTO.g.cs", source);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            if (data.Count > 0)
            {
                string key = data.Keys.First();
                SymbolData symbolData = data[key];
                var name = symbolData.MainSymbol.Name;
                var nameSpace = symbolData.MainNameSpace;

                string src = GetReportHelper.GetReportResponseEnvelopeCompilationUnit(data, nameSpace, name);
                context.AddSource($"{symbolData.MainFullName}.ReportResponseEnvelope.g.cs", src);

                if (data.Values.Where(c => c.GenerationMode is GenerationMode.Post or GenerationMode.All).Any())
                {
                    string src2 = PostRequestEnvelopeHelper.GetPostRequestEnvelopeCompilationUnit(data, nameSpace, name);

                    context.AddSource($"{symbolData.MainFullName}.PostRequestEnvelope.g.cs", src2);

                    string src3 = PostObjectsHelper.GetPostObjectsCompilationUnit(data, nameSpace, name);

                    context.AddSource($"{symbolData.MainFullName}.PostObject.g.cs", src3);
                }
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

        if (baseType != null && baseType.SpecialType == SpecialType.None)
        {
            BaseSymbolData Basevalue = new(symbolData.MainSymbol, baseType, symbolData.ReqEnvelopeSymbol, baseType.Name, true, symbolData);
            symbolData.BaseSymbolData = Basevalue;
            if (!ComplexChildNames.Contains(Basevalue.FullName))
            {
                ComplexChildNames.Add(Basevalue.FullName);
                if (!_data.ContainsKey(Basevalue.FullName))
                {
                    SymbolData value = new(symbolData.MainSymbol, baseType, $"{symbolData.TypeName}Base{baseType.Name}", symbolData.ReqEnvelopeSymbol, true, symbolData)
                    {
                        ActivitySourceName = symbolData.ActivitySourceName,
                        GenerationMode = symbolData.GenerationMode,
                        IsBaseSymbol = true,
                        IsParentChild = symbolData.IsParentChild
                    };
                    _data.Add(value.FullName, value);
                    ProcessGetSymbol(value, _data, ComplexChildNames);
                    foreach (var item in value.TDLFunctionMethods)
                    {
                        symbolData.TDLFunctionMethods.Add(item.Value);
                    }
                    foreach (var item in value.TDLGetObjectMethods)
                    {
                        symbolData.TDLGetObjectMethods.Add(item.Value);
                    }
                    Basevalue.SymbolData = value;
                    symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                    symbolData.TDLLinesCount += value.TDLLinesCount + 1;
                    symbolData.TDLPartsCount += value.TDLPartsCount;

                }
                else
                {
                    var value = _data[Basevalue.FullName];
                    foreach (var item in value.TDLFunctionMethods)
                    {
                        symbolData.TDLFunctionMethods.Add(item.Value);
                    }
                    foreach (var item in value.TDLGetObjectMethods)
                    {
                        symbolData.TDLGetObjectMethods.Add(item.Value);
                    }
                    Basevalue.SymbolData = value;
                    symbolData.TDLLinesCount += value.TDLLinesCount + 1;
                    symbolData.BaseSymbolData = Basevalue;
                    symbolData.TDLPartsCount += value.TDLPartsCount;
                    symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                }
            }
            else
            {
                var value = _data[Basevalue.FullName];
                Basevalue.SymbolData = value;
                symbolData.BaseSymbolData = Basevalue;
                //symbolData.TDLLinesCount += Basevalue.TDLLinesCount + 1;
                symbolData.BaseSymbolData.Exclude = true;
                //symbolData.TDLPartsCount += Basevalue.TDLPartsCount;

                //symbolData.SimpleFieldsCount += childSymbolData.SimpleFieldsCount;
            }
            Basevalue.IsParentChild = symbolData.IsParentChild;
        }
        symbolData.TDLCollectionDetails ??= symbolData.BaseSymbolData?.SymbolData.TDLCollectionDetails;
        foreach (var childSymbol in childSymbols)
        {
            if (childSymbol.Name == "OtherAttributes" || childSymbol.Name == "OtherFields")
            {
                continue;
            }
            if (childSymbol.DeclaredAccessibility is Accessibility.Public && childSymbol.Kind is SymbolKind.Property || symbolData.Symbol.TypeKind is TypeKind.Enum && childSymbol.Kind is SymbolKind.Field)
            {
                var propertySymbol = childSymbol;
                //var json = JsonSerializer.Serialize(childSymbol);
                ChildSymbolData childData = new(propertySymbol, symbolData);
                if (childData.Attributes.Any(c => c.HasFullyQualifiedMetadataName(XmlIgnoreAttributeName)))
                {
                    continue;
                }
                ;

                symbolData.Children.Add(childData.Name, childData);
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
                                SymbolData value = new(symbolData.MainSymbol, childData.ChildType, $"{symbolData.TypeName}Child{childData.ChildType.Name}", symbolData.ReqEnvelopeSymbol, true) { ActivitySourceName = symbolData.ActivitySourceName };
                                _data.Add(childData.ChildTypeFullName, value);
                                ProcessGetSymbol(value, _data, ComplexChildNames);
                                childData.SymbolData = value;

                            }
                            else
                            {
                                var value = _data[childData.ChildTypeFullName];
                                childData.SymbolData = value;

                                //childData.Exclude = true;
                            }
                            ParseAttributes(childData);

                        }
                        else
                        {
                            var value = _data[childData.ChildTypeFullName];
                            childData.SymbolData = value;
                            childData.Exclude = true;
                        }
                        continue;
                    }
                    // If complex property then we will add explode so we require additional Part and Line
                    symbolData.TDLPartsCount++;
                    symbolData.TDLLinesCount++;
                    if (!ComplexChildNames.Contains(childData.ChildTypeFullName))
                    {
                        ComplexChildNames.Add(childData.ChildTypeFullName);
                        if (!_data.ContainsKey(childData.ChildTypeFullName))
                        {
                            SymbolData value = new(symbolData.MainSymbol, childData.ChildType, $"{symbolData.TypeName}Child{childData.ChildType.Name}", symbolData.ReqEnvelopeSymbol, true, symbolData);
                            value.ActivitySourceName = symbolData.ActivitySourceName;
                            value.IsParentChild = true;
                            value.GenerationMode = symbolData.GenerationMode;
                            _data.Add(childData.ChildTypeFullName, value);
                            ProcessGetSymbol(value, _data, ComplexChildNames);
                            childData.SymbolData = value;
                        }
                        else
                        {
                            var value = _data[childData.ChildTypeFullName];
                            childData.SymbolData = value;
                            symbolData.TDLPartsCount += value.TDLPartsCount;
                        }
                        var childSymbolData = childData.SymbolData;
                        symbolData.SimpleFieldsCount += childSymbolData.SimpleFieldsCount;
                        symbolData.TDLLinesCount += childSymbolData.TDLLinesCount + 1;
                        symbolData.TDLPartsCount += childSymbolData.TDLPartsCount;
                        foreach (var item in childSymbolData.TDLFunctionMethods)
                        {
                            symbolData.TDLFunctionMethods.Add(item.Value);
                        }
                        foreach (var item in childSymbolData.TDLGetObjectMethods)
                        {
                            symbolData.TDLGetObjectMethods.Add(item.Value);
                        }
                    }
                    else
                    {
                        var childSymbolData = _data[childData.ChildTypeFullName];
                        childData.SymbolData = childSymbolData;
                        //symbolData.TDLLinesCount += 1;
                        childData.Exclude = true;
                    }
                }
                else
                {
                    if (childData.IsList)
                    {
                        symbolData.TDLPartsCount++;
                        symbolData.TDLLinesCount++;
                    }
                    symbolData.SimpleFieldsCount++;
                }
                ParseAttributes(childData);
                var childs = childData.XMLData.Where(c => c.Symbol != null).ToList();
                int Counter = 1;
                foreach (var child in childs)
                {
                    ChildSymbolData xmlchildData = new(child.Symbol!, symbolData);
                    xmlchildData.ReportVarName = $"{childData.ReportVarName}{Counter}";
                    child.ChildSymbolData = xmlchildData;
                    var fullName = child.Symbol!.OriginalDefinition.ToString();
                    symbolData.TDLPartsCount++;
                    symbolData.TDLLinesCount++;
                    if (!ComplexChildNames.Contains(fullName))
                    {
                        if (!_data.ContainsKey(fullName))
                        {
                            SymbolData value = new(symbolData.MainSymbol, xmlchildData.ChildType, $"{symbolData.TypeName}Child{xmlchildData.ChildType.Name}", symbolData.ReqEnvelopeSymbol, true, symbolData);
                            value.ActivitySourceName = symbolData.ActivitySourceName;
                            value.GenerationMode = symbolData.GenerationMode;
                            value.IsParentChild = true;
                            xmlchildData.SymbolData = value;
                            _data.Add(fullName, value);
                            ProcessGetSymbol(value, _data, ComplexChildNames);
                        }
                        else
                        {
                            var value = _data[fullName];
                            xmlchildData.SymbolData = value;
                        }
                        var childSymbolData = xmlchildData.SymbolData;
                        symbolData.SimpleFieldsCount += childSymbolData.SimpleFieldsCount;
                        symbolData.TDLLinesCount += childSymbolData.TDLLinesCount + 1;
                        symbolData.TDLPartsCount += childSymbolData.TDLPartsCount;
                        foreach (var item in childSymbolData.TDLFunctionMethods)
                        {
                            symbolData.TDLFunctionMethods.Add(item.Value);
                        }
                        foreach (var item in childSymbolData.TDLGetObjectMethods)
                        {
                            symbolData.TDLGetObjectMethods.Add(item.Value);
                        }
                    }
                    else
                    {
                        var value = _data[fullName];
                        xmlchildData.SymbolData = value;
                        symbolData.TDLLinesCount += value.TDLLinesCount;
                        xmlchildData.Exclude = true;
                    }

                    xmlchildData.DefaultTDLCollectionDetails = xmlchildData.SymbolData.TDLCollectionDetails;
                    xmlchildData.XmlTag = child.XmlTag ?? xmlchildData.SymbolData.Name.ToUpper();
                    Counter++;
                }


            }

        }

    }
    // =>Group:BaseGroup=>TallyObject
    private void ParseAttributes(ChildSymbolData childData)
    {
        List<XMLData> xMLData = [];
        List<TDLCollectionData> tDLCollectionDetails = [];
        try
        {
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
                    XMLData? tXMLData = ParseXMLData(attributeDataAttribute);
                    if (tXMLData != null)
                    {
                        xMLData.Add(tXMLData);
                    }
                }
                if (attributeName == XMLAttributeAttributeName)
                {
                    XMLData? tXMLData = ParseXMLAttributeData(attributeDataAttribute);
                    if (tXMLData != null)
                    {
                        tXMLData.IsAttribute = true;
                        childData.IsAttribute = true;
                        xMLData.Add(tXMLData);
                    }
                }
                //if (attributeName == XMLEnumAttributeName && childData.Parent.IsEnum)
                //{
                //    xMLData ??= ParseEnumXMLData(attributeDataAttribute);

                //}
                if (attributeName == EnumChoiceAttributeName && childData.Parent.IsEnum)
                {
                    var txMLData = xMLData.FirstOrDefault();
                    if (txMLData == null)
                    {
                        xMLData.Add(txMLData = new());
                    }
                    var choice = ParseEnumChoiceXMLData(attributeDataAttribute);
                    if (choice != null && !string.IsNullOrEmpty(choice.Choice))
                    {
                        txMLData.EnumChoices.Add(choice);
                    }
                    //xMLData.Add((txMLData));

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
                            xMLData.Add(listItemXmlTag);
                        }
                    }
                }
                if (attributeName == TDLCollectionAttributeName)
                {

                    TDLCollectionData? tDLCollectionData = ParseTDLCollectionData(attributeDataAttribute);
                    if (tDLCollectionData != null)
                    {
                        tDLCollectionDetails.Add(tDLCollectionData);

                    }
                    childData.DefaultTDLCollectionDetails = tDLCollectionData;
                }


            }

            //if (childData.IsComplex && childData.SymbolData != null)
            //{
            //    foreach (var attributeDataAttribute in childData.SymbolData.Attributes)
            //    {
            //        //if (attributeDataAttribute.GetAttrubuteMetaName() == TDLCollectionAttributeName)
            //        //{
            //        //    childData.DefaultTDLCollectionDetails ??= ParseTDLCollectionData(attributeDataAttribute);
            //        //}
            //    }
            //}
            string upperName = childData.Name.ToUpper();
            var sameTypeXMLData = xMLData.Where(x => x.Symbol == null || (x.Symbol?.HasFullyQualifiedMetadataName(childData.ChildType.OriginalDefinition.ToString()) ?? false)).FirstOrDefault();
            xMLData.Remove(sameTypeXMLData);
            childData.XMLData = xMLData;
            childData.TDLFieldDetails ??= new()
            {
                Set = $"${sameTypeXMLData?.XmlTag ?? upperName}",
                ExcludeInFetch = false
            };
            if (childData.TDLFieldDetails.Set == null)
            {
                if (childData.Parent.IsTallyComplexObject)
                {
                    childData.TDLFieldDetails.Set = "{0}";

                }
                else
                {
                    childData.TDLFieldDetails.Set = $"${sameTypeXMLData?.XmlTag ?? upperName}";

                }
            }
            childData.TDLFieldDetails.FetchText ??= sameTypeXMLData?.XmlTag ?? upperName;
            childData.XmlTag = sameTypeXMLData?.XmlTag ?? upperName;
            if (!childData.IsComplex)
            {
                switch (childData.ChildType.SpecialType)
                {
                    case SpecialType.System_Boolean:
                        childData.TDLFieldDetails.Set = $"$${GetBooleanFromLogicFieldFunctionName}:{childData.TDLFieldDetails.Set}";
                        childData.TDLFieldDetails.Invisible ??= "$$ISEmpty:$$value";
                        break;
                    case SpecialType.System_DateTime:
                        ApplyDateProperties(childData);
                        break;
                    case SpecialType.System_Enum:
                        childData.TDLFieldDetails.Set = $"{string.Format(GetTDLFunctionsMethodName, childData.SymbolData!.Name)}:{childData.TDLFieldDetails.Set}";

                        break;
                    case SpecialType.System_Decimal or SpecialType.System_Int32 or SpecialType.System_Int64:
                        childData.TDLFieldDetails.Invisible ??= "$$ISEmpty:$$value";
                        break;
                }
                if (childData.IsEnum)
                {
                    childData.EnumChoices = sameTypeXMLData?.EnumChoices;
                    childData.TDLFieldDetails.Invisible ??= "$$ISEmpty:$$value";
                    childData.TDLFieldDetails.Set = $"$${string.Format(GetEnumFunctionName, childData.SymbolData!.Name)}:{childData.TDLFieldDetails.Set}";
                }
                if (childData.ChildTypeFullName == DateOnlyType)
                {
                    ApplyDateProperties(childData);
                }
                static void ApplyDateProperties(ChildSymbolData childData)
                {
                    childData.TDLFieldDetails!.Set = $"$${GetTransformDateToXSDFunctionName}:{childData.TDLFieldDetails.Set}";
                    childData.TDLFieldDetails.Invisible ??= "$$ISEmpty:$$value";
                }
            }
            if (childData.IsNullable)
            {
                childData.TDLFieldDetails.Invisible ??= "$$ISEmpty:$$value";

            }
            childData.DefaultTDLCollectionDetails ??= childData.SymbolData?.TDLCollectionDetails;


        }
        catch (Exception)
        {

            throw;
        }


    }

    private EnumChoiceData? ParseEnumChoiceXMLData(AttributeData attributeDataAttribute)
    {
        EnumChoiceData enumChoiceData;
        string choice = string.Empty;
        if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeDataAttribute.ConstructorArguments;
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        choice = constructorArguments.First().Value?.ToString();
                        break;
                    default:
                        break;
                }
            }
        }

        string[] versions = [];
        if (attributeDataAttribute.NamedArguments != null && attributeDataAttribute.NamedArguments.Length > 0)
        {
            var namedArguments = attributeDataAttribute.NamedArguments;
            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "Choice":
                        choice = (string)namedArgument.Value.Value!;
                        break;
                    case "Versions":
                        versions = [.. namedArgument.Value.Values.Select(c => (string)c.Value!)];
                        break;
                }
            }

        }
        return new EnumChoiceData(choice, versions);
    }

    private void ParseClassAttributes(SymbolData symbolData)
    {
        XMLData? xmlData = null;
        foreach (AttributeData attributeDataAttribute in symbolData.Attributes)
        {
            string attrName = attributeDataAttribute.GetAttrubuteMetaName();
            switch (attrName)
            {
                case XmlRootAttributeName:
                    xmlData = ParseRootXMLData(attributeDataAttribute);
                    break;
                case MaptoDTOAttributeName:
                    symbolData.MapToData = ParseMapToData(attributeDataAttribute);
                    break;
                case TDLCollectionAttributeName:
                    symbolData.TDLCollectionDetails = ParseTDLCollectionData(attributeDataAttribute);
                    break;
                case TDLFunctionsMethodNameAttributeName:
                    addToFunctionList(symbolData,
                        attributeDataAttribute,
                        symbolData.TDLFunctionMethods,
                        symbolData.ParentSymbol?.TDLFunctionMethods);
                    break;
                case TDLNamesetMethodNameAttributeName:
                    addToFunctionList(symbolData,
                            attributeDataAttribute,
                            symbolData.TDLNameSetMethods,
                            symbolData.ParentSymbol?.TDLNameSetMethods);
                    break;
                case TDLObjectsMethodNameAttributeName:
                    addToFunctionList(symbolData,
                            attributeDataAttribute,
                            symbolData.TDLGetObjectMethods,
                            symbolData.ParentSymbol?.TDLGetObjectMethods);
                    break;
                case TDLDefaultFiltersMethodNameAttributeName:
                    addToFunctionList(symbolData,
                            attributeDataAttribute,
                            symbolData.DefaultFilterMethods,
                            symbolData.ParentSymbol?.DefaultFilterMethods);
                    break;

                default:
                    continue;
            }
        }
        
        symbolData.RootXmlTag = xmlData?.XmlTag ?? symbolData.Name.ToUpper();

        void addToFunctionList(SymbolData symbolData,
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
                    case "Type":
                        xMLData.Symbol = (INamedTypeSymbol)namedArgument.Value.Value!;
                        break;
                }
            }

        }
        return xMLData;
    }
    private XMLData? ParseXMLAttributeData(AttributeData attributeDataAttribute)
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
                    case "AttributeName":
                        xMLData.XmlTag = (string)namedArgument.Value.Value!;
                        break;
                    case "Type":
                        xMLData.Symbol = (INamedTypeSymbol)namedArgument.Value.Value!;
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
                    case "TypeInfo":
                        tdlCollectionData.Symbol = (INamedTypeSymbol)namedArgument.Value.Value!;
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
            xMLData ??= new();
            var constructorArguments = attributeDataAttribute.ConstructorArguments;
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        xMLData.XmlTag = (string)constructorArguments[i].Value!;
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
                fieldData.ExcludeInFetch = (bool)constructorArguments.Skip(1).First().Value!;
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
                    case "ExcludeInFetch":
                        fieldData.ExcludeInFetch = (bool?)namedArgument.Value.Value ?? false;
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

internal class XMLData
{
    public string? XmlTag { get; set; }

    public List<EnumChoiceData> EnumChoices { get; set; } = [];

    public INamedTypeSymbol? Symbol { get; set; }

    public ChildSymbolData ChildSymbolData { get; set; }
    public bool IsAttribute { get; set; }
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
    public INamedTypeSymbol? Symbol { get; internal set; }
}
public class EnumChoiceData
{
    public EnumChoiceData(string choice, string[]? versions = null)
    {
        Choice = choice;
        Versions = versions ?? [];
    }

    public string Choice { get; }

    public string[] Versions { get; }
}

