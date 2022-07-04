using System.Reflection;


namespace TallyConnector.Services;
public static class AttributeHelper
{
    public static TDLCollectionAttribute? GetTDLCollectionAttributeValue(Type type)
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

    public static TDLXMLSetAttribute? GetTDLXMLSetAttributeValue(PropertyInfo propertyinfo)
    {
        TDLXMLSetAttribute[] CElement = (TDLXMLSetAttribute[])Attribute.GetCustomAttributes(propertyinfo, typeof(TDLXMLSetAttribute));//propertyinfo.CustomAttributes.FirstOrDefault(Attributedata => Attributedata.AttributeType == typeof(XmlAttributeAttribute));
        if (CElement.Length > 0)
        {
            return CElement[0];
        }
        return null;
    }
}
