namespace UnitTests.AdvanceTests;
[TestClass]
public class CutomFunctionAttributeTests
{
    [TestMethod]
    public async Task TestAddCustomFunctiontoEnvelope()
    {

        var src = @"using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using static TallyConnector.Core.Constants;
using TallyConnector.Core.Models.Request;

namespace TallyConnector.Core.Models.Temp
{

    [TDLCollection(Type = ""Ledger"")]
    [XmlRoot(""LEDGER"")]
    [XmlType(AnonymousType = true)]
    [ImplementTallyRequestableObject]
    [TDLFunctionsMethodName(nameof(TC_TLedgerFunctions))]
    public partial class TLedger
    {
        public string Name { get; set; }

        public static TDLFunction[] TC_TLedgerFunctions()
        {
            return [new TDLFunction(""GJTF"")];
        }

    }


}";
        await VerifyTDLReport.VerifyGeneratorAsync(src, [
            ("TallyConnector.Core.Models.Temp.TLedger.cs",@"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Core.Models.Temp;
/*
* Generated based on TallyConnector.Core.Models.Temp.TLedger
*/
partial class TLedger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string Name_9QSD_FieldName = ""Name_9QSD"";
    const string ReportName = ""TLedger_R1ZB"";
    const string CollectionName = ""TLedgersCollection_R1ZB"";
    const string XMLTag = ""LEDGER"";
    const int SimpleFieldsCount = 1;
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
        tdlMsg.Functions = [..TC_TLedgerFunctions()];
        return reqEnvelope;
    }

    public static global::System.Xml.Serialization.XmlAttributeOverrides GetXMLAttributeOverides()
    {
        var xmlAttributeOverrides = new global::System.Xml.Serialization.XmlAttributeOverrides();
        var XmlAttributes = new global::System.Xml.Serialization.XmlAttributes();
        XmlAttributes.XmlElements.Add(new(XMLTag));
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::TallyConnector.Core.Models.Temp.TLedger>.TypeInfo, ""Objects"", XmlAttributes);
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
        return new(ReportName, [Name_9QSD_FieldName], XMLTag);
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        return[];
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_9QSD_FieldName, ""NAME"", ""$Name"");
        return _fields;
    }

    public static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, ""Ledger"", nativeFields: [..GetFetchList()]);
        return collections;
    }

    public static string[] GetFetchList()
    {
        return[""Name""];
    }
}")]);

    }
}

