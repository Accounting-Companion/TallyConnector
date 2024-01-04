namespace UnitTests.BasicTests;
[TestClass]
public class BasicSourceGenTests
{
    [TestMethod]
    public async Task TestBasicModel()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
namespace TestNameSpace;
public partial class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

    //private string TestField {get;set;}

    public virtual void RemoveNullChilds()
    {

    }
}";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class Group
{
    const string ParentTDLFieldName = ""TC_Group_Parent"";
    const string NameTDLFieldName = ""TC_Group_Name"";
    internal const string ReportName = ""TC_GroupList"";
    const string _collectionName = ""TC_GroupCollection"";
    public static global::TallyConnector.Core.Models.RequestEnvelope GetRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, ReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(ReportName)];
        tdlMsg.Form = [new(ReportName)];
        tdlMsg.Part = [..global::TestNameSpace.Group.GetTDLParts()];
        tdlMsg.Line = [..global::TestNameSpace.Group.GetTDLLines()];
        tdlMsg.Field = [..global::TestNameSpace.Group.GetTDLFields()];
        tdlMsg.Collection = [..global::TestNameSpace.Group.GetTDLCollections()];
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
        lines[0] = new(ReportName, [ParentTDLFieldName,NameTDLFieldName], ""GROUP"");
        return lines;
    }

    public static global::TallyConnector.Core.Models.Field[] GetTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[2];
        fields[0] = new(ParentTDLFieldName, ""PARENT"", ""$PARENT"");
        fields[1] = new(NameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }

    public static global::TallyConnector.Core.Models.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(_collectionName, ""GROUP"", nativeFields: [""*""]);
        return collections;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src, ("TestNameSpace.Group.TDLReport.g.cs", resp1));
    }

    [TestMethod]
    public async Task TestComplexModel()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
using System.Collections.Generic;
namespace TestNameSpace;
public partial class Voucher : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    public List<LedgerEntry> LedgerEntries { get; set; } = new();

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }


}
public partial class LedgerEntry
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Name { get; set; }
}";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class Voucher
{
    const string ParentTDLFieldName = ""TC_Voucher_Parent"";
    const string NameTDLFieldName = ""TC_Voucher_Name"";
    internal const string ReportName = ""TC_VoucherList"";
    const string _collectionName = ""TC_VoucherCollection"";
    public static global::TallyConnector.Core.Models.RequestEnvelope GetRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, ReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(ReportName)];
        tdlMsg.Form = [new(ReportName)];
        tdlMsg.Part = [..global::TestNameSpace.Voucher.GetTDLParts()];
        tdlMsg.Line = [..global::TestNameSpace.Voucher.GetTDLLines()];
        tdlMsg.Field = [..global::TestNameSpace.Voucher.GetTDLFields()];
        tdlMsg.Collection = [..global::TestNameSpace.Voucher.GetTDLCollections()];
        return reqEnvelope;
    }

    public static global::TallyConnector.Core.Models.Part[] GetTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Part[2];
        parts[0] = new(ReportName, _collectionName);
        var ledgerEntriesParts = global::TestNameSpace.LedgerEntry.GetTDLParts();
        parts.AddToArray(ledgerEntriesParts, 1);
        return parts;
    }

    public static global::TallyConnector.Core.Models.Line[] GetTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[2];
        lines[0] = new(ReportName, [ParentTDLFieldName,NameTDLFieldName], ""VOUCHER"")
        {
            Explode = [$""{TestNameSpace.LedgerEntry.ReportName}:Yes""]
        };
        var ledgerEntriesLines = global::TestNameSpace.LedgerEntry.GetTDLLines();
        lines.AddToArray(ledgerEntriesLines, 1);
        return lines;
    }

    public static global::TallyConnector.Core.Models.Field[] GetTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[3];
        fields[0] = new(ParentTDLFieldName, ""PARENT"", ""$PARENT"");
        var ledgerEntriesFields = global::TestNameSpace.LedgerEntry.GetTDLFields();
        fields.AddToArray(ledgerEntriesFields, 1);
        fields[2] = new(NameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }

    public static global::TallyConnector.Core.Models.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(_collectionName, ""VOUCHER"", nativeFields: [""*""]);
        return collections;
    }
}";
        var resp2 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class LedgerEntry
{
    const string NameTDLFieldName = ""TC_LedgerEntry_Name"";
    internal const string ReportName = ""TC_LedgerEntryList"";
    const string _collectionName = ""LedgerEntry"";
    public static global::TallyConnector.Core.Models.Part[] GetTDLParts(string collectionName = _collectionName)
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = new(ReportName, collectionName);
        return parts;
    }

    public static global::TallyConnector.Core.Models.Line[] GetTDLLines(string xmlTag = ""LEDGERENTRY"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(ReportName, [NameTDLFieldName], xmlTag);
        return lines;
    }

    public static global::TallyConnector.Core.Models.Field[] GetTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[1];
        fields[0] = new(NameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src,
                                                     [("TestNameSpace.Voucher.TDLReport.g.cs", resp1),
                                                         ("TestNameSpace.LedgerEntry.TDLReport.g.cs", resp2)]);
    }
    [TestMethod]
    public async Task TestComplexNestedModel()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
