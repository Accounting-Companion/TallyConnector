using TallyConnector.Core.Models;

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
}
