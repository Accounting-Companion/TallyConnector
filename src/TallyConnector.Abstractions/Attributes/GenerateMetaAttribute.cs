namespace TallyConnector.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class|AttributeTargets.Enum,AllowMultiple =false)]
public class GenerateMetaAttribute : Attribute
{
}
