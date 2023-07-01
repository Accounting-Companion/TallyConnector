using TallyConnector.Core.Models.Common;
using TallyConnector.Core.Models.Common.Request;
using TallyConnector.Core.Models.Common.Response;

namespace TallyConnector.Core.Attributes;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GenerateHelperMethodAttribute<ReturnObjectType, GetObjectType, PostObjectType> :
        GenerateHelperMethodAttribute<ReturnObjectType,
            GetObjectType,
            PostObjectType,
            RequestEnvelope,
            ResponseEnvelope<GetObjectType>, PostRequestEnvelope<PostObjectType>>
                where GetObjectType : TallyObject
                where PostObjectType : IPostTallyObject
                where ReturnObjectType : class
{
}
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class GenerateHelperMethodAttribute<ReturnObjectType, GetObjectType, PostObjectType, RequestEnvelopeType, ResponseEnvelopeType, PostEnvelopeType> : Attribute
    where GetObjectType : TallyObject
    where PostObjectType : IPostTallyObject
    where ReturnObjectType : class
    where RequestEnvelopeType : RequestEnvelope
    where ResponseEnvelopeType : ResponseEnvelope<GetObjectType>
    where PostEnvelopeType : PostRequestEnvelope<PostObjectType>
{
    public HelperMethodGenerationMode GenerationMode { get; set; }

    public GenerateHelperMethodAttribute()
    {
    }
}
