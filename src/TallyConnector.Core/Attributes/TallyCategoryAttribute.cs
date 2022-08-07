using System.ComponentModel;

namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.All)]
public class TallyCategoryAttribute : CategoryAttribute
{
    public TallyCategoryAttribute(string category) : base(category)
    {
    }

}
