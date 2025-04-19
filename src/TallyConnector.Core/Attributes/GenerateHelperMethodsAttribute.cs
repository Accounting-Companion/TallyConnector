namespace TallyConnector.Core.Attributes;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GenerateHelperMethodAttribute<GetObjectType> : GenerateHelperMethodAttribute<GetObjectType, RequestEnvelope>
    where GetObjectType : IBaseObject
{

}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GenerateHelperMethodAttribute<GetObjectType, RequestEnvelopeType> : Attribute
    where GetObjectType : IBaseObject
    where RequestEnvelopeType : RequestEnvelope
{
    public GenerationMode GenerationMode { get; set; }
    public string? MethodNameSuffix { get; set; }
    public string? MethodNameSuffixPlural { get; set; }
    public Type[]? Args { get; set; }
}
