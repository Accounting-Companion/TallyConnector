using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
public partial class TallyService
{
    internal const string GroupParentTDLFieldName = ""TC_Group_Parent"";
    internal const string GroupNameTDLFieldName = ""TC_Group_Name"";
    internal const string GroupReportName = ""TC_GroupList"";
    const string Group_collectionName = ""TC_GroupCollection"";
    internal static global::TallyConnector.Core.Models.RequestEnvelope GetGroupRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, GroupReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(GroupReportName)];
        tdlMsg.Form = [new(GroupReportName)];
        tdlMsg.Part = [..global::TestNameSpace.TallyService.GetGroupTDLParts()];
        tdlMsg.Line = [..global::TestNameSpace.TallyService.GetGroupTDLLines()];
        tdlMsg.Field = [..global::TestNameSpace.TallyService.GetGroupTDLFields()];
        tdlMsg.Collection = [..global::TestNameSpace.TallyService.GetGroupTDLCollections()];
        tdlMsg.Functions = [..global::TestNameSpace.TallyService.GetDefaultTDLFunctions()];
        return reqEnvelope;
    }

    internal static global::TallyConnector.Core.Models.Part[] GetGroupTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = new(GroupReportName, Group_collectionName);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetGroupTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(GroupReportName, [GroupParentTDLFieldName,GroupNameTDLFieldName], ""GROUP"");
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetGroupTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[2];
        fields[0] = new(GroupParentTDLFieldName, ""PARENT"", ""$PARENT"");
        fields[1] = new(GroupNameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }

    internal static global::TallyConnector.Core.Models.Collection[] GetGroupTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(Group_collectionName, ""GROUP"", nativeFields: [""*""]);
        return collections;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src, ("TestNameSpace.Group.TallyService.TDLReport.g.cs", resp1));
    }
}

