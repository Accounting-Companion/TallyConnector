namespace TC.TDLReportSourceGenerator;
public static class Constants
{
    /// <summary>
    /// TDL Report is Generated for every object that inherits this Interface
    /// </summary>
    public const string BaseInterfaceName = "TallyConnector.Core.Models.ITallyBaseObject";
    public const string XMLElementAttributeName = "System.Xml.Serialization.XmlElement";
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




    public const string GetRequestEnvelopeMethodName = "GetRequestEnevelope";
    public const string GetTDLReportsMethodName = "GetTDLReports";
    public const string GetTDLFormsMethodName = "GetTDLForms";
    public const string GetTDLPartsMethodName = "GetTDLParts";
    public const string GetTDLLinesMethodName = "GetTDLLines";
    public const string GetTDLCollectionsMethodName = "GetTDLCollections";
    public const string GetTDLFieldsMethodName = "GetTDLFields";

    public const string AddToArrayExtensionMethodName = "AddToArray";
}
