using System.Collections.Immutable;
using TC.TDLReportSourceGenerator.Models;

namespace TC.TDLReportSourceGenerator.Execute;
internal class GenerateTDLReportsCommand
{
    private readonly ImmutableArray<INamedTypeSymbol> _symbols;
    private readonly Dictionary<string, SymbolData> _data = [];

    public GenerateTDLReportsCommand(ImmutableArray<INamedTypeSymbol> symbols)
    {
        _symbols = symbols;
    }

    public void Execute(SourceProductionContext context)
    {
        foreach (var symbol in _symbols)
        {
            var symbolData = new SymbolData(symbol);
            if (!_data.ContainsKey(symbolData.FullName))
            {
                _data.Add(symbolData.FullName, symbolData);
                ProcessSymbol(symbolData);
                
            }
        }
        foreach (var Symbol in _data)
        {
            var compilationUnitSyntax = new GenerateTDLReport(Symbol.Value).GetCompilationUnit();
            var source = compilationUnitSyntax.ToFullString();
            context.AddSource($"{Symbol.Key}.TDLReport.g.cs", source);
        }

    }
    void ProcessSymbol(SymbolData symbolData, HashSet<string>? complexChildNames = null)
    {
        ParseAttributes(symbolData);
        IEnumerable<ISymbol> childSymbols = symbolData.Symbol.GetMembers();
        var ComplexChildNames = complexChildNames ?? [];
        foreach (var childSymbol in childSymbols)
        {
            if (childSymbol.DeclaredAccessibility is Accessibility.Public && childSymbol is IPropertySymbol propertySymbol)
            {
                ChildSymbolData childData = new(propertySymbol, symbolData);

                symbolData.Children.Add(childData);
                if (childData.IsComplex)
                {
                    symbolData.ComplexFieldsCount++;
                    if (!ComplexChildNames.Contains(childData.ChildTypeFullName))
                    {
                        ComplexChildNames.Add(childData.ChildTypeFullName);
                        if (!_data.ContainsKey(childData.ChildTypeFullName))
                        {
                            SymbolData value = new(childData.ChildType,true);
                            _data.Add(childData.ChildTypeFullName, value);
                            ProcessSymbol(value, ComplexChildNames);
                            symbolData.SimpleFieldsCount += value.SimpleFieldsCount;
                            symbolData.ComplexFieldsCount += value.ComplexFieldsCount;
                            childData.SymbolData = value;
                        }
                    }
                    else
                    {
                        childData.Exclude = true;
                    }
                }
                else
                {
                    ParseAttributes(childData);
                    symbolData.SimpleFieldsCount++;
                }
            }

        }
    }
    private void ParseAttributes(ChildSymbolData childData)
    {
        XMLData? xMLData = null;
        foreach (AttributeData attributeDataAttribute in childData.Attributes)
        {
            if (attributeDataAttribute.GetAttrubuteMetaName() == TDLXMLSetAttributeName)
            {
                childData.TDLFieldDetails = ParseTDLFieldDetails(attributeDataAttribute);

            }
            if (attributeDataAttribute.GetAttrubuteMetaName() == XMLElementAttributeName)
            {
                xMLData = ParseXMLData(attributeDataAttribute);
            }

        }
        string upperName = childData.Name.ToUpper();
        childData.TDLFieldDetails ??= new()
        {
            Set = $"${upperName}",
            IncludeInFetch = false
        };
        childData.XmlTag = xMLData?.XmlTag ?? upperName;
    }
    private void ParseAttributes(SymbolData symbolData)
    {
        XMLData? xmlData = null;
        foreach (AttributeData attributeDataAttribute in symbolData.Attributes)
        {
            if (attributeDataAttribute.GetAttrubuteMetaName() == TDLXMLSetAttributeName)
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
        }
        return xMLData;
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
