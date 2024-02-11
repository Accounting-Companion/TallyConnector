namespace TallyConnector.TDLReportSourceGenerator.Extensions.Symbols;
public static class AttributeDataExtensions
{
    public static bool HasFullyQualifiedMetadataName(this AttributeData attributeData, string metaName)
    {
        return attributeData.GetAttrubuteMetaName() == metaName;
    }
    public static string GetAttrubuteMetaName(this AttributeData attributeData)
    {
        if (attributeData.AttributeClass!.IsGenericType)
        {

            string name = attributeData.AttributeClass!.OriginalDefinition.ToString();
            name = name.Split('<').First();
            return name;
        }
        else
        {
            string attributeMetaName = attributeData.AttributeClass!.OriginalDefinition.ToString();
            return attributeMetaName;
        }

    }
}
