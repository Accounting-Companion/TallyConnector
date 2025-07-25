
namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;

internal class XmlIgnoreAttributeTransformer : AbstractPropertyAttributeTransformer
{
    public override void TransformAsync(ClassPropertyData propertyData, AttributeData attributeData)
    {
        propertyData.XmlIgnore = true;
    }
}