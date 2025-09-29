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

    public const string XmlAttributeOverridesClassName = $"{TallyConnectorModelsNameSpace}.XMLOverrideswithTracking";
    public const string XmlAttributesClassName = "System.Xml.Serialization.XmlAttributes";
    public const string CollectionsNameSpace = "System.Collections.Generic";
    public const string ListClassName = "List";
    public const string DictionaryClassName = "Dictionary";
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
    public const string GenerateHelperMethodGenericAttrName = $"{TallyConnectorCoreAttributesNameSpace}.GenerateHelperMethodAttribute`1";
    public const string GenerateHelperMethodAttrName = $"{TallyConnectorCoreAttributesNameSpace}.GenerateHelperMethodAttribute";
    public const string ImplementTallyServiceAttrName = $"{TallyConnectorCoreAttributesNameSpace}.SourceGenerator.ImplementTallyService";

    public const string TallyConnectorCoreNameSpace = "TallyConnector.Core";
    public const string TallyConnectorModelsNameSpace = $"{TallyConnectorCoreNameSpace}.Models";
    public const string TallyConnectorCommonModelsNameSpace = $"{TallyConnectorModelsNameSpace}.Common";
    public const string YesEnum = $"{TallyConnectorModelsNameSpace}.YesNo.Yes";
    public const string NoEnum = $"{TallyConnectorModelsNameSpace}.YesNo.No";
    public const string TallyConnectorRequestModelsNameSpace = $"{TallyConnectorModelsNameSpace}.Request";
    public const string TallyConnectorResponseModelsNameSpace = $"{TallyConnectorModelsNameSpace}.Response";
    public const string PostResponseFullName = $"{TallyConnectorResponseModelsNameSpace}.PostResponse";
    public const string TallyConnectorCoreAttributesNameSpace = $"{TallyConnectorCoreNameSpace}.Attributes";
    public const string TallyConnectorResponseNameSpace = $"{TallyConnectorModelsNameSpace}.Common.Response";
    public const string TallyConnectorPaginationNameSpace = $"TallyConnector.Models.Common.Pagination";
    public const string ReportResponseEnvelopeClassName = "ReportResponseEnvelope";
    public const string FieldFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.Field";

    public const string PostRequestOptionsFullName = $"{TallyConnectorRequestModelsNameSpace}.PostRequestOptions";
    public const string PartTypeName = "Part";
    public const string PartFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.{PartTypeName}";

    public const string LineTypeName = $"Line";

    public const string LineFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.{LineTypeName}";
    public const string CollectionFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.Collection";
    public const string TDLFunctionFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.TDLFunction";
    public const string FilterFullTypeName = $"{TallyConnectorModelsNameSpace}.Filter";
    public const string TDLNameSetFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.NameSet";
    public const string LanguageNamesFullTypeName = $"{TallyConnectorModelsNameSpace}.LanguageNameList";
    public const string LicenseInfoFullTypeName = $"{TallyConnectorModelsNameSpace}.{LicenseInfoPropertyName}";

    public const string PaginatedResponseClassName = $"PaginatedResponse";


    public const string BaseLedgerEntryInterfaceName = $"{TallyConnectorModelsNameSpace}.Interfaces.Voucher.IBaseLedgerEntry";
    public const string TallyObjectDTOInterfaceName = $"{TallyConnectorModelsNameSpace}.Interfaces.IBaseTallyObjectDTO";
    public const string TallyObjectDTOName = $"{TallyConnectorModelsNameSpace}.TallyObjectDTO";



    public const string HeaderTypeEnumName = $"{TallyConnectorRequestModelsNameSpace}.HType";
    public const string RequestTypeEnumName = $"{TallyConnectorModelsNameSpace}.Request.RequestType";
    public const string TallyResponseEnvelopeTypeName = $"PostRequestEnvelope";
    public const string RequestEnvelopeFullTypeName = $"{TallyConnectorRequestModelsNameSpace}.RequestEnvelope";
    public const string ActionEnumFullTypeName = $"{TallyConnectorModelsNameSpace}.Action";
    public const string TallyEnvelopeFullTypeName = $"{TallyConnectorModelsNameSpace}.Response.ResponseEnvelope";
    public const string PostResultFullTypeName = $"{TallyConnectorModelsNameSpace}.Response.PostResult";
    public const string PostResultsFullTypeName = $"{TallyConnectorModelsNameSpace}.Response.PostResults";
    public const string ReportFullTypeName = $"{TallyConnectorModelsNameSpace}.Report";
    public const string BaseCompanyTypeName = $"{TallyConnectorModelsNameSpace}.BaseCompany";
    public const string FormFullTypeName = $"{TallyConnectorModelsNameSpace}.Form";
    public const string ExtensionsNameSpace = "TallyConnector.Core.Extensions";
    public const string TDLCollectionAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.TDLCollectionAttribute";
    public const string IgnoreForCreateDTOAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.IgnoreForCreateDTOAttribute";
    public const string TDLFieldAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.TDLFieldAttribute";
    public const string MaptoDTOAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.MaptoDTOAttribute";

    public const string TDLFunctionsMethodNameAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.TDLFunctionsMethodNameAttribute";
    public const string TDLCollectionsMethodNameAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.TDLCollectionsMethodNameAttribute";
    public const string TDLFiltersMethodNameAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.TDLFiltersMethodNameAttribute";
    public const string TDLComputeMethodNameAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.TDLComputeMethodNameAttribute";
    public const string TDLNamesetMethodNameAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.TDLNamesetMethodNameAttribute";
    public const string TDLObjectsMethodNameAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.TDLObjectsMethodNameAttribute";
    public const string TDLDefaultFiltersMethodNameAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.TDLDefaultFiltersMethodNameAttribute";
    public const string EnumChoiceAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.EnumXMLChoiceAttribute";
    public const string ActivitySourceAttributeName = $"{TallyConnectorCoreAttributesNameSpace}.SetActivitySourceAttribute";




    public const string GetObjectsMethodName = "Get{0}Async";
    public const string PostObjectsMethodName = "Post{0}Async";

    public const string GetXMLAttributeOveridesMethodName = "Get{0}XMLAttributeOverides";
    public const string GetRequestEnvelopeMethodName = "Get{0}RequestEnvelope";
    public const string GetPostRequerstEnvelopeMethodName = "GetPostRequestEnvelope";
    public const string GetTDLPartsMethodName = "Get{0}TDLParts";
    public const string GetMainTDLPartMethodName = "Get{0}MainTDLPart";
    public const string GetMainTDLLineMethodName = "Get{0}MainTDLLine";
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

    public const string GetBooleanFromLogicFieldMethodName = $"{TallyConnectorCoreNameSpace}.Constants.DefaultFunctions.GetBoolFunction";
    public const string GetTransformDateToXSDMethodName = $"{TallyConnectorCoreNameSpace}.Constants.DefaultFunctions.GetDateFunction";


    public const string StartActivityMethodName = "StartActivity";

    public const string DTOTypeInfoFieldName = "DTOTypeInfo";

    public const string TallyBaseClientFullName = "TallyConnector.Services.TallyAbstractClient";

    public const string DateOnlyType = "System.DateOnly";

    public const string SimpleFieldsCountFieldName = "SimpleFieldsCount";
    public const string ComplexFieldsCountFieldName = "ComplexFieldsCount";

    public static List<string> DefaultSimpleTypes = [DateOnlyType];



    public class Models
    {
        public const string BaseAliasedMasterObjectFullName = $"TallyConnector.Models.Base.Masters.BaseAliasedMasterObject";
        public const string BaseMasterObjectFullName = $"TallyConnector.Models.Base.Masters.BaseMasterObject";
        public class Interfaces
        {
            public const string PREFIX = $"{TallyConnectorModelsNameSpace}.Interfaces";
            public const string TallyRequestableObjectInterfaceFullName = $"{PREFIX}.ITallyRequestableObject";


        }
        public class Abstractions
        {
            public const string PREFIX = "TallyConnector.Abstractions.Models";
            public const string MetaObjectypeName = $"MetaObject";
            public const string PropertyMetaDataTypeName = $"PropertyMetaData";
            public const string IMetaGeneratedTypeName = $"IMetaGenerated";
            public const string IMetaGeneratedFullTypeName = $"{PREFIX}.IMetaGenerated";
        }
        public class Response
        {
            public const string PREFIX = $"{TallyConnectorModelsNameSpace}.Response";
            public const string ReportResponseEnvelopeClassName = $"{PREFIX}.ReportResponseEnvelope";
        }
    }
    public class Attributes
    {
        public const string PREFIX = TallyConnectorCoreAttributesNameSpace;

        public class SourceGenerator
        {
            public const string PREFIX = $"{TallyConnectorCoreAttributesNameSpace}.SourceGenerator";
            public const string ImplementTallyRequestableObjectAttribute = $"{PREFIX}.ImplementTallyRequestableObjectAttribute";

        }
        public class Abstractions
        {
            public const string PREFIX = "TallyConnector.Abstractions.Attributes";
            public const string GenerateMetaAttributeName = $"{PREFIX}.GenerateMetaAttribute";
            public const string GenerateITallyRequestableObectAttributeeName = $"{PREFIX}.GenerateITallyRequestableObectAttribute";

        }

    }
    public class MultiXMLMeta
    {
        public const string MetasVarName = "_metas";
        public const string OriginalPrefixVarName = "_orgPrefix";


        public const string GeneratePathMethodName = "GenerateMultiXMLPath";

        public class Parameters
        {
            public const string OriginalPrefix = "orgPrefix";
        }
    }
    public class Meta
    {
        public const string Name = "Meta";


        public const string PathPrefixVarName = "_pathPrefix";

        public const string IdentifierNameVarName = "IdentifierName";
        public const string InstanceVarName = "Instance";
        public const string ReportVarName = "TDLReportName";
        public const string CollectionNameVarName = "TDLDefaultCollectionName";
        public const string TDLDefaultPartVarName = "TDLDefaultPart";
        public const string TDLDefaultLineVarName = "TDLDefaultLine";
        public const string ObjectTypeVarName = "TallyObjectType";

        public const string PartVarName = "Part";

        public const string XMLTagVarName = "XMLTag";

        public const string AllPartsVarName = "AllParts";
        public const string AllLinesVarName = "AllLines";
        public const string FieldsVarName = "Fields";
        public const string FieldNamesVarName = "FieldNames";
        public const string AllFetchTextVarName = "AllFetchList";
        public const string DefaultCollectionVarName = "DefaultCollection";
        public const string ExplodesVarName = "Explodes";
        public const string NameSetsVarName = "NameSets";

        public const string AllPartsPropPath = $"{Name}.{AllPartsVarName}";
        public const string AllLinesPropPath = $"{Name}.{AllLinesVarName}";
        public const string FieldsPropPath = $"{Name}.{FieldsVarName}";
        public const string CollectionNamePropPath = $"{Name}.{CollectionNameVarName}";
        public const string ObjectTypePropPath = $"{Name}.{ObjectTypeVarName}";
        public const string AllFetchTextPropPath = $"{Name}.{AllFetchTextVarName}";
        public const string DefaultCollectionPropPath = $"{Name}.{DefaultCollectionVarName}";

        public const string TDLDefaultLinePropPath = $"{Name}.{TDLDefaultLineVarName}";
        public const string TDLDefaultPartPropPath = $"{Name}.{TDLDefaultPartVarName}";

        public const string IdentifierPropPath = $"{Name}.{IdentifierNameVarName}";

        public const string NameSetsPropPath = $"{Name}.{NameSetsVarName}";


        public class Parameters
        {
            public const string Name = "name";
            public const string XMLTag = "xmlTag";
            public const string PathPrefix = "pathPrefix";
        }
    }
}
