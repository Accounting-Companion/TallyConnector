using System.Collections.Immutable;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;
public class TDLFieldAttributeTransformer : AbstractPropertyAttributeTransformer
{
    public override void TransformAsync(PropertyData propertyData, AttributeData attributeData)
    {
        PropertyTDLFieldData? fieldData = null;
        if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
        {
            ImmutableArray<TypedConstant> constructorArguments = attributeData.ConstructorArguments;
            fieldData = new()
            {
                Set = (string)constructorArguments.First().Value!
            };
            if (constructorArguments.Length == 2)
            {
                fieldData.ExcludeInFetch = (bool)constructorArguments.Skip(1).First().Value!;
            }

        }
        if (attributeData.NamedArguments != null && attributeData.NamedArguments.Length > 0)
        {
            ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = attributeData.NamedArguments;

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

        propertyData.TDLFieldData = fieldData;
     
    }
}
