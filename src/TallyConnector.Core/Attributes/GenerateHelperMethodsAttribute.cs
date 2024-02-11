using TallyConnector.Core.Models;

namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GenerateHelperMethodsAttribute<T> : Attribute where T : Models.BasicTallyObject
{
    public string? PluralName { get; set; }
    public string? TypeName { get; set; }
    public string? MethodName { get; set; }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GenerateHelperMethodAttribute<GetObjectType> : GenerateHelperMethodAttribute<GetObjectType, RequestEnvelope>
    where GetObjectType : ITallyBaseObject
{

}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GenerateHelperMethodAttribute<GetObjectType, RequestEnvelopeType> : Attribute
    where GetObjectType : ITallyBaseObject
    where RequestEnvelopeType : RequestEnvelope
{
    public GenerationMode GenerationMode { get; set; }
    public string? MethodNameSuffix { get; set; }
    public string? MethodNameSuffixPlural { get; set; }
    public Type[]? Args { get; set; }
}
