namespace TC.TDLReportSourceGenerator;
public static class Constants
{
    /// <summary>
    /// TDL Report is Generated for every object that inherits this Interface
    /// </summary>
    public const string BaseInterfaceName = "TallyConnector.Core.Models.ITallyBaseObject";
    public const string BaseClassName = "TallyConnector.Services.BaseTallyService";
    public const string GenerateHelperMethodAttrName = "TallyConnector.Core.Attributes.GenerateHelperMethodAttribute";
    public const string XMLElementAttributeName = "System.Xml.Serialization.XmlElementAttribute";
    public const string XMLArrayItemAttributeName = "System.Xml.Serialization.XmlArrayItemAttribute";
    public const string XMLArrayAttributeName = "System.Xml.Serialization.XmlArrayAttribute";
    public const string IEnumerableInterfaceName = "System.Collections.IEnumerable";

    public const string FieldFullTypeName = "TallyConnector.Core.Models.Field";
    public const string PartFullTypeName = "TallyConnector.Core.Models.Part";
    public const string LineFullTypeName = "TallyConnector.Core.Models.Line";
    public const string CollectionFullTypeName = "TallyConnector.Core.Models.Collection";

    public const string HeaderTypeEnumName = "TallyConnector.Core.Models.HType";
    public const string RequestEnvelopeFullTypeName = "TallyConnector.Core.Models.RequestEnvelope";
    public const string ReportFullTypeName = "TallyConnector.Core.Models.Report";
    public const string FormFullTypeName = "TallyConnector.Core.Models.Form";
    public const string ExtensionsNameSpace = "TallyConnector.Core.Extensions";
    public const string TDLXMLSetAttributeName = "TallyConnector.Core.Attributes.TDLXMLSetAttribute";
    public const string TDLCollectionAttributeName = "TallyConnector.Core.Attributes.TDLCollectionAttribute";




    public const string GetRequestEnvelopeMethodName = "Get{0}RequestEnevelope";
    public const string GetTDLPartsMethodName = "Get{0}TDLParts";
    public const string GetTDLLinesMethodName = "Get{0}TDLLines";
    public const string GetTDLCollectionsMethodName = "Get{0}TDLCollections";
    public const string GetTDLFieldsMethodName = "Get{0}TDLFields";

    public const string AddToArrayExtensionMethodName = "AddToArray";
}
