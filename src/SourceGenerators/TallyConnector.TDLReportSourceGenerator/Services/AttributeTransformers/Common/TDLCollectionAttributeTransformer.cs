using System.Collections.Immutable;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Common;
public class TDLCollectionAttributeTransformer
{
    public TDLCollectionData? Transform(AttributeData attributeData)
    {
        TDLCollectionData? tdlCollectionData = null;
        if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
        {
            ImmutableArray<TypedConstant> constructorArguments = attributeData.ConstructorArguments;
            tdlCollectionData = new()
            {
                CollectionName = (string)constructorArguments.First().Value!
            };
        }
        if (attributeData.NamedArguments != null && attributeData.NamedArguments.Length > 0)
        {
            ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = attributeData.NamedArguments;

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
}
public class TDLCollectionData
{
    public string? CollectionName { get; set; }
    public bool? Exclude { get; set; }
    public string? ExplodeCondition { get; internal set; }
    public string? Type { get; internal set; }
    public INamedTypeSymbol? Symbol { get; internal set; }
}
