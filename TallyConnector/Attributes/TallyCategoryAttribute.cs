using System.ComponentModel;

namespace TallyConnector.Attributes;
[AttributeUsage(AttributeTargets.All)]
public class TallyCategoryAttribute : CategoryAttribute
{
    public TallyCategoryAttribute(string category) : base(category)
    {
    }

}
