using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Class;
internal class XMLRootAttributeTransformer : ClassPropertyAttributeTransformer
{
    public override void TransformAsync(ModelData data, AttributeData attributeData)
    {
        XMLData? xMLData = null;
        if (attributeData.ConstructorArguments != null && attributeData.ConstructorArguments.Length > 0)
        {
            xMLData ??= new();
            var constructorArguments = attributeData.ConstructorArguments;
            for (int i = 0; i < constructorArguments.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        xMLData.XmlTag = (string)constructorArguments[i].Value!;
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
                }
            }

        }
        data.XMLTag = xMLData?.XmlTag;
    }
}
