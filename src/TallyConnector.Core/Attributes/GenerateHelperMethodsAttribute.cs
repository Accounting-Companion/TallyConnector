namespace TallyConnector.Core.Attributes;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GenerateHelperMethodAttribute<ObjectType> : Attribute where ObjectType : ITallyRequestableObject 
{
    public string? MethodNameSuffix { get; set; }
    public string? MethodNameSuffixPlural { get; set; }
    public Type[]? Args { get; set; }
}
