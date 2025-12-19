namespace TallyConnector.Core.Models.Interfaces;


public interface ITallyRequestableObject
{
    // static string XMLTag;
    static abstract XMLOverrideswithTracking GetXMLAttributeOverides();
    static abstract RequestEnvelope GetRequestEnvelope();
    static abstract RequestEnvelope GetCountRequestEnvelope();

}


public interface IBaseTallyRequestableObject
{
    static abstract Part[] GetTDLParts();
    static abstract Field[] GetTDLFields();
}
