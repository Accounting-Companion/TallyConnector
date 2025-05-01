namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;

public abstract class AbstractPropertyAttributeTransformer
{
    public abstract void TransformAsync(Models.PropertyData propertyData, AttributeData attributeData);
}