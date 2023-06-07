using TallyConnector.Core.Models;

namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Class)]
public class TallyObjectTypeAttribute : Attribute
{
    public TallyObjectTypeAttribute(TallyObjectType tallyObjectType)
    {
        TallyObjectType = tallyObjectType;
    }
    public TallyObjectType TallyObjectType { get; set; }
}
