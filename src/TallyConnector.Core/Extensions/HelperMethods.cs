using System.Security.AccessControl;
using TallyConnector.Core.Models;
using static TallyConnector.TDLReportSourceGenerator.Constants;
using System.Linq;
namespace TallyConnector.Core.Extensions;
public static class HelperMethods
{
    public static string ToTallyString(this YesNo yesNo)
    {
        return yesNo == YesNo.Yes ? "Yes" : "No";
    }
    public static string? ToTallyString(this bool? src)
    {
        if (src == null) return null;
        return src.Value ? "Yes" : "No";
    }
    public static string ToTallyString(this bool src)
    {
        return src ? "Yes" : "No";
    }
    public static string ToTallyString(this DateTime src)
    {
        return src.ToString();
    }
    public static string? ToTallyString(this DateTime? src)
    {
        return src.ToString();
    }
    public static Type RequestDataType = typeof(RequestData);
    public const string RequestDataMember = nameof(RequestData.Data);
    public static XMLOverrideswithTracking AddDataAttributeOverrides(this XMLOverrideswithTracking xMLOverrides, XmlAttributes xmlAttributes, bool safeCheck = false)
    {
        
        if (xMLOverrides.TryGet(RequestDataType, RequestDataMember,out var existing))
        {
            foreach (XmlArrayItemAttribute newItem in xmlAttributes.XmlArrayItems)
            {
                bool found = false;
                foreach (XmlArrayItemAttribute existingItem in existing.XmlArrayItems)
                {
                    if (existingItem.ElementName == newItem.ElementName &&
                        existingItem.Type == newItem.Type)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    existing.XmlArrayItems.Add(newItem);
                }
            }
            return xMLOverrides;
        }
        xMLOverrides.Add(RequestDataType, RequestDataMember, xmlAttributes);
        return xMLOverrides;
    }
    public static XMLOverrideswithTracking AddCollectionArrayItemAttributeOverrides(this XMLOverrideswithTracking xMLOverrides, string xmlTag, Type type)
    {
        var XMLAttributes = new XmlAttributes();
        XMLAttributes.XmlArrayItems.Add(new(xmlTag, type));
        XMLAttributes.XmlArray = new("COLLECTION");
        xMLOverrides.AddDataAttributeOverrides(XMLAttributes);
        return xMLOverrides;
    }
    public static XMLOverrideswithTracking AddMessageArrayItemAttributeOverrides(this XMLOverrideswithTracking xMLOverrides, string xmlTag, Type type)
    {
        var XMLAttributes = new XmlAttributes();
        XMLAttributes.XmlArrayItems.Add(new(xmlTag, type));
        XMLAttributes.XmlArray = new("TALLYMESSAGE");
        xMLOverrides.AddDataAttributeOverrides(XMLAttributes);
        return xMLOverrides;
    }

}
