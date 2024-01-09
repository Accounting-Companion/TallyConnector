using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.BasicTests;
[TestClass]
public class HierarchyTests
{
    [TestMethod]
    public async Task TestBasicHierarchy()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
namespace TestNameSpace;
[TallyConnector.Core.Attributes.GenerateHelperMethod<Group>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
[TallyConnector.Core.Attributes.GenerateHelperMethod<Group>]
public partial class TallyServicegrp : TallyConnector.Services.BaseTallyService
{
}
public class Group :BaseGroup, TallyConnector.Core.Models.ITallyBaseObject
{

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

}
public class BaseGroup 
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }
}";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
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
        var lines = new global::TallyConnector.Core.Models.Line[2];
        lines[0] = new(GroupReportName, [GroupNameTDLFieldName], ""GROUP"")
        {
            Use = BaseGroupReportName
        };
        var baseGroupLines = global::TestNameSpace.TallyService.GetBaseGroupTDLLines();
        lines.AddToArray(baseGroupLines, 1);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetGroupTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[2];
        var baseGroupFields = global::TestNameSpace.TallyService.GetBaseGroupTDLFields();
        fields.AddToArray(baseGroupFields, 0);
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
namespace TestNameSpace;
public partial class TallyService
{
    internal const string BaseGroupParentTDLFieldName = ""TC_BaseGroup_Parent"";
    internal const string BaseGroupReportName = ""TC_BaseGroupList"";
    const string BaseGroup_collectionName = ""BaseGroup"";
    internal static global::TallyConnector.Core.Models.Part[] GetBaseGroupTDLParts(string partName = BaseGroupReportName, string? collectionName = BaseGroup_collectionName)
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = new(partName, collectionName, BaseGroupReportName);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetBaseGroupTDLLines(string xmlTag = ""BASEGROUP"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(BaseGroupReportName, [BaseGroupParentTDLFieldName], xmlTag);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetBaseGroupTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[1];
        fields[0] = new(BaseGroupParentTDLFieldName, ""PARENT"", ""$PARENT"");
        return fields;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src,
            ("TestNameSpace.Group.TallyService.TDLReport.g.cs", resp1), ("TestNameSpace.BaseGroup.TallyService.TDLReport.g.cs", resp2));
    }
    [TestMethod]
    public async Task GetComplexHirarchy()
    {
        var src = @"#nullable enable
using System.Xml.Serialization;
namespace TestNameSpace;
[TallyConnector.Core.Attributes.GenerateHelperMethod<UnitTests.Models.Ledger>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}";
        var resp1 = "";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src, ("TestNameSpace.Group.TallyService.TDLReport.g.cs", resp1));
    }
}
