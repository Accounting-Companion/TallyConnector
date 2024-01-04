using TallyConnector.Core.Extensions;

#nullable enable
namespace IntegrationTests;
public partial class LedgerEntry
{
    const string LedgerNameTDLFieldName = "TC_LedgerEntry_LedgerName";
    internal const string ReportName = "TC_LedgerEntryList";
    const string _collectionName = "LedgerEntry";
    public static global::TallyConnector.Core.Models.Part[] GetTDLParts(string collectionName = _collectionName)
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = new(ReportName, collectionName);
        return parts;
    }

    public static global::TallyConnector.Core.Models.Line[] GetTDLLines(string xmlTag = "LEDGERENTRY")
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(ReportName, [LedgerNameTDLFieldName], xmlTag);
        return lines;
    }

    public static global::TallyConnector.Core.Models.Field[] GetTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[1];
        fields[0] = new(LedgerNameTDLFieldName, "LEDGERNAME", "$LEDGERNAME");
        return fields;
    }
}