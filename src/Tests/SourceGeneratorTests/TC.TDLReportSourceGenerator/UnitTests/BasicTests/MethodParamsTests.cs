using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.BasicTests;
[TestClass]
public class MethodParamsTests
{
    [TestMethod]
    public async Task TestParams()
    {

        var src = @"
#nullable enable
using System.Xml.Serialization;
namespace TallyConnector.CustomServices;
[TallyConnector.Core.Attributes.GenerateHelperMethod<Group>(GenerationMode=TallyConnector.Core.Models.GenerationMode.GetMultiple,Args = [typeof(TallyConnector.Core.Models.BaseRequestOptions)])]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
[XmlRoot(ElementName = ""GROUP"")]
public partial class Group : TallyConnector.Core.Models.Masters.BaseLedger
{
    //[XmlElement(ElementName = ""PARENT"")]
    //public string? Parent { get; set; }

    //[XmlElement(ElementName = ""NAME"")]
    //public string? Name { get; set; }

}";
        string resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.CustomServices;
partial class TallyService
{
    internal const string GroupParentTDLFieldName = ""TC_Group_Parent"";
    internal const string GroupNameTDLFieldName = ""TC_Group_Name"";
    internal const string GroupReportName = ""TC_GroupList"";
    const string Group_collectionName = ""TC_GroupCollection"";
    public async global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<global::TallyConnector.CustomServices.Group>> GetGroups(global::TallyConnector.Core.Models.BaseRequestOptions? reqOptions = null, global::System.Threading.CancellationToken token = default)
    {
        var reqEnvelope = global::TallyConnector.CustomServices.TallyService.GetGroupRequestEnevelope();
        if (reqOptions != null)
        {
            reqEnvelope.PopulateOptions(reqOptions);
        }

        await PopulateDefaultOptions(reqEnvelope, token);
        var reqXml = reqEnvelope.GetXML();
        var resp = await SendRequestAsync(reqXml, ""GetGroups"", token);
        var respEnv = global::TallyConnector.Services.XMLToObject.GetObjfromXml<global::TallyConnector.CustomServices.Models.TallyServiceReportResponseEnvelopeForGroup>(resp.Response!, null, _logger);
        return respEnv.Objects;
    }

    internal static global::TallyConnector.Core.Models.RequestEnvelope GetGroupRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, GroupReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(GroupReportName)];
        tdlMsg.Form = [new(GroupReportName)];
        tdlMsg.Part = [..global::TallyConnector.CustomServices.TallyService.GetGroupTDLParts()];
        tdlMsg.Line = [..global::TallyConnector.CustomServices.TallyService.GetGroupTDLLines()];
        tdlMsg.Field = [..global::TallyConnector.CustomServices.TallyService.GetGroupTDLFields()];
        tdlMsg.Collection = [..global::TallyConnector.CustomServices.TallyService.GetGroupTDLCollections()];
        tdlMsg.Functions = [..global::TallyConnector.CustomServices.TallyService.GetDefaultTDLFunctions()];
        return reqEnvelope;
    }

    internal static global::TallyConnector.Core.Models.Part GetGroupMainTDLPart()
    {
        return new(GroupReportName, Group_collectionName);
    }

    internal static global::TallyConnector.Core.Models.Part[] GetGroupTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = global::TallyConnector.CustomServices.TallyService.GetGroupMainTDLPart();
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
        var resp2 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.CustomServices.Models;
[global::System.Xml.Serialization.XmlRootAttribute(""ENVELOPE"")]
public class TallyServiceReportResponseEnvelopeForGroup
{
    [System.Xml.Serialization.XmlElementAttribute(ElementName = ""GROUP"")]
    public global::System.Collections.Generic.List<global::TallyConnector.CustomServices.Group> Objects { get; set; } = [];
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src, [
            ("TallyConnector.CustomServices.Group.TallyService.TDLReport.g.cs", resp1),
            ("TallyService.ReportResponseEnvelope.g.cs", resp2),
        ]);
    }

}
