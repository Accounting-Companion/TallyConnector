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
[TallyConnector.Core.Attributes.GenerateHelperMethod<Group>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public partial class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

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

    [TestMethod]
    public async Task TestComplexModel()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
using System.Collections.Generic;
using TallyConnector.Core.Attributes;
namespace TestNameSpace;
[GenerateHelperMethod<Voucher>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public partial class Voucher : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    [TDLCollection(CollectionName = ""LedgerEntries"")]
    public List<LedgerEntry> LedgerEntries { get; set; } = new();

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }


}
public partial class LedgerEntry
{
    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }
}";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string VoucherParentTDLFieldName = ""TC_Voucher_Parent"";
    internal const string VoucherNameTDLFieldName = ""TC_Voucher_Name"";
    const string VoucherLedgerEntryCollectionName = ""LedgerEntries"";
    internal const string VoucherReportName = ""TC_VoucherList"";
    const string Voucher_collectionName = ""TC_VoucherCollection"";
    internal static global::TallyConnector.Core.Models.RequestEnvelope GetVoucherRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, VoucherReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(VoucherReportName)];
        tdlMsg.Form = [new(VoucherReportName)];
        tdlMsg.Part = [..global::TestNameSpace.TallyService.GetVoucherTDLParts()];
        tdlMsg.Line = [..global::TestNameSpace.TallyService.GetVoucherTDLLines()];
        tdlMsg.Field = [..global::TestNameSpace.TallyService.GetVoucherTDLFields()];
        tdlMsg.Collection = [..global::TestNameSpace.TallyService.GetVoucherTDLCollections()];
        tdlMsg.Functions = [..global::TestNameSpace.TallyService.GetDefaultTDLFunctions()];
        return reqEnvelope;
    }

    internal static global::TallyConnector.Core.Models.Part[] GetVoucherTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Part[2];
        parts[0] = new(VoucherReportName, Voucher_collectionName);
        var ledgerEntriesParts = global::TestNameSpace.TallyService.GetLedgerEntryTDLParts(""TC_VoucherLedgerEntriesList"", VoucherLedgerEntryCollectionName);
        parts.AddToArray(ledgerEntriesParts, 1);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetVoucherTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[2];
        lines[0] = new(VoucherReportName, [VoucherParentTDLFieldName,VoucherNameTDLFieldName], ""VOUCHER"")
        {
            Explode = [$""TC_VoucherLedgerEntriesList:Yes""]
        };
        var ledgerEntriesLines = global::TestNameSpace.TallyService.GetLedgerEntryTDLLines(""LEDGERENTRIES"");
        lines.AddToArray(ledgerEntriesLines, 1);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetVoucherTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[3];
        fields[0] = new(VoucherParentTDLFieldName, ""PARENT"", ""$PARENT"");
        var ledgerEntriesFields = global::TestNameSpace.TallyService.GetLedgerEntryTDLFields();
        fields.AddToArray(ledgerEntriesFields, 1);
        fields[2] = new(VoucherNameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }

    internal static global::TallyConnector.Core.Models.Collection[] GetVoucherTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(Voucher_collectionName, ""VOUCHER"", nativeFields: [""*"", VoucherLedgerEntryCollectionName]);
        return collections;
    }
}";
        var resp2 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string LedgerEntryNameTDLFieldName = ""TC_LedgerEntry_Name"";
    internal const string LedgerEntryReportName = ""TC_LedgerEntryList"";
    const string LedgerEntry_collectionName = ""LedgerEntry"";
    internal static global::TallyConnector.Core.Models.Part[] GetLedgerEntryTDLParts(string partName = LedgerEntryReportName, string? collectionName = LedgerEntry_collectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = new(partName, collectionName, LedgerEntryReportName)
        {
            XMLTag = xmlTag
        };
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetLedgerEntryTDLLines(string xmlTag = ""LEDGERENTRY"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(LedgerEntryReportName, [LedgerEntryNameTDLFieldName], xmlTag);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetLedgerEntryTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[1];
        fields[0] = new(LedgerEntryNameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src,
                                                     [("TestNameSpace.Voucher.TallyService.TDLReport.g.cs", resp1),
                                                         ("TestNameSpace.LedgerEntry.TallyService.TDLReport.g.cs", resp2)]);
    }
    [TestMethod]
    public async Task TestComplexNestedModel()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
using System.Collections.Generic;
using TallyConnector.Core.Attributes;
namespace TestNameSpace;
[GenerateHelperMethod<Voucher>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public partial class Voucher : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    [TDLCollection(CollectionName = ""LedgerEntries"")]
    [XmlElement(ElementName = ""LedgerEntries.List"")]
    public List<LedgerEntry> LedgerEntries { get; set; } = new();
    

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

    [TDLCollection(CollectionName = ""InventoryEntries"")]
    public List<InventoryEntry> InventoryEntries { get; set; } = new();


}
public partial class LedgerEntry
{
    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }
    

}
public partial class InventoryEntry
{
    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

