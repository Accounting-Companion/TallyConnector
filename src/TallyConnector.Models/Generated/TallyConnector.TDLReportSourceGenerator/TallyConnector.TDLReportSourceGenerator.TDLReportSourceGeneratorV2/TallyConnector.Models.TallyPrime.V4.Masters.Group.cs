using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Models.TallyPrime.V4.Masters;
/*
* Generated based on TallyConnector.Models.TallyPrime.V4.Masters.Group
*/
partial class Group : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string GUID_QW2F_FieldName = "GUID_QW2F";
    const string RemoteId_4JMJ_FieldName = "RemoteId_4JMJ";
    const string MasterId_2JFP_FieldName = "MasterId_2JFP";
    const string AlterId_RUKQ_FieldName = "AlterId_RUKQ";
    const string EnteredBy_WTJ3_FieldName = "EnteredBy_WTJ3";
    const string AlteredBy_K5HW_FieldName = "AlteredBy_K5HW";
    const string Name_SMP5_FieldName = "Name_SMP5";
    const string Alias_ADB2_FieldName = "Alias_ADB2";
    const string Parent_CTOI_FieldName = "Parent_CTOI";
    const string ReservedName_VZK2_FieldName = "ReservedName_VZK2";
    const string IsRevenue_0NZM_FieldName = "IsRevenue_0NZM";
    const string IsDeemedPositive_WPPA_FieldName = "IsDeemedPositive_WPPA";
    const string AffectGrossProfit_O1S3_FieldName = "AffectGrossProfit_O1S3";
    const string IsSubledger_PNHX_FieldName = "IsSubledger_PNHX";
    const string SortPosition_9H2I_FieldName = "SortPosition_9H2I";
    const string AddlAllocType_X6BO_FieldName = "AddlAllocType_X6BO";
    const string IsCalculable_0LSO_FieldName = "IsCalculable_0LSO";
    const string IsAddable_FOD7_FieldName = "IsAddable_FOD7";
    const string NameList_ZIWC_FieldName = "NameList_ZIWC";
    const string LanguageId_XANV_FieldName = "LanguageId_XANV";
    const string LanguageNameList_S9R3_PartName = "LanguageNameList_S9R3";
    const string NameList_ZIWC_PartName = "NameList_ZIWC";
    const string ReportName = "Group_U9XI";
    const string CollectionName = "GroupsCollection_U9XI";
    const string XMLTag = "GROUP";
    const int SimpleFieldsCount = 20;
    const int ComplexFieldsCount = 2;
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
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::TallyConnector.Models.TallyPrime.V4.Masters.Group>.TypeInfo, "Objects", XmlAttributes);
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
        var parts = new global::TallyConnector.Core.Models.Request.Part[ComplexFieldsCount];
        parts[0] = new(LanguageNameList_S9R3_PartName, "LanguageName");
        parts[1] = new(NameList_ZIWC_PartName, "Name")
        {
            XMLTag = "NAME.LIST"
        };
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [GUID_QW2F_FieldName,RemoteId_4JMJ_FieldName,MasterId_2JFP_FieldName,AlterId_RUKQ_FieldName,EnteredBy_WTJ3_FieldName,AlteredBy_K5HW_FieldName,Name_SMP5_FieldName,Alias_ADB2_FieldName,Parent_CTOI_FieldName,ReservedName_VZK2_FieldName,IsRevenue_0NZM_FieldName,IsDeemedPositive_WPPA_FieldName,AffectGrossProfit_O1S3_FieldName,IsSubledger_PNHX_FieldName,SortPosition_9H2I_FieldName,AddlAllocType_X6BO_FieldName,IsCalculable_0LSO_FieldName,IsAddable_FOD7_FieldName], XMLTag)
        {
            Explode = [$"{LanguageNameList_S9R3_PartName}:YES"]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(LanguageNameList_S9R3_PartName, [LanguageId_XANV_FieldName], "LANGUAGENAME.LIST")
        {
            Explode = [$"{NameList_ZIWC_PartName}:YES"]
        };
        _lines[1] = new(NameList_ZIWC_PartName, [NameList_ZIWC_FieldName]);
        return _lines;
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
        _fields[6] = new(Name_SMP5_FieldName, "NAME", "$NAME");
        _fields[7] = new(Alias_ADB2_FieldName, "ALIAS", "$_FirstAlias");
        _fields[8] = new(Parent_CTOI_FieldName, "PARENT", "$PARENT");
        _fields[9] = new(ReservedName_VZK2_FieldName, "RESERVEDNAME", "$RESERVEDNAME");
        _fields[10] = new(IsRevenue_0NZM_FieldName, "ISREVENUE", "$ISREVENUE");
        _fields[11] = new(IsDeemedPositive_WPPA_FieldName, "ISDEEMEDPOSITIVE", "$ISDEEMEDPOSITIVE");
        _fields[12] = new(AffectGrossProfit_O1S3_FieldName, "AFFECTSGROSSPROFIT", "$AFFECTSGROSSPROFIT");
        _fields[13] = new(IsSubledger_PNHX_FieldName, "ISSUBLEDGER", "$ISSUBLEDGER");
        _fields[14] = new(SortPosition_9H2I_FieldName, "SORTPOSITION", "$SORTPOSITION");
        _fields[15] = new(AddlAllocType_X6BO_FieldName, "ADDLALLOCTYPE", "$ADDLALLOCTYPE");
        _fields[16] = new(IsCalculable_0LSO_FieldName, "BASICGROUPISCALCULABLE", "$BASICGROUPISCALCULABLE");
        _fields[17] = new(IsAddable_FOD7_FieldName, "ISADDABLE", "$ISADDABLE");
        _fields[18] = new(NameList_ZIWC_FieldName, "NAME", "$NAME");
        _fields[19] = new(LanguageId_XANV_FieldName, "LANGUAGEID", "$LANGUAGEID");
        return _fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, "GROUP", nativeFields: [..GetFetchList()]);
        return collections;
    }

    internal static string[] GetFetchList()
    {
        return["REMOTEALTGUID", "ALTERID", "ENTEREDBY", "ALTEREDBY", "NAME", "Alias", "PARENT", "RESERVEDNAME", "ISREVENUE", "ISDEEMEDPOSITIVE", "AFFECTSGROSSPROFIT", "ISSUBLEDGER", "SORTPOSITION", "ADDLALLOCTYPE", "BASICGROUPISCALCULABLE", "ISADDABLE", "LanguageName.NAME,LanguageName.LANGUAGEID"];
    }
}