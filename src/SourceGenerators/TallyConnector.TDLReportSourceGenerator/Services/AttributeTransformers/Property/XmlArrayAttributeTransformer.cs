using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;

public class XmlArrayAttributeTransformer : AbstractPropertyAttributeTransformer
{
    public override void TransformAsync(ClassPropertyData propertyData, AttributeData attributeData)
    {
        XMLData? xMLData = null;
        if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
        {
            var constructorArguments = attributeData.ConstructorArguments;
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
        if (attributeData.NamedArguments != null && attributeData.NamedArguments.Length > 0)
        {
            var namedArguments = attributeData.NamedArguments;
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
        if (xMLData == null)
        {
            return;
        }
        propertyData.ListXMLTag = xMLData.XmlTag;
    }
}
