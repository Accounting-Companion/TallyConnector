using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;

public class EnumChoiceAttributeTransformer : AbstractPropertyAttributeTransformer
{
    public override void TransformAsync(ClassPropertyData propertyData, AttributeData attributeData)
    {
        propertyData.DefaultXMLData ??= new();

        string? choice =null;
        if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeData.ConstructorArguments;
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
        if (attributeData.NamedArguments != null && attributeData.NamedArguments.Length > 0)
        {
            var namedArguments = attributeData.NamedArguments;
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
        propertyData.DefaultXMLData.EnumChoices.Add(new (choice ?? propertyData.Name, versions));
    }
}
public class XMLEnumAttributeTransformer : AbstractPropertyAttributeTransformer
{
    public override void TransformAsync(ClassPropertyData propertyData, AttributeData attributeData)
    {
        propertyData.DefaultXMLData ??= new();

        string? choice = null;
        if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeData.ConstructorArguments;
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
        if (attributeData.NamedArguments != null && attributeData.NamedArguments.Length > 0)
        {
            var namedArguments = attributeData.NamedArguments;
            foreach (var namedArgument in namedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "Name":
                        choice = (string)namedArgument.Value.Value!;
                        break;
                }
            }

        }
        propertyData.DefaultXMLData.EnumChoices.Add(new(choice ?? propertyData.Name, versions));
    }
}