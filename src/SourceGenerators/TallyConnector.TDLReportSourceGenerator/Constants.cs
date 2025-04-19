namespace TallyConnector.TDLReportSourceGenerator;
public static class Constants
{
    /// <summary>
    /// TDL Report is Generated for every object that inherits this Interface
    /// </summary>
    public const string BaseInterfaceName = $"{TallyConnectorModelsNameSpace}.ITallyBaseObject";
    public const string BaseObjectName = $"{TallyConnectorModelsNameSpace}.Base.BaseTallyObject";
    public const string MasterObjectInterfaceName = $"{TallyConnectorModelsNameSpace}.Interfaces.IBaseMasterObject";
    public const string VoucherObjectInterfaceName = $"{TallyConnectorModelsNameSpace}.Interfaces.IBaseVoucherObject";
    public const string TallyComplexObjectInterfaceName = $"{TallyConnectorModelsNameSpace}.TallyComplexObjects.ITallyComplexObject";
    public const string TallyRateClassName = $"{TallyConnectorModelsNameSpace}.TallyComplexObjects.TallyRateField";
    public const string TallyAmountClassName = $"{TallyConnectorModelsNameSpace}.TallyComplexObjects.TallyAmountField";
    public const string BaseClassName = "TallyConnector.Services.BaseTallyService";
    public const string XMLToObjectClassName = "TallyConnector.Services.XMLToObject";
    public const string PaginatedRequestOptionsClassName = $"{TallyConnectorModelsNameSpace}.Request.PaginatedRequestOptions";
    public const string RequestOptionsClassName = $"{TallyConnectorModelsNameSpace}.Request.RequestOptions";
    public const string GetObjfromXmlMethodName = "GetObjfromXml";
    public const string PopulateOptionsExtensionMethodName = "PopulateOptions";
    public const string PopulateDefaultOptionsMethodName = "PopulateDefaultOptions";

    public const string SendRequestMethodName = "SendRequestAsync";

    public const string XmlAttributeOverridesClassName = "System.Xml.Serialization.XmlAttributeOverrides";
    public const string XmlAttributesClassName = "System.Xml.Serialization.XmlAttributes";
    public const string CollectionsNameSpace = "System.Collections.Generic";
    public const string ListClassName = "List";
    public const string IEnumerableClassName = "IEnumerable";

    public const string IEnumerableInterfaceName = "System.Collections.IEnumerable";
    public const string XmlRootAttributeName = "System.Xml.Serialization.XmlRootAttribute";
    public const string XMLElementAttributeName = "System.Xml.Serialization.XmlElementAttribute";
    public const string XMLAttributeAttributeName = "System.Xml.Serialization.XmlAttributeAttribute";
    public const string XMLEnumAttributeName = "System.Xml.Serialization.XmlEnumAttribute";

    public const string XMLArrayItemAttributeName = "System.Xml.Serialization.XmlArrayItemAttribute";
    public const string XMLArrayAttributeName = "System.Xml.Serialization.XmlArrayAttribute";
    public const string XmlIgnoreAttributeName = "System.Xml.Serialization.XmlIgnoreAttribute";


    public const string CancellationTokenStructName = "System.Threading.CancellationToken";
    public const string GenerateHelperMethodAttrName = $"{TallyConnectorAttributesNameSpace}.GenerateHelperMethodAttribute";
    public const string ImplementTallyServiceAttrName = $"{TallyConnectorAttributesNameSpace}.SourceGenerator.ImplementTallyService";

    public const string TallyConnectorNameSpace = "TallyConnector.Core";
    public const string TallyConnectorModelsNameSpace = $"{TallyConnectorNameSpace}.Models";
    public const string TallyConnectorCommonModelsNameSpace = $"{TallyConnectorModelsNameSpace}.Common";
    public const string YesEnum = $"{TallyConnectorModelsNameSpace}.Common.YesNo.Yes";
    public const string NoEnum = $"{TallyConnectorModelsNameSpace}.Common.YesNo.No";
    public const string TallyConnectorRequestModelsNameSpace = $"{TallyConnectorModelsNameSpace}.Request";
    public const string TallyConnectorResponseModelsNameSpace = $"{TallyConnectorModelsNameSpace}.Response";
    public const string TallyConnectorAttributesNameSpace = $"{TallyConnectorNameSpace}.Attributes";
    public const string TallyConnectorResponseNameSpace = $"{TallyConnectorModelsNameSpace}.Common.Response";
    public const string TallyConnectorPaginationNameSpace = $"{TallyConnectorModelsNameSpace}.Common.Pagination";
    public const string ReportResponseEnvelopeClassName = "ReportResponseEnvelope";
    public const string FieldFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.Field";
    public const string PartFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.Part";
    public const string LineFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.Line";
    public const string CollectionFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.Collection";
    public const string TDLFunctionFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.TDLFunction";
    public const string FilterFullTypeName = $"{TallyConnectorModelsNameSpace}.Filter";
    public const string TDLNameSetFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.NameSet";
    public const string LanguageNamesFullTypeName = $"{TallyConnectorModelsNameSpace}.LanguageNameList";
    public const string LicenseInfoFullTypeName = $"{TallyConnectorCommonModelsNameSpace}.{LicenseInfoPropertyName}";

    public const string PaginatedResponseClassName = $"PaginatedResponse";


    public const string BaseLedgerEntryInterfaceName = $"{TallyConnectorModelsNameSpace}.Interfaces.Voucher.IBaseLedgerEntry";
    public const string TallyObjectDTOInterfaceName = $"{TallyConnectorModelsNameSpace}.Interfaces.IBaseTallyObjectDTO";



