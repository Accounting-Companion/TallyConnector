using TallyConnector.Core.Extensions;

#nullable enable
namespace IntegrationTests;
public partial class Group
{
    const string ParentTDLFieldName = "TC_Group_Parent";
    internal const string ReportName = "TC_GroupList";
    const string _collectionName = "TC_GroupCollection";
    public static global::TallyConnector.Core.Models.RequestEnvelope GetRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, ReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(ReportName)];
        tdlMsg.Form = [new(ReportName)];
        tdlMsg.Part = [..global::IntegrationTests.Group.GetTDLParts()];
        tdlMsg.Line = [..global::IntegrationTests.Group.GetTDLLines()];
        tdlMsg.Field = [..global::IntegrationTests.Group.GetTDLFields()];
        tdlMsg.Collection = [..global::IntegrationTests.Group.GetTDLCollections()];
        return reqEnvelope;
    }

    public static global::TallyConnector.Core.Models.Part[] GetTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = new(ReportName, _collectionName);
        return parts;
    }

    public static global::TallyConnector.Core.Models.Line[] GetTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(ReportName, [ParentTDLFieldName], "GROUP");
        return lines;
    }

    public static global::TallyConnector.Core.Models.Field[] GetTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[1];
        fields[0] = new(ParentTDLFieldName, "PARENT", "$PARENT");
        return fields;
    }

    public static global::TallyConnector.Core.Models.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(_collectionName, "GROUP", nativeFields: ["*"]);
        return collections;
    }
}