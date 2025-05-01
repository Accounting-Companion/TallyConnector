namespace TallyConnector.Core.Models.Interfaces;

#if NET8_0_OR_GREATER
public interface ITallyRequestableObject
{
    public static abstract Task<RequestEnvelope> GetRequestEnvelope();

}


#endif