    public const string HeaderTypeEnumName = $"{TallyConnectorRequestModelsNameSpace}.HType";
    public const string RequestTypeEnumName = $"{TallyConnectorModelsNameSpace}.Request.RequestType";
    public const string TallyResponseEnvelopeTypeName = $"PostRequestEnvelope";
    public const string RequestEnvelopeFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.RequestEnvelope";
    public const string ActionEnumFullTypeName = $"{TallyConnectorCommonModelsNameSpace}.Action";
    public const string TallyEnvelopeFullTypeName = $"{TallyConnectorModelsNameSpace}.Response.ResponseEnvelope";
    public const string PostResultFullTypeName = $"{TallyConnectorModelsNameSpace}.Response.PostResult";
    public const string PostResultsFullTypeName = $"{TallyConnectorModelsNameSpace}.Response.PostResults";
    public const string ReportFullTypeName = $"{TallyConnectorModelsNameSpace}.Report";
    public const string BaseCompanyTypeName = $"{TallyConnectorModelsNameSpace}.BaseCompany";
    public const string FormFullTypeName = $"{TallyConnectorModelsNameSpace}.Form";
    public const string ExtensionsNameSpace = "TallyConnector.Core.Extensions";
    public const string TDLCollectionAttributeName = $"{TallyConnectorAttributesNameSpace}.TDLCollectionAttribute";
    public const string IgnoreForCreateDTOAttributeName = $"{TallyConnectorAttributesNameSpace}.IgnoreForCreateDTOAttribute";
    public const string TDLFieldAttributeName = $"{TallyConnectorAttributesNameSpace}.TDLFieldAttribute";
    public const string MaptoDTOAttributeName = $"{TallyConnectorAttributesNameSpace}.MaptoDTOAttribute";

    public const string TDLFunctionsMethodNameAttributeName = $"{TallyConnectorAttributesNameSpace}.TDLFunctionsMethodNameAttribute";
    public const string TDLCollectionsMethodNameAttributeName = $"{TallyConnectorAttributesNameSpace}.TDLCollectionsMethodNameAttribute";
    public const string TDLFiltersMethodNameAttributeName = $"{TallyConnectorAttributesNameSpace}.TDLFiltersMethodNameAttribute";
    public const string TDLComputeMethodNameAttributeName = $"{TallyConnectorAttributesNameSpace}.TDLComputeMethodNameAttribute";
    public const string TDLNamesetMethodNameAttributeName = $"{TallyConnectorAttributesNameSpace}.TDLNamesetMethodNameAttribute";
    public const string TDLObjectsMethodNameAttributeName = $"{TallyConnectorAttributesNameSpace}.TDLObjectsMethodNameAttribute";
    public const string TDLDefaultFiltersMethodNameAttributeName = $"{TallyConnectorAttributesNameSpace}.TDLDefaultFiltersMethodNameAttribute";
    public const string EnumChoiceAttributeName = $"{TallyConnectorAttributesNameSpace}.EnumXMLChoiceAttribute";
    public const string ActivitySourceAttributeName = $"{TallyConnectorAttributesNameSpace}.SetActivitySourceAttribute";




    public const string GetObjectsMethodName = "Get{0}Async";
    public const string PostObjectsMethodName = "Post{0}Async";

    public const string GetXMLAttributeOveridesMethodName = "Get{0}XMLAttributeOverides";
    public const string GetRequestEnvelopeMethodName = "Get{0}RequestEnevelope";
    public const string GetPostRequerstEnvelopeMethodName = "GetPostRequestEnvelope";
    public const string GetTDLPartsMethodName = "Get{0}TDLParts";
    public const string GetMainTDLPartMethodName = "Get{0}MainTDLPart";
    public const string GetTDLLinesMethodName = "Get{0}TDLLines";
    public const string GetTDLCollectionsMethodName = "Get{0}TDLCollections";
    public const string GetFetchListMethodName = "Get{0}FetchList";
    public const string GetTDLFunctionsMethodName = "Get{0}TDLFunctions";
    public const string GetTDLNameSetsMethodName = "Get{0}TDLNameSets";
    public const string GetTDLObjectsMethodName = "Get{0}TDLObjects";
    public const string GetTDLFieldsMethodName = "Get{0}TDLFields";
    public const string GetEnumFunctionName = "TC_Get{0}";
    public const string GetEnumNameSetName = "TC_{0}Enum";
    public const string GetDefaultTDLFunctionsMethodName = "GetDefaultTDLFunctions";
    public const string setValueParamName = "setValue";
    public const string ParseReportMethodName = "ParseReportFromXML";

    public const string PostRequestEnvelopeMessageName = "{0}PostRequestEnvelopeMessage";

    public const string AddToArrayExtensionMethodName = "AddToArray";
    public const string GetTallyStringMethodName = "GetTallyString";
    public const string LicenseInfoPropertyName = "LicenseInfo";
    public const string ShortVersionPropertyName = "TallyShortVersion";
    public const string AddCustomResponseReportForPostMethodName = "AddCustomResponseReportForPost";

    public const string GetBooleanFromLogicFieldFunctionName = "TC_GetBooleanFromLogicField";
    public const string GetTransformDateToXSDFunctionName = "TC_TransformDateToXSD";


    public const string StartActivityMethodName = "StartActivity";

    public const string DateOnlyType = "System.DateOnly";

    public static List<string> DefaultSimpleTypes = [DateOnlyType];
}
