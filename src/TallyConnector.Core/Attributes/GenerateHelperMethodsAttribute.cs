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
public class GenerateHelperMethodAttribute<GetObjectType> : GenerateHelperMethodAttribute<GetObjectType, GetObjectType, RequestEnvelope,ResponseEnvelope>
    where GetObjectType : ITallyBaseObject
{
    
}
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GenerateHelperMethodAttribute<GetObjectType, PostObjectType> : GenerateHelperMethodAttribute<GetObjectType, PostObjectType,RequestEnvelope,ResponseEnvelope>
    where GetObjectType : ITallyBaseObject
    where PostObjectType : ITallyBaseObject
{
    
}
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GenerateHelperMethodAttribute<GetObjectType, PostObjectType, RequestEnvelopeType, ResponseEnvelopeType> : Attribute
    where GetObjectType : ITallyBaseObject
    where PostObjectType : ITallyBaseObject
    where RequestEnvelopeType : RequestEnvelope
    where ResponseEnvelopeType : ResponseEnvelope
{
    
}
