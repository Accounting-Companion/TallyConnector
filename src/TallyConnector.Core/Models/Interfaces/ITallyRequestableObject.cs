namespace TallyConnector.Core.Models.Interfaces;


public interface ITallyRequestableObject
{
    // static string XMLTag;
    static abstract XmlAttributeOverrides GetXMLAttributeOverides();
    static abstract RequestEnvelope GetRequestEnvelope();

}


public interface IBaseTallyRequestableObject
{
    static abstract Part[] GetTDLParts();
    static abstract Field[] GetTDLFields();
}
