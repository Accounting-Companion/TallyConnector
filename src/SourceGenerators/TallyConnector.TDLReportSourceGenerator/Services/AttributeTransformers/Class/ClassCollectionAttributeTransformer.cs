using TallyConnector.TDLReportSourceGenerator.Models;
using TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Common;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Class;
public class ClassCollectionAttributeTransformer : ClassPropertyAttributeTransformer
{
    private readonly TDLCollectionAttributeTransformer _transformer = new();
    public ClassCollectionAttributeTransformer()
    {

    }

    public override void TransformAsync(ClassData data, AttributeData attributeData)
    {
        var collectionData = _transformer.Transform(attributeData);
        data.TDLCollectionData = collectionData;
        if (data.TDLCollectionData == null && data.BaseData?.TDLCollectionData != null)
        {
            data.TDLCollectionData = data.BaseData.TDLCollectionData;
        }
    }
}
