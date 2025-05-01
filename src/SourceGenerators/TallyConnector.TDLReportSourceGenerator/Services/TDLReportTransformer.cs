using System.Threading.Tasks;
using TallyConnector.TDLReportSourceGenerator.Models;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;

namespace TallyConnector.TDLReportSourceGenerator.Services;
public class TDLReportTransformer
{
    private readonly IAssemblySymbol _assembly;
    private readonly Dictionary<string, ModelData> _processedSymbols = [];

    public TDLReportTransformer(IAssemblySymbol assembly)
    {
        _assembly = assembly;
    }

    public async Task<ModelData> TransformAsync(INamedTypeSymbol symbol, CancellationToken token)
    {
        ModelData modelData = new(symbol);
        _processedSymbols.TryGetValue(modelData.FullName, out var ExistingmodelData);
        if (ExistingmodelData != null)
        {
            return ExistingmodelData;
        }
        await TransformBaseSymbolDataAsync(modelData, symbol.BaseType, token);
        TransformMembers(modelData, symbol);

        _processedSymbols[modelData.FullName] = modelData;
        return modelData;
    }
    public List<ModelData> GetTransformedData()
    {
        return [.. _processedSymbols.Values];

    }

    private async Task TransformBaseSymbolDataAsync(ModelData modelData, INamedTypeSymbol? baseType, CancellationToken token)
    {
        if (baseType == null || baseType.HasFullyQualifiedMetadataName("object"))
        {
            return;
        }
        bool isRequestableObjectInterface = baseType.HasInterfaceWithFullyQualifiedMetadataName(Constants.Models.Interfaces.TallyRequestableObjectInterfaceFullName);

        bool IsTallyRequestableObject = isRequestableObjectInterface || baseType.GetAttributes().Any(c => c.HasFullyQualifiedMetadataName(Attributes.SourceGenerator.ImplementTallyRequestableObjectAttribute));
        modelData.BaseData = new(baseType.GetClassMetaName(), baseType.Name)
        {
            IsTallyRequestableObject = IsTallyRequestableObject
        };

        if (!IsTallyRequestableObject)
        {
            TransformMembers(modelData, baseType);
            return;
        }
        bool isSameAssembly = SymbolEqualityComparer.Default.Equals(baseType.ContainingAssembly, _assembly);
        if (!isSameAssembly)
        {
            // Todo : Handle this when base class is from not same assembly we need to acess simple properties count and save
            //modelData.SimplePropertiesCount = 2;
            return;
        }
        modelData.BaseData.ModelData = await TransformAsync(baseType, token);
        modelData.SimplePropertiesCount += modelData.BaseData.ModelData.SimplePropertiesCount;

    }

    private void TransformMembers(ModelData modelData, INamedTypeSymbol symbol)
    {
        var members = symbol.GetMembers();
        foreach (var member in members)
        {
            if (member.DeclaredAccessibility != Accessibility.Public || member.Kind != SymbolKind.Property)
            {
                continue;
            }
            PropertyData propertyData = TransformMember(member);
            propertyData.SetFieldName($"{_assembly.Name}\0{modelData.FullName}");
            if (propertyData.IsComplex)
            {

            }
            else
            {
                modelData.SimplePropertiesCount++;
            }
            if (modelData.Properties.ContainsKey(propertyData.Name))
            {
                propertyData.IsOveridden = true;
            }
            modelData.Properties[propertyData.Name] = propertyData;
        }
    }

    private PropertyData TransformMember(ISymbol member)
    {
        PropertyData propertyData = new(member);
        PropertyAttributesTransformer.TransformAsync(propertyData, member.GetAttributes());
        return propertyData;
    }
}

