namespace UnitTests.BasicTests;
[TestClass]
public class TallyComplexObjectTests
{
    [TestMethod]
    public async Task TestAmount()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
namespace TestNameSpace;
[TallyConnector.Core.Attributes.GenerateHelperMethod<Ledger>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public partial class Ledger : TallyConnector.Core.Models.ITallyBaseObject
{

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

    [XmlElement(ElementName = ""ClosingBalance"")]
    public TallyConnector.Core.Models.TallyComplexObjects.Amount? ClosingBalance { get; set; }

}";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
partial class TallyService
{
    internal const string LedgerNameTDLFieldName = ""TC_Ledger_Name"";
    internal const string LedgerReportName = ""TC_LedgerList"";
    const string Ledger_collectionName = ""TC_LedgerCollection"";
    internal static global::TallyConnector.Core.Models.RequestEnvelope GetLedgerRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, LedgerReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(LedgerReportName)];
        tdlMsg.Form = [new(LedgerReportName)];
        tdlMsg.Part = [..global::TestNameSpace.TallyService.GetLedgerTDLParts()];
        tdlMsg.Line = [..global::TestNameSpace.TallyService.GetLedgerTDLLines()];
        tdlMsg.Field = [..global::TestNameSpace.TallyService.GetLedgerTDLFields()];
        tdlMsg.Collection = [..global::TestNameSpace.TallyService.GetLedgerTDLCollections()];
        tdlMsg.Functions = [..global::TestNameSpace.TallyService.GetDefaultTDLFunctions(), ..global::TallyConnector.Core.Models.TallyComplexObjects.Amount.GetTDLFunctions()];
        return reqEnvelope;
    }

    internal static global::TallyConnector.Core.Models.Part[] GetLedgerTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Part[2];
        parts[0] = new(LedgerReportName, Ledger_collectionName);
        var closingBalanceParts = global::TestNameSpace.TallyService.GetAmountTDLParts(""TC_LedgerClosingBalanceList"", null);
        parts.AddToArray(closingBalanceParts, 1);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetLedgerTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[2];
        lines[0] = new(LedgerReportName, [LedgerNameTDLFieldName], ""LEDGER"")
        {
            Explode = [$""TC_LedgerClosingBalanceList:NOT $$IsEmpty:$ClosingBalance""]
        };
        var closingBalanceLines = global::TestNameSpace.TallyService.GetAmountTDLLines(""ClosingBalance"");
        lines.AddToArray(closingBalanceLines, 1);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetLedgerTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[6];
        fields[0] = new(LedgerNameTDLFieldName, ""NAME"", ""$NAME"");
        var closingBalanceFields = global::TestNameSpace.TallyService.GetAmountTDLFields(""$ClosingBalance"");
        fields.AddToArray(closingBalanceFields, 1);
        return fields;
    }

    internal static global::TallyConnector.Core.Models.Collection[] GetLedgerTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(Ledger_collectionName, ""LEDGER"", nativeFields: [""*""]);
        return collections;
    }
}";

        var resp2 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
partial class TallyService
{
    internal const string AmountBaseAmountTDLFieldName = ""TC_Amount_BaseAmount"";
    internal const string AmountForexAmountTDLFieldName = ""TC_Amount_ForexAmount"";
    internal const string AmountForexSymbolTDLFieldName = ""TC_Amount_ForexSymbol"";
    internal const string AmountExchangeRateTDLFieldName = ""TC_Amount_ExchangeRate"";
    internal const string AmountIsDebitTDLFieldName = ""TC_Amount_IsDebit"";
    internal const string AmountReportName = ""TC_AmountList"";
    const string Amount_collectionName = ""Amount"";
    internal static global::TallyConnector.Core.Models.Part[] GetAmountTDLParts(string partName = AmountReportName, string? collectionName = Amount_collectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = new(partName, collectionName, AmountReportName)
        {
            XMLTag = xmlTag
        };
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetAmountTDLLines(string xmlTag = ""AMOUNT"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(AmountReportName, [AmountBaseAmountTDLFieldName,AmountForexAmountTDLFieldName,AmountForexSymbolTDLFieldName,AmountExchangeRateTDLFieldName,AmountIsDebitTDLFieldName], xmlTag);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetAmountTDLFields(string setValue)
    {
        var fields = new global::TallyConnector.Core.Models.Field[5];
        fields[0] = new(AmountBaseAmountTDLFieldName, ""BASEAMOUNT"", string.Format(""$$BaseValue:{0}"", setValue))
        {
            Type = ""Amount : Base""
        };
        fields[1] = new(AmountForexAmountTDLFieldName, ""FOREXAMOUNT"", string.Format(""$$ForexValue:{0}"", setValue))
        {
            Type = ""Amount : Forex"",
            Invisible = ""$$Value=#TC_Amount_BaseAmount""
        };
        fields[2] = new(AmountForexSymbolTDLFieldName, ""FOREXSYMBOL"", string.Format(""{0}"", setValue))
        {
            Type = ""Amount : Rate"",
            Format = ""Forex,Currency"",
            Invisible = ""#TC_Amount_ForexAmount=#TC_Amount_BaseAmount""
        };
        fields[3] = new(AmountExchangeRateTDLFieldName, ""EXCHANGERATE"", string.Format(""$$RatexValue:{0}"", setValue))
        {
            Type = ""Number"",
            Invisible = ""$$Value=1""
        };
        fields[4] = new(AmountIsDebitTDLFieldName, ""ISDEBIT"", string.Format(""$$TC_GetBooleanFromLogicField:$$IsDebit:{0}"", setValue));
        return fields;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src, [("TestNameSpace.Group.TallyService.TDLReport.g.cs", resp1),
            ("TestNameSpace.Group.TallyService.TDLReport.g.cs", resp2)]);
    }
}

