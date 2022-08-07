using System.Reflection;

namespace TallyConnector.Services;

/// <summary>
/// Helper class to cache PropertyInfo generated through Reflection 
/// </summary>
public static class PropertyHelper
{
    private static Dictionary<Type, PropertyInfo[]> _propertyInfoDict = new();
    public static PropertyInfo[] GetPropertyInfo(Type type)
    {
        _ = _propertyInfoDict.TryGetValue(type, out PropertyInfo[]? PropertyInfo);
        if (PropertyInfo == null)
        {
            PropertyInfo = type.GetProperties();
            _propertyInfoDict[type] = PropertyInfo;
        };
        return PropertyInfo;
    }
}
