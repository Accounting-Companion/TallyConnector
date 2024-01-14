namespace TC.TDLReportSourceGenerator;
public static class Constants
{
    /// <summary>
    /// TDL Report is Generated for every object that inherits this Interface
    /// </summary>
    public const string BaseInterfaceName = "TallyConnector.Core.Models.ITallyBaseObject";
    public const string TallyComplexObjectInterfaceName = "TallyConnector.Core.Models.TallyComplexObjects.ITallyComplexObject";
    public const string BaseClassName = "TallyConnector.Services.BaseTallyService";
    public const string XMLToObjectClassName = "TallyConnector.Services.XMLToObject";
    public const string GetObjfromXmlMethodName = "GetObjfromXml";

    public const string SendRequestMethodName = "SendRequestAsync";

    public const string XmlAttributeOverridesClassName = "System.Xml.Serialization.XmlAttributeOverrides";
    public const string XmlAttributesClassName = "System.Xml.Serialization.XmlAttributes";
    public const string CollectionsNameSpace = "System.Collections.Generic";
    public const string ListClassName = "List";

    public const string IEnumerableInterfaceName = "System.Collections.IEnumerable";
    public const string XMLElementAttributeName = "System.Xml.Serialization.XmlElementAttribute";
    public const string XMLEnumAttributeName = "System.Xml.Serialization.XmlEnumAttribute";
    public const string XMLArrayItemAttributeName = "System.Xml.Serialization.XmlArrayItemAttribute";
    public const string XMLArrayAttributeName = "System.Xml.Serialization.XmlArrayAttribute";
    public const string GenerateHelperMethodAttrName = "TallyConnector.Core.Attributes.GenerateHelperMethodAttribute";

    public const string TallyConnectorModelsNameSpace = "TallyConnector.Core.Models";
    public const string TallyConnectorResponseNameSpace = $"{TallyConnectorModelsNameSpace}.Common.Response";
    public const string ReportResponseEnvelopeClassName = "ReportResponseEnvelope";
    public const string FieldFullTypeName = "TallyConnector.Core.Models.Field";
    public const string PartFullTypeName = "TallyConnector.Core.Models.Part";
    public const string LineFullTypeName = "TallyConnector.Core.Models.Line";
    public const string CollectionFullTypeName = "TallyConnector.Core.Models.Collection";
    public const string TDLFunctionFullTypeName = "TallyConnector.Core.Models.TDLFunction";
    public const string TDLNameSetFullTypeName = "TallyConnector.Core.Models.NameSet";

    public const string HeaderTypeEnumName = "TallyConnector.Core.Models.HType";
    public const string RequestEnvelopeFullTypeName = "TallyConnector.Core.Models.RequestEnvelope";
    public const string ReportFullTypeName = "TallyConnector.Core.Models.Report";
    public const string FormFullTypeName = "TallyConnector.Core.Models.Form";
    public const string ExtensionsNameSpace = "TallyConnector.Core.Extensions";
    public const string TDLCollectionAttributeName = "TallyConnector.Core.Attributes.TDLCollectionAttribute";
    public const string TDLFieldAttributeName = "TallyConnector.Core.Attributes.TDLFieldAttribute";

    public const string TDLFunctionsMethodNameAttributeName = "TallyConnector.Core.Attributes.TDLFunctionsMethodNameAttribute";
    public const string TDLCollectionsMethodNameAttributeName = "TallyConnector.Core.Attributes.TDLCollectionsMethodNameAttribute";
    public const string TDLFiltersMethodNameAttributeName = "TallyConnector.Core.Attributes.TDLFiltersMethodNameAttribute";
    public const string TDLComputeMethodNameAttributeName = "TallyConnector.Core.Attributes.TDLComputeMethodNameAttribute";
    public const string TDLNamesetMethodNameAttributeName = "TallyConnector.Core.Attributes.TDLNamesetMethodNameAttribute";




    public const string GetObjectsMethodName = "Get{0}s";
    public const string GetRequestEnvelopeMethodName = "Get{0}RequestEnevelope";
    public const string GetTDLPartsMethodName = "Get{0}TDLParts";
    public const string GetTDLLinesMethodName = "Get{0}TDLLines";
    public const string GetTDLCollectionsMethodName = "Get{0}TDLCollections";
    public const string GetTDLFunctionsMethodName = "Get{0}TDLFunctions";
    public const string GetTDLNameSetsMethodName = "Get{0}TDLNameSets";
    public const string GetTDLFieldsMethodName = "Get{0}TDLFields";
    public const string GetEnumFunctionName = "TC_Get{0}";
    public const string GetEnumNameSetName = "TC_{0}Enum";
    public const string GetDefaultTDLFunctionsMethodName = "GetDefaultTDLFunctions";
    public const string setValueParamName = "setValue";

    public const string AddToArrayExtensionMethodName = "AddToArray";

    public const string GetBooleanFromLogicFieldFunctionName = "TC_GetBooleanFromLogicField";
    public const string GetTransformDateToXSDFunctionName = "TC_TransformDateToXSD";
}