    [TDLCollection(CollectionName = ""AccountingAllocations"")]
    public List<LedgerEntry> LedgerEntries { get; set; } = new();
}";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string VoucherParentTDLFieldName = ""TC_Voucher_Parent"";
    internal const string VoucherNameTDLFieldName = ""TC_Voucher_Name"";
    const string VoucherLedgerEntryCollectionName = ""LedgerEntries"";
    const string VoucherInventoryEntryCollectionName = ""InventoryEntries"";
    internal const string VoucherReportName = ""TC_VoucherList"";
    const string Voucher_collectionName = ""TC_VoucherCollection"";
    internal static global::TallyConnector.Core.Models.RequestEnvelope GetVoucherRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.RequestEnvelope(global::TallyConnector.Core.Models.HType.Data, VoucherReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(VoucherReportName)];
        tdlMsg.Form = [new(VoucherReportName)];
        tdlMsg.Part = [..global::TestNameSpace.TallyService.GetVoucherTDLParts()];
        tdlMsg.Line = [..global::TestNameSpace.TallyService.GetVoucherTDLLines()];
        tdlMsg.Field = [..global::TestNameSpace.TallyService.GetVoucherTDLFields()];
        tdlMsg.Collection = [..global::TestNameSpace.TallyService.GetVoucherTDLCollections()];
        tdlMsg.Functions = [..global::TestNameSpace.TallyService.GetDefaultTDLFunctions()];
        return reqEnvelope;
    }

    internal static global::TallyConnector.Core.Models.Part[] GetVoucherTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Part[4];
        parts[0] = new(VoucherReportName, Voucher_collectionName);
        var ledgerEntriesParts = global::TestNameSpace.TallyService.GetLedgerEntryTDLParts(""TC_VoucherLedgerEntriesList"", VoucherLedgerEntryCollectionName);
        parts.AddToArray(ledgerEntriesParts, 1);
        var inventoryEntriesParts = global::TestNameSpace.TallyService.GetInventoryEntryTDLParts(""TC_VoucherInventoryEntriesList"", VoucherInventoryEntryCollectionName);
        parts.AddToArray(inventoryEntriesParts, 2);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetVoucherTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[3];
        lines[0] = new(VoucherReportName, [VoucherParentTDLFieldName,VoucherNameTDLFieldName], ""VOUCHER"")
        {
            Explode = [$""TC_VoucherLedgerEntriesList:Yes"", $""TC_VoucherInventoryEntriesList:Yes""]
        };
        var ledgerEntriesLines = global::TestNameSpace.TallyService.GetLedgerEntryTDLLines(""LedgerEntries.List"");
        lines.AddToArray(ledgerEntriesLines, 1);
        var inventoryEntriesLines = global::TestNameSpace.TallyService.GetInventoryEntryTDLLines(""INVENTORYENTRIES"");
        lines.AddToArray(inventoryEntriesLines, 2);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetVoucherTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[4];
        fields[0] = new(VoucherParentTDLFieldName, ""PARENT"", ""$PARENT"");
        var ledgerEntriesFields = global::TestNameSpace.TallyService.GetLedgerEntryTDLFields();
        fields.AddToArray(ledgerEntriesFields, 1);
        fields[2] = new(VoucherNameTDLFieldName, ""NAME"", ""$NAME"");
        var inventoryEntriesFields = global::TestNameSpace.TallyService.GetInventoryEntryTDLFields();
        fields.AddToArray(inventoryEntriesFields, 3);
        return fields;
    }

    internal static global::TallyConnector.Core.Models.Collection[] GetVoucherTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(Voucher_collectionName, ""VOUCHER"", nativeFields: [""*"", VoucherLedgerEntryCollectionName, VoucherInventoryEntryCollectionName]);
        return collections;
    }
}";
        var resp2 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string LedgerEntryNameTDLFieldName = ""TC_LedgerEntry_Name"";
    internal const string LedgerEntryReportName = ""TC_LedgerEntryList"";
    const string LedgerEntry_collectionName = ""LedgerEntry"";
    internal static global::TallyConnector.Core.Models.Part[] GetLedgerEntryTDLParts(string partName = LedgerEntryReportName, string? collectionName = LedgerEntry_collectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Part[1];
        parts[0] = new(partName, collectionName, LedgerEntryReportName)
        {
            XMLTag = xmlTag
        };
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetLedgerEntryTDLLines(string xmlTag = ""LEDGERENTRY"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(LedgerEntryReportName, [LedgerEntryNameTDLFieldName], xmlTag);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetLedgerEntryTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[1];
        fields[0] = new(LedgerEntryNameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }
}";
        var resp3 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string InventoryEntryNameTDLFieldName = ""TC_InventoryEntry_Name"";
    const string InventoryEntryLedgerEntryCollectionName = ""AccountingAllocations"";
    internal const string InventoryEntryReportName = ""TC_InventoryEntryList"";
    const string InventoryEntry_collectionName = ""InventoryEntry"";
    internal static global::TallyConnector.Core.Models.Part[] GetInventoryEntryTDLParts(string partName = InventoryEntryReportName, string? collectionName = InventoryEntry_collectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Part[2];
        parts[0] = new(partName, collectionName, InventoryEntryReportName)
        {
            XMLTag = xmlTag
        };
        var ledgerEntriesParts = global::TestNameSpace.TallyService.GetLedgerEntryTDLParts(""TC_InventoryEntryLedgerEntriesList"", InventoryEntryLedgerEntryCollectionName);
        parts.AddToArray(ledgerEntriesParts, 1);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetInventoryEntryTDLLines(string xmlTag = ""INVENTORYENTRY"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[1];
        lines[0] = new(InventoryEntryReportName, [InventoryEntryNameTDLFieldName], xmlTag)
        {
            Explode = [$""TC_InventoryEntryLedgerEntriesList:Yes""]
        };
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetInventoryEntryTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[1];
        fields[0] = new(InventoryEntryNameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src,
                                                     [("TestNameSpace.Voucher.TallyService.TDLReport.g.cs", resp1),
                                                         ("TestNameSpace.LedgerEntry.TallyService.TDLReport.g.cs", resp2),
                                                         ("TestNameSpace.InventoryEntry.TallyService.TDLReport.g.cs", resp3)
                                                     ]);
    }

    [TestMethod]
    public async Task TestComlexModelWithNested2()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
using System.Collections.Generic;
using TallyConnector.Core.Attributes;
namespace TestNameSpace;
[GenerateHelperMethod<Group>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public partial class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

    [XmlElement(ElementName = ""LANGUAGENAME.LIST"")]
    [TDLCollection(CollectionName = ""LanguageName"")]
    public List<TallyConnector.Core.Models.LanguageNameList>? LanguageNameList { get; set; }
}
";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string GroupParentTDLFieldName = ""TC_Group_Parent"";
    internal const string GroupNameTDLFieldName = ""TC_Group_Name"";
    const string GroupLanguageNameListCollectionName = ""LanguageName"";
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
        var parts = new global::TallyConnector.Core.Models.Part[4];
        parts[0] = new(GroupReportName, Group_collectionName);
        var languageNameListParts = global::TestNameSpace.TallyService.GetLanguageNameListTDLParts(""TC_GroupLanguageNameListList"", GroupLanguageNameListCollectionName);
        parts.AddToArray(languageNameListParts, 1);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetGroupTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[4];
        lines[0] = new(GroupReportName, [GroupParentTDLFieldName,GroupNameTDLFieldName], ""GROUP"")
        {
            Explode = [$""TC_GroupLanguageNameListList:Yes""]
        };
        var languageNameListLines = global::TestNameSpace.TallyService.GetLanguageNameListTDLLines(""LANGUAGENAME.LIST"");
        lines.AddToArray(languageNameListLines, 1);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetGroupTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[4];
        fields[0] = new(GroupParentTDLFieldName, ""PARENT"", ""$PARENT"");
        fields[1] = new(GroupNameTDLFieldName, ""NAME"", ""$NAME"");
        var languageNameListFields = global::TestNameSpace.TallyService.GetLanguageNameListTDLFields();
        fields.AddToArray(languageNameListFields, 2);
        return fields;
    }

    internal static global::TallyConnector.Core.Models.Collection[] GetGroupTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(Group_collectionName, ""GROUP"", nativeFields: [""*"", GroupLanguageNameListCollectionName]);
        return collections;
    }
}";
        var resp2 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string LanguageNameListLanguageAliasTDLFieldName = ""TC_LanguageNameList_LanguageAlias"";
    internal const string LanguageNameListReportName = ""TC_LanguageNameListList"";
    const string LanguageNameList_collectionName = ""LanguageNameList"";
    internal static global::TallyConnector.Core.Models.Part[] GetLanguageNameListTDLParts(string partName = LanguageNameListReportName, string? collectionName = LanguageNameList_collectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Part[3];
        parts[0] = new(partName, collectionName, LanguageNameListReportName)
        {
            XMLTag = xmlTag
        };
        var nameListParts = global::TestNameSpace.TallyService.GetNamesTDLParts(""TC_LanguageNameListNameListList"", null);
        parts.AddToArray(nameListParts, 1);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetLanguageNameListTDLLines(string xmlTag = ""LANGUAGENAMELIST"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[3];
        lines[0] = new(LanguageNameListReportName, [LanguageNameListLanguageAliasTDLFieldName], xmlTag)
        {
            Explode = [$""TC_LanguageNameListNameListList:Yes""]
        };
        var nameListLines = global::TestNameSpace.TallyService.GetNamesTDLLines(""NAME.LIST"");
        lines.AddToArray(nameListLines, 1);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetLanguageNameListTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[2];
        fields[0] = new(LanguageNameListLanguageAliasTDLFieldName, ""LANGUAGEALIAS"", ""$LANGUAGEALIAS"");
        var nameListFields = global::TestNameSpace.TallyService.GetNamesTDLFields();
        fields.AddToArray(nameListFields, 1);
        return fields;
    }
}";
        var resp3 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string NamesNAMESTDLFieldName = ""TC_Names_NAMES"";
    const string NamesNAMESCollectionName = ""Name"";
    internal const string NamesReportName = ""TC_NamesList"";
    const string Names_collectionName = ""Names"";
    internal static global::TallyConnector.Core.Models.Part[] GetNamesTDLParts(string partName = NamesReportName, string? collectionName = Names_collectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Part[2];
        parts[0] = new(partName, collectionName, NamesReportName)
        {
            XMLTag = xmlTag
        };
        parts[1] = new(""TC_NamesNAMESList"", NamesNAMESCollectionName);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetNamesTDLLines(string xmlTag = ""NAMES"")
    {
        var lines = new global::TallyConnector.Core.Models.Line[2];
        lines[0] = new(NamesReportName, [""SimpleField""], xmlTag)
        {
            Explode = [$""TC_NamesNAMESList:Yes""]
        };
        lines[1] = new(""TC_NamesNAMESList"", [NamesNAMESTDLFieldName]);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetNamesTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[1];
        fields[0] = new(NamesNAMESTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src,
                                                       [
                                                           ("TestNameSpace.Group.TallyService.TDLReport.g.cs", resp1),
                                                           ("TallyConnector.Core.Models.LanguageNameList.TallyService.TDLReport.g.cs", resp2),
                                                           ("TallyConnector.Core.Models.Names.TallyService.TDLReport.g.cs", resp3),
                                                       ]);
    }

    [TestMethod]
    public async Task TestDecimal()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
namespace TestNameSpace;
[TallyConnector.Core.Attributes.GenerateHelperMethod<Group>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public partial class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = ""NAME"")]
    public decimal? OpeningBal { get; set; }

}";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string GroupParentTDLFieldName = ""TC_Group_Parent"";
    internal const string GroupOpeningBalTDLFieldName = ""TC_Group_OpeningBal"";
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
        lines[0] = new(GroupReportName, [GroupParentTDLFieldName,GroupOpeningBalTDLFieldName], ""GROUP"");
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetGroupTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[2];
        fields[0] = new(GroupParentTDLFieldName, ""PARENT"", ""$PARENT"");
        fields[1] = new(GroupOpeningBalTDLFieldName, ""NAME"", ""$NAME"");
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

    [TestMethod]
    public async Task TestEnum()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
