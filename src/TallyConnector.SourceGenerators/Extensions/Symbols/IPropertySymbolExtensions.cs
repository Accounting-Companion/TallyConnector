using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TallyConnector.SourceGenerators.Extensions.Symbols;
public static class IPropertySymbolExtensions
{
    public static string? GetXmlTagFromPropertyAttributes(this IPropertySymbol propertySymbol)
    {
        System.Collections.Immutable.ImmutableArray<AttributeData> attributeData = propertySymbol.GetAttributes();
        foreach (AttributeData attributeDataAttribute in attributeData)
        {
            if (attributeDataAttribute.GetAttrubuteMetaName() == "System.Xml.Serialization.XmlElementAttribute")
            {
                if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
                {
                    var name = attributeDataAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (name != null)
                    {
                        return name;
                    }
                }
            }
        }
        return null;
    }
    public static XMlProperties? GetXmlProperties(this IPropertySymbol propertySymbol)
    {
        System.Collections.Immutable.ImmutableArray<AttributeData> attributeData = propertySymbol.GetAttributes();
        foreach (AttributeData attributeDataAttribute in attributeData)
        {
            if (attributeDataAttribute.GetAttrubuteMetaName() == "System.Xml.Serialization.XmlElementAttribute")
            {
                if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
                {
                    var name = attributeDataAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (name != null)
                    {
                        return new(name);
                    }
                }
            }

            if (attributeDataAttribute.GetAttrubuteMetaName() == "System.Xml.Serialization.XmlAttributeAttribute")
            {
                if (attributeDataAttribute.ConstructorArguments != null && attributeDataAttribute.ConstructorArguments.Length > 0)
                {
                    var name = attributeDataAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                    if (name != null)
                    {
                        return new(name, true);
                    }

                }
                if (attributeDataAttribute.NamedArguments != null && attributeDataAttribute.NamedArguments.Length > 0)
                {
                    System.Collections.Immutable.ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = attributeDataAttribute.NamedArguments;
                    foreach (var namedArgument in namedArguments)
                    {
                        if(namedArgument.Key != null && namedArgument.Key == "AttributeName")
                        {
                            if (!namedArgument.Value.IsNull)
                            {
                                return new(namedArgument.Value.Value!.ToString(), true);
                            }
                        }
                    }
                }
            }
        }
        return null;
    }
}
public class XMlProperties
{
    public XMlProperties(string xmlTag)
    {
        XMLTag = xmlTag;
    }
    public XMlProperties(string xmlTag, bool isAttribute)
    {
        XMLTag = xmlTag;
        IsAttribute = true;
    }

    public string XMLTag { get; set; }

    public bool IsAttribute { get; set; }
}
