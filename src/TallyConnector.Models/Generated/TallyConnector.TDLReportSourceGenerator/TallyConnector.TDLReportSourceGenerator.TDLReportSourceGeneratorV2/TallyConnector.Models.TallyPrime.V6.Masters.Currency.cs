using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Models.TallyPrime.V6.Masters;
/*
* Generated based on TallyConnector.Models.TallyPrime.V6.Masters.Currency
*/
partial class Currency : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string GUID_QW2F_FieldName = "GUID_QW2F";
    const string RemoteId_4JMJ_FieldName = "RemoteId_4JMJ";
    const string MasterId_2JFP_FieldName = "MasterId_2JFP";
    const string AlterId_RUKQ_FieldName = "AlterId_RUKQ";
    const string EnteredBy_WTJ3_FieldName = "EnteredBy_WTJ3";
    const string AlteredBy_K5HW_FieldName = "AlteredBy_K5HW";
    const string Name_LOQY_FieldName = "Name_LOQY";
    const string FormalName_4PG6_FieldName = "FormalName_4PG6";
    const string ReportName = "Currency_VO7L";
    const string CollectionName = "CurrencysCollection_VO7L";
    const string XMLTag = "CURRENCY";
    const int SimpleFieldsCount = 8;
    const int ComplexFieldsCount = 0;
    public static global::TallyConnector.Core.Models.Request.RequestEnvelope GetRequestEnvelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.Request.RequestEnvelope(global::TallyConnector.Core.Models.Request.HType.Data, ReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(ReportName)];
        tdlMsg.Form = [new(ReportName)];
        tdlMsg.Part = [GetMainTDLPart(), ..GetTDLParts()];
        tdlMsg.Line = [GetMainTDLLine(), ..GetTDLLines()];
        tdlMsg.Field = [..GetTDLFields()];
        tdlMsg.Collection = [..GetTDLCollections()];
        return reqEnvelope;
    }

    public static global::System.Xml.Serialization.XmlAttributeOverrides GetXMLAttributeOverides()
    {
        var xmlAttributeOverrides = new global::System.Xml.Serialization.XmlAttributeOverrides();
        var XmlAttributes = new global::System.Xml.Serialization.XmlAttributes();
        XmlAttributes.XmlElements.Add(new(XMLTag));
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::TallyConnector.Models.TallyPrime.V6.Masters.Currency>.TypeInfo, "Objects", XmlAttributes);
        xmlAttributeOverrides.Add(typeof(global::TallyConnector.Models.Base.BaseMasterObject), "Name", new global::System.Xml.Serialization.XmlAttributes() { XmlIgnore = true });
        return xmlAttributeOverrides;
    }

    public static global::TallyConnector.Core.Models.Request.Part GetMainTDLPart(string partName = ReportName, string? collectionName = CollectionName, string? xmlTag = null)
    {
        return new(partName, collectionName, partName)
        {
            XMLTag = xmlTag
        };
    }

    public static global::TallyConnector.Core.Models.Request.Part[] GetTDLParts()
    {
        return[];
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [GUID_QW2F_FieldName,RemoteId_4JMJ_FieldName,MasterId_2JFP_FieldName,AlterId_RUKQ_FieldName,EnteredBy_WTJ3_FieldName,AlteredBy_K5HW_FieldName,Name_LOQY_FieldName,FormalName_4PG6_FieldName], XMLTag);
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        return[];
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(GUID_QW2F_FieldName, "GUID", "$GUID");
        _fields[1] = new(RemoteId_4JMJ_FieldName, "REMOTEALTGUID", "$REMOTEALTGUID");
        _fields[2] = new(MasterId_2JFP_FieldName, "MASTERID", "$MASTERID");
        _fields[3] = new(AlterId_RUKQ_FieldName, "ALTERID", "$ALTERID");
        _fields[4] = new(EnteredBy_WTJ3_FieldName, "ENTEREDBY", "$ENTEREDBY");
        _fields[5] = new(AlteredBy_K5HW_FieldName, "ALTEREDBY", "$ALTEREDBY");
        _fields[6] = new(Name_LOQY_FieldName, "ORIGINALNAME", "$ORIGINALNAME");
        _fields[7] = new(FormalName_4PG6_FieldName, "MAILINGNAME", "$MAILINGNAME");
        return _fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, "CURRENCY", nativeFields: [..GetFetchList()]);
        return collections;
    }

    internal static string[] GetFetchList()
    {
        return["REMOTEALTGUID", "ALTERID", "ENTEREDBY", "ALTEREDBY", "ORIGINALNAME", "MAILINGNAME"];
    }
}