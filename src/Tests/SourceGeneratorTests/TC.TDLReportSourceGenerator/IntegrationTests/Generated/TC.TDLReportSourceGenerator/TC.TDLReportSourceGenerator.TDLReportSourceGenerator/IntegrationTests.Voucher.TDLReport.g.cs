using TallyConnector.Core.Extensions;

#nullable enable
namespace IntegrationTests;
public partial class Voucher
{
    const string VoucherNumberTDLFieldName = "TC_Voucher_VoucherNumber";
    internal const string ReportName = "TC_VoucherList";
    const string _collectionName = "TC_VoucherCollection";
    public static global::TallyConnector.Core.Models.RequestEnvelope GetRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, ReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(ReportName)];
        tdlMsg.Form = [new(ReportName)];
        tdlMsg.Part = [..global::IntegrationTests.Voucher.GetTDLParts()];
        tdlMsg.Line = [..global::IntegrationTests.Voucher.GetTDLLines()];
        tdlMsg.Field = [..global::IntegrationTests.Voucher.GetTDLFields()];
        tdlMsg.Collection = [..global::IntegrationTests.Voucher.GetTDLCollections()];
        return reqEnvelope;
    }

    public static global::TallyConnector.Core.Models.Part[] GetTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Part[2];
        parts[0] = new(ReportName, _collectionName);
        var ledgerEntriesParts = global::IntegrationTests.LedgerEntry.GetTDLParts();
        parts.AddToArray(ledgerEntriesParts, 1);
        return parts;
    }

    public static global::TallyConnector.Core.Models.Line[] GetTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[2];
        lines[0] = new(ReportName, [VoucherNumberTDLFieldName], "VOUCHER")
        {
            Explode = [$"{IntegrationTests.LedgerEntry.ReportName}:Yes"]
        };
        var ledgerEntriesLines = global::IntegrationTests.LedgerEntry.GetTDLLines();
        lines.AddToArray(ledgerEntriesLines, 1);
        return lines;
    }

    public static global::TallyConnector.Core.Models.Field[] GetTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[2];
        fields[0] = new(VoucherNumberTDLFieldName, "VOUCHERNUMBER", "$VOUCHERNUMBER");
        var ledgerEntriesFields = global::IntegrationTests.LedgerEntry.GetTDLFields();
        fields.AddToArray(ledgerEntriesFields, 1);
        return fields;
    }

    public static global::TallyConnector.Core.Models.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(_collectionName, "VOUCHER", nativeFields: ["*"]);
        return collections;
    }
}