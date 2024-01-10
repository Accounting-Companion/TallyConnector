using System.Reflection;


namespace TallyConnector.Services;
public static class AttributeHelper
{
    public static TDLCollectionAttribute? GetTDLCollectionAttributeValue(Type type, ILogger? _logger = null)
    {
        TDLCollectionAttribute[] TDLColattribute = (TDLCollectionAttribute[])Attribute.GetCustomAttributes(type, typeof(TDLCollectionAttribute));
        if (TDLColattribute.Length > 0)
        {
            return TDLColattribute[0];
        }
        return null;
    }
    public static TDLCollectionAttribute? GetTDLCollectionAttributeValue(PropertyInfo propertyinfo)
    {
        TDLCollectionAttribute[] CElement = (TDLCollectionAttribute[])Attribute.GetCustomAttributes(propertyinfo, typeof(TDLCollectionAttribute));//propertyinfo.CustomAttributes.FirstOrDefault(Attributedata => Attributedata.AttributeType == typeof(XmlAttributeAttribute));
        if (CElement.Length > 0)
        {
            return CElement[0];
        }
        return null;
    }

    public static string? GetXmlElement(PropertyInfo propertyinfo)
    {
        XmlElementAttribute[] CElement = (XmlElementAttribute[])Attribute.GetCustomAttributes(propertyinfo, typeof(XmlElementAttribute));//propertyinfo.CustomAttributes.FirstOrDefault(Attributedata => Attributedata.AttributeType == typeof(XmlAttributeAttribute));
        if (CElement.Length > 0)
        {
            string xmlTag = CElement[0].ElementName;
            return xmlTag;
        }
        return null;
    }
    public static TallyObjectType? GetTallyObjectTypeAttribute(Type type, ILogger? _logger = null)
    {
        Type attributeType = typeof(TallyObjectTypeAttribute);
        _logger?.LogDebug("Getting {Attribute} attribute of {objtype}", attributeType.Name, type.Name);
        TallyObjectTypeAttribute[] elem = (TallyObjectTypeAttribute[])Attribute.GetCustomAttributes(type, attributeType);//propertyinfo.CustomAttributes.FirstOrDefault(Attributedata => Attributedata.AttributeType == typeof(XmlAttributeAttribute));
        _logger?.LogDebug("{object} has {count} {Attribute}s", type.Name, elem.Length, attributeType.Name);
        if (elem.Length > 0)
        {
            var objectType = elem[0].TallyObjectType;
            return objectType;
        }
        return null;
    }
    public static string? GetXmlRootElement(Type type, ILogger? _logger = null)
    {
        _logger?.LogDebug("Getting {Attribute} attribute of {objtype}", typeof(XmlRootAttribute).Name, type.Name);
        XmlRootAttribute[] RElement = (XmlRootAttribute[])Attribute.GetCustomAttributes(type, typeof(XmlRootAttribute));//propertyinfo.CustomAttributes.FirstOrDefault(Attributedata => Attributedata.AttributeType == typeof(XmlAttributeAttribute));
        _logger?.LogDebug("{object} has {count} {Attribute}s", type.Name, RElement.Length, typeof(XmlRootAttribute).Name);
        if (RElement.Length > 0)
        {
            string xmlTag = RElement[0].ElementName;
            return xmlTag;
        }
        return null;
    }

    public static TDLFieldAttribute? GetTDLXMLSetAttributeValue(PropertyInfo propertyinfo)
    {
        TDLFieldAttribute[] CElement = (TDLFieldAttribute[])Attribute.GetCustomAttributes(propertyinfo, typeof(TDLFieldAttribute));//propertyinfo.CustomAttributes.FirstOrDefault(Attributedata => Attributedata.AttributeType == typeof(XmlAttributeAttribute));
        if (CElement.Length > 0)
        {
            return CElement[0];
        }
        return null;
    }


}