namespace TestNameSpace;
[TallyConnector.Core.Attributes.GenerateHelperMethod<Group>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public partial class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = ""NAME"")]
    public TallyConnector.Core.Models.AdAllocType? AdAllocType { get; set; }

}";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string GroupParentTDLFieldName = ""TC_Group_Parent"";
    internal const string GroupOpeningBalTDLFieldName = ""TC_Group_OpeningBal"";
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
        lines[0] = new(GroupReportName, [GroupParentTDLFieldName,GroupOpeningBalTDLFieldName], ""GROUP"");
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetGroupTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[2];
        fields[0] = new(GroupParentTDLFieldName, ""PARENT"", ""$PARENT"");
        fields[1] = new(GroupOpeningBalTDLFieldName, ""NAME"", ""$NAME"");
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

    [TestMethod]
    public async Task TestSimpleList()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
using System.Collections.Generic;
using TallyConnector.Core.Attributes;
namespace TestNameSpace;
[GenerateHelperMethod<Group>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public partial class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

    [XmlArray(ElementName = ""ADDRESS.LIST"")]
    [XmlArrayItem(ElementName = ""Address"")]
    [TDLCollection(CollectionName =""Address"")]
    public List<string>? Addreses { get; set; }
}
";
        var resp1 = @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TestNameSpace;
