using System.Collections.Immutable;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;
public class PropertyAttributesTransformer
{
    private static readonly Dictionary<string, AbstractPropertyAttributeTransformer> _propertyattributeTransformers = new()
    {
        { XMLElementAttributeName,new XmlElementAttributeTransformer() },
        { XMLArrayAttributeName,new XmlArrayAttributeTransformer() },
        { XMLArrayItemAttributeName,new XmlElementAttributeTransformer() },
        { XmlIgnoreAttributeName,new XmlIgnoreAttributeTransformer() },
        { TDLFieldAttributeName,new TDLFieldAttributeTransformer() },
        { TDLCollectionAttributeName,new PropertyCollectionAttributeTransformer() },
        { EnumChoiceAttributeName,new EnumChoiceAttributeTransformer() },
        { XMLEnumAttributeName,new XMLEnumAttributeTransformer() },
        { XMLAttributeAttributeName,new XmlAttributeAttributeTransformer() },
        { IgnoreForCreateDTOAttributeName,new IgnoreForCreateDTOAttributeTransformer() },
    };
    /// <summary>
    /// Propogates propertyData properties based on attributes to property
    /// </summary>
    /// <param name="propertyData"></param>
    /// <param name="memberAttributes"></param>
    internal static void TransformAsync(ClassPropertyData propertyData, ImmutableArray<AttributeData> memberAttributes)
    {
        foreach (var memberAttribute in memberAttributes)
        {
            var fullName = memberAttribute.GetAttrubuteMetaName();
            _propertyattributeTransformers.TryGetValue(fullName, out var attributeTransformer);

            attributeTransformer?.TransformAsync(propertyData, memberAttribute);
        }
        if (propertyData.TDLFieldData == null)
        {
            var tdlfieldData = new PropertyTDLFieldData();
            propertyData.TDLFieldData = tdlfieldData;
        }

        // Setting defaults
        if (propertyData.XMLData.Count == 1 && propertyData.DefaultXMLData == null)
        {
            propertyData.DefaultXMLData = propertyData.XMLData[0];
            propertyData.XMLData.RemoveAt(0);
        }
        propertyData.TDLFieldData.FetchText ??= propertyData.DefaultXMLData?.XmlTag ?? propertyData.Name.ToUpper();
        if (string.IsNullOrEmpty(propertyData.TDLFieldData.Set))
        {
            propertyData.TDLFieldData.Set = $"${propertyData.DefaultXMLData?.XmlTag ?? propertyData.Name.ToUpper()}";
        }
    }
}

internal class IgnoreForCreateDTOAttributeTransformer : AbstractPropertyAttributeTransformer
{
    public override void TransformAsync(ClassPropertyData propertyData, AttributeData attributeData)
    {
        propertyData.IgnoreForDTO = true;
    }
}