namespace UnitTests.AdvanceTests;

[TestClass]
public class TallyComplexObjectTests
{
    [TestMethod]
    public async Task TestComplexProperty()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using TallyConnector.Core.Models.TallyComplexObjects;
namespace UnitTests.TestBasic;

[ImplementTallyRequestableObject]
[TDLCollection(Type = ""Ledger"")]
public partial class Ledger
{
    [XmlElement(ElementName = ""NAME"")]
    public string Name { get; set; }
    [XmlElement(ElementName = ""PARENT"")]
    public string Parent { get; set; }

    [XmlElement(ElementName = ""OPENINGBALANCE"")]
    public TallyAmountField OpeningBalance { get; set; }
}
";
        await VerifyTDLReport.VerifyGeneratorAsync(src,
           ("UnitTests.TestBasic.Ledger.cs", @"using TallyConnector.Core.Extensions;
using static TallyConnector.Core.Constants;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Ledger
*/
partial class Ledger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string Name_39DV_FieldName = ""Name_39DV"";
    const string Parent_DR40_FieldName = ""Parent_DR40"";
    const string Amount_MSOZ_FieldName = ""Amount_MSOZ"";
    const string Currency_HDWD_FieldName = ""Currency_HDWD"";
    const string ForexAmount_LRH3_FieldName = ""ForexAmount_LRH3"";
    const string ForexCurrency_AQUH_FieldName = ""ForexCurrency_AQUH"";
    const string RateOfExchange_HQDQ_FieldName = ""RateOfExchange_HQDQ"";
    const string IsDebit_FSR4_FieldName = ""IsDebit_FSR4"";
    const string OpeningBalance_2OQW_PartName = ""OpeningBalance_2OQW"";
    const string ReportName = ""Ledger_BUL5"";
    const string CollectionName = ""LedgersCollection_BUL5"";
    const string XMLTag = ""LEDGER"";
    const int SimpleFieldsCount = 8;
    const int ComplexFieldsCount = 1;
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
        tdlMsg.Functions = [TallyConnector.Core.Constants.DefaultFunctions.GetBoolFunction()];
        return reqEnvelope;
    }

    public static global::System.Xml.Serialization.XmlAttributeOverrides GetXMLAttributeOverides()
    {
        var xmlAttributeOverrides = new global::System.Xml.Serialization.XmlAttributeOverrides();
        var XmlAttributes = new global::System.Xml.Serialization.XmlAttributes();
        XmlAttributes.XmlElements.Add(new(XMLTag));
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::UnitTests.TestBasic.Ledger>.TypeInfo, ""Objects"", XmlAttributes);
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
        parts[0] = new(OpeningBalance_2OQW_PartName, null);
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [Name_39DV_FieldName,Parent_DR40_FieldName], XMLTag)
        {
            Explode = [$""{OpeningBalance_2OQW_PartName}:{string.Format(""NOT $$IsEmpty:{0}"", ""$OPENINGBALANCE"")}""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(OpeningBalance_2OQW_PartName, [Amount_MSOZ_FieldName,Currency_HDWD_FieldName,ForexAmount_LRH3_FieldName,ForexCurrency_AQUH_FieldName,RateOfExchange_HQDQ_FieldName,IsDebit_FSR4_FieldName], ""OPENINGBALANCE"")
        {
            Local = [$""Field:{Amount_MSOZ_FieldName}:Set:$$BaseValue:$OPENINGBALANCE"", $""Field:{Currency_HDWD_FieldName}:Set:$CurrencyName:Company:##SVCurrentCompany"", $""Field:{ForexAmount_LRH3_FieldName}:Set:$$ForexValue:$OPENINGBALANCE"", $""Field:{ForexCurrency_AQUH_FieldName}:Set:$FOREXSYMBOL"", $""Field:{RateOfExchange_HQDQ_FieldName}:Set:$$RatexValue:$OPENINGBALANCE"", $""Field:{IsDebit_FSR4_FieldName}:Set:$$TC_GetBooleanFromLogicField:$$IsDebit:$OPENINGBALANCE""]
        };
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_39DV_FieldName, ""NAME"", ""$NAME"");
        _fields[1] = new(Parent_DR40_FieldName, ""PARENT"", ""$PARENT"");
        _fields[2] = new(Amount_MSOZ_FieldName, ""AMOUNT"")
        {
            Type = ""Number""
        };
        _fields[3] = new(Currency_HDWD_FieldName, ""CURRENCY"")
        {
            Invisible = ""$$ISEmpty:$$value""
        };
        _fields[4] = new(ForexAmount_LRH3_FieldName, ""FOREXAMOUNT"")
        {
            Type = ""Number"",
            Invisible = ""$$Value=#Amount_C0LF""
        };
        _fields[5] = new(ForexCurrency_AQUH_FieldName, ""FOREXSYMBOL"")
        {
            Type = ""Amount : Rate"",
            Format = ""Forex,Currency"",
            Invisible = ""#ForexAmount_MT3L=#Amount_C0LF""
        };
        _fields[6] = new(RateOfExchange_HQDQ_FieldName, ""RATEOFEXCHANGE"")
        {
            Type = ""Number"",
            Invisible = ""$$Value=1""
        };
        _fields[7] = new(IsDebit_FSR4_FieldName, ""ISDEBIT"");
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
        return[""NAME"", ""PARENT"", ""OPENINGBALANCE""];
    }
}"));
    }
}