using System.Collections.Generic;
namespace TestNameSpace;
public partial class Voucher : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    public List<LedgerEntry> LedgerEntries { get; set; } = new();
    

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }


}
public partial class LedgerEntry
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Name { get; set; }

    public List<InventoryEntry> InventoryEntries { get; set; } = new();
}
public partial class InventoryEntry
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Name { get; set; }
}";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class Voucher
{
    const string ParentTDLFieldName = ""TC_Voucher_Parent"";
    const string NameTDLFieldName = ""TC_Voucher_Name"";
    internal const string ReportName = ""TC_VoucherList"";
    const string _collectionName = ""TC_VoucherCollection"";
    public static global::TallyConnector.Core.Models.RequestEnvelope GetRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, ReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(ReportName)];
        tdlMsg.Form = [new(ReportName)];
        tdlMsg.Part = [..global::TestNameSpace.Voucher.GetTDLParts()];
        tdlMsg.Line = [..global::TestNameSpace.Voucher.GetTDLLines()];
        tdlMsg.Field = [..global::TestNameSpace.Voucher.GetTDLFields()];
        tdlMsg.Collection = [..global::TestNameSpace.Voucher.GetTDLCollections()];
        return reqEnvelope;
    }

    public static global::TallyConnector.Core.Models.Part[] GetTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Part[3];
        parts[0] = new(ReportName, _collectionName);
        var ledgerEntriesParts = global::TestNameSpace.LedgerEntry.GetTDLParts();
        parts.AddToArray(ledgerEntriesParts, 1);
        return parts;
    }

    public static global::TallyConnector.Core.Models.Line[] GetTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[3];
        lines[0] = new(ReportName, [ParentTDLFieldName,NameTDLFieldName], ""VOUCHER"")
        {
            Explode = [$""{TestNameSpace.LedgerEntry.ReportName}:Yes""]
        };
        var ledgerEntriesLines = global::TestNameSpace.LedgerEntry.GetTDLLines();
        lines.AddToArray(ledgerEntriesLines, 1);
        return lines;
    }

    public static global::TallyConnector.Core.Models.Field[] GetTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[4];
        fields[0] = new(ParentTDLFieldName, ""PARENT"", ""$PARENT"");
        var ledgerEntriesFields = global::TestNameSpace.LedgerEntry.GetTDLFields();
        fields.AddToArray(ledgerEntriesFields, 1);
        fields[3] = new(NameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }

    public static global::TallyConnector.Core.Models.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(_collectionName, ""VOUCHER"", nativeFields: [""*""]);
        return collections;
    }
}";
        var resp2 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class LedgerEntry
{
    const string NameTDLFieldName = ""TC_LedgerEntry_Name"";
    internal const string ReportName = ""TC_LedgerEntryList"";
    const string _collectionName = ""LedgerEntry"";
    public static global::TallyConnector.Core.Models.Part[] GetTDLParts(string collectionName = _collectionName)
    {
        var parts = new global::TallyConnector.Core.Models.Part[2];
        parts[0] = new(ReportName, collectionName);
        var inventoryEntriesParts = global::TestNameSpace.InventoryEntry.GetTDLParts();
        parts.AddToArray(inventoryEntriesParts, 1);
        return parts;
    }

    public static global::TallyConnector.Core.Models.Line[] GetTDLLines(string xmlTag = ""LEDGERENTRY"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[2];
        lines[0] = new(ReportName, [NameTDLFieldName], xmlTag)
        {
            Explode = [$""{TestNameSpace.InventoryEntry.ReportName}:Yes""]
        };
        var inventoryEntriesLines = global::TestNameSpace.InventoryEntry.GetTDLLines();
        lines.AddToArray(inventoryEntriesLines, 1);
        return lines;
    }

    public static global::TallyConnector.Core.Models.Field[] GetTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[2];
        fields[0] = new(NameTDLFieldName, ""NAME"", ""$NAME"");
        var inventoryEntriesFields = global::TestNameSpace.InventoryEntry.GetTDLFields();
        fields.AddToArray(inventoryEntriesFields, 1);
        return fields;
    }
}";
        var resp3 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class InventoryEntry
{
    const string NameTDLFieldName = ""TC_InventoryEntry_Name"";
    internal const string ReportName = ""TC_InventoryEntryList"";
    const string _collectionName = ""InventoryEntry"";
    public static global::TallyConnector.Core.Models.Part[] GetTDLParts(string collectionName = _collectionName)
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = new(ReportName, collectionName);
        return parts;
    }

    public static global::TallyConnector.Core.Models.Line[] GetTDLLines(string xmlTag = ""INVENTORYENTRY"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(ReportName, [NameTDLFieldName], xmlTag);
        return lines;
    }

    public static global::TallyConnector.Core.Models.Field[] GetTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[1];
        fields[0] = new(NameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src,
                                                     [("TestNameSpace.Voucher.TDLReport.g.cs", resp1),
                                                         ("TestNameSpace.LedgerEntry.TDLReport.g.cs", resp2),
                                                         ("TestNameSpace.InventoryEntry.TDLReport.g.cs", resp3)
                                                     ]);
    }
}
