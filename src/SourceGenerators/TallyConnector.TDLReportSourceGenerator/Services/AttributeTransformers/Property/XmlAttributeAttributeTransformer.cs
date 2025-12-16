using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;

public class XmlAttributeAttributeTransformer : AbstractPropertyAttributeTransformer
{
    public override void TransformAsync(ClassPropertyData propertyData, AttributeData attributeData)
    {
        string? attributeName = null;
        if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
        {
            attributeName = attributeData.ConstructorArguments.First().Value?.ToString();
        }
        else if (attributeData.NamedArguments != null && attributeData.NamedArguments.Length > 0)
        {
            attributeName = attributeData.NamedArguments.FirstOrDefault(a => a.Key == "AttributeName").Value.Value as string;
        }

        propertyData.IsAttribute = true;
        string xmlTag = attributeName ?? propertyData.Name.ToUpper();
        var xmlData = new XMLData { XmlTag = xmlTag };
        propertyData.DefaultXMLData = xmlData;
    }
}
