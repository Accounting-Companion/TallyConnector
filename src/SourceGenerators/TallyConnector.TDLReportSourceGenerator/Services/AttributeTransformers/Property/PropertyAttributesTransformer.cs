using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using TallyConnector.TDLReportSourceGenerator.Models;

namespace TallyConnector.TDLReportSourceGenerator.Services.AttributeTransformers.Property;
public class PropertyAttributesTransformer
{
    private static readonly Dictionary<string, AbstractPropertyAttributeTransformer> _propertyattributeTransformers = new()
    {
        { TDLFieldAttributeName,new TDLFieldAttributeTransformer() }
    };
    /// <summary>
    /// Propogates propertyData properties based on attributes to property
    /// </summary>
    /// <param name="propertyData"></param>
    /// <param name="memberAttributes"></param>
    internal static void TransformAsync(PropertyData propertyData, ImmutableArray<AttributeData> memberAttributes)
    {
        foreach (var memberAttribute in memberAttributes)
        {
            var fullName = memberAttribute.GetAttrubuteMetaName();
            _propertyattributeTransformers.TryGetValue(fullName, out var attributeTransformer);
            if(attributeTransformer == null)
            {
                return;
            }
            attributeTransformer.TransformAsync(propertyData, memberAttribute);
        }
    }
}