public partial class TallyService
{
    internal const string GroupParentTDLFieldName = ""TC_Group_Parent"";
    internal const string GroupNameTDLFieldName = ""TC_Group_Name"";
    internal const string GroupAddresesTDLFieldName = ""TC_Group_Addreses"";
    const string GroupAddresesCollectionName = ""Address"";
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
        var parts = new global::TallyConnector.Core.Models.Part[2];
        parts[0] = new(GroupReportName, Group_collectionName);
        parts[1] = new(""TC_GroupAddresesList"", GroupAddresesCollectionName)
        {
            XMLTag = ""ADDRESS.LIST""
        };
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Line[] GetGroupTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Line[2];
        lines[0] = new(GroupReportName, [GroupParentTDLFieldName,GroupNameTDLFieldName], ""GROUP"")
        {
            Explode = [$""TC_GroupAddresesList:Yes""]
        };
        lines[1] = new(""TC_GroupAddresesList"", [GroupAddresesTDLFieldName]);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Field[] GetGroupTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Field[3];
        fields[0] = new(GroupParentTDLFieldName, ""PARENT"", ""$PARENT"");
        fields[1] = new(GroupNameTDLFieldName, ""NAME"", ""$NAME"");
        fields[2] = new(GroupAddresesTDLFieldName, ""Address"", ""$Address"");
        return fields;
    }

    internal static global::TallyConnector.Core.Models.Collection[] GetGroupTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Collection[1];
        collections[0] = new(Group_collectionName, ""GROUP"", nativeFields: [""*"", GroupAddresesCollectionName]);
        return collections;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src, ("TestNameSpace.Group.TallyService.TDLReport.g.cs", resp1));

    }
}
