using TallyConnector.Core.Models;

namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GenerateHelperMethodsAttribute<T> : Attribute where T : Models.BasicTallyObject
{
    public string? PluralName { get; set; }

    public string? TypeName { get; set; }
    public string? MethodName { get; set; }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GenerateHelperMethodAttribute<GetObjectType> : GenerateHelperMethodAttribute<GetObjectType, RequestEnvelope,ResponseEnvelope>
    where GetObjectType : ITallyBaseObject
{
    
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GenerateHelperMethodAttribute<GetObjectType, RequestEnvelopeType, ResponseEnvelopeType> : Attribute
    where GetObjectType : ITallyBaseObject
    where RequestEnvelopeType : RequestEnvelope
    where ResponseEnvelopeType : ResponseEnvelope
{
    
}
