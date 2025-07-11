using TallyConnector.TDLReportSourceGenerator.Models;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Common;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;
public class PropertyCollectionAttributeTransformer : AbstractPropertyAttributeTransformer
{
    private readonly TDLCollectionAttributeTransformer _transformer = new();
    public PropertyCollectionAttributeTransformer()
    {

    }

    public override void TransformAsync(ClassPropertyData data, AttributeData attributeData)
    {
        var collectionData = _transformer.Transform(attributeData);
        data.TDLCollectionData = collectionData;

        //if (data.TDLCollectionData == null && data.)
        //{
        //    data.TDLCollectionData = data.BaseData.ClassData.TDLCollectionData;
        //}
    }
}
