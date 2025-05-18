using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.AdvanceTests;
[TestClass]
public class ListTests
{
    [TestMethod]
    public async Task TestSimpleListinClass()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using System.Collections.Generic;
namespace UnitTests.TestBasic;

[ImplementTallyRequestableObject]
[TDLCollection(Type = ""Ledger"")]
public partial class Ledger
{
    public string Name { get; set; }
    public string Parent { get; set; }
    public List<string> Address { get; set; }
}
";
         await VerifyTDLReportV2.VerifyGeneratorAsync(src,
            ("UnitTests.TestBasic.Ledger.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Ledger
*/
partial class Ledger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string Name_39DV_FieldName = ""Name_39DV"";
    const string Parent_DR40_FieldName = ""Parent_DR40"";
    const string Address_YWHP_FieldName = ""Address_YWHP"";
    const string Address_YWHP_PartName = ""Address_YWHP"";
    const string ReportName = ""Ledger_BUL5"";
    const string CollectionName = ""LedgersCollection_BUL5"";
    const string XMLTag = ""LEDGER"";
    const int SimpleFieldsCount = 3;
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
        parts[0] = new(Address_YWHP_PartName, ""Address"");
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [Name_39DV_FieldName,Parent_DR40_FieldName], XMLTag)
        {
            Explode = [$""{Address_YWHP_PartName}:YES""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(Address_YWHP_PartName, [Address_YWHP_FieldName]);
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_39DV_FieldName, ""NAME"", ""$Name"");
        _fields[1] = new(Parent_DR40_FieldName, ""PARENT"", ""$Parent"");
        _fields[2] = new(Address_YWHP_FieldName, ""ADDRESS"", ""$Address"");
        return _fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, ""Ledger"", nativeFields: [..GetFetchList()]);
        return collections;
    }

    internal static string[] GetFetchList()
    {
        return[""Name"", ""Parent"", ""Address""];
    }
}"));
    }
    [TestMethod]
    public async Task TestSimpleListinClassNested()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
namespace UnitTests.TestBasic;

[ImplementTallyRequestableObject]
[TDLCollection(Type = ""Ledger"")]
public partial class Ledger
{
    public string Name { get; set; }
    public string Parent { get; set; }
    [TDLCollection(CollectionName = ""MULTIADRESS"")]
    public List<Multiaddress> MultiAddress { get; set; }
}
public class Multiaddress
{   [XmlArray(ElementName = ""ADDRESS.LIST"")]
    [XmlArrayItem(ElementName = ""ADDRESS"")]
    [TDLCollection(CollectionName = ""ADDRESS"")]
    public List<string> AddressLines { get; set; }
}
";
         await VerifyTDLReportV2.VerifyGeneratorAsync(src,
            ("UnitTests.TestBasic.Ledger.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Ledger
*/
partial class Ledger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string Name_39DV_FieldName = ""Name_39DV"";
    const string Parent_DR40_FieldName = ""Parent_DR40"";
    const string AddressLines_VE9R_FieldName = ""AddressLines_VE9R"";
    const string MultiAddress_RJUI_PartName = ""MultiAddress_RJUI"";
    const string AddressLines_VE9R_PartName = ""AddressLines_VE9R"";
    const string ReportName = ""Ledger_BUL5"";
    const string CollectionName = ""LedgersCollection_BUL5"";
    const string XMLTag = ""LEDGER"";
    const int SimpleFieldsCount = 3;
    const int ComplexFieldsCount = 2;
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
        parts[0] = new(MultiAddress_RJUI_PartName, ""MULTIADRESS"");
        parts[1] = new(AddressLines_VE9R_PartName, ""ADDRESS"")
        {
            XMLTag = ""ADDRESS.LIST""
        };
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [Name_39DV_FieldName,Parent_DR40_FieldName], XMLTag)
        {
            Explode = [$""{MultiAddress_RJUI_PartName}:YES""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(MultiAddress_RJUI_PartName, [""SimpleField""], ""MultiAddress"")
        {
            Explode = [$""{AddressLines_VE9R_PartName}:YES""]
        };
        _lines[1] = new(AddressLines_VE9R_PartName, [AddressLines_VE9R_FieldName]);
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_39DV_FieldName, ""NAME"", ""$Name"");
        _fields[1] = new(Parent_DR40_FieldName, ""PARENT"", ""$Parent"");
        _fields[2] = new(AddressLines_VE9R_FieldName, ""ADDRESS"", ""$ADDRESS"");
        return _fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, ""Ledger"", nativeFields: [..GetFetchList()]);
        return collections;
    }

    internal static string[] GetFetchList()
    {
        return[""Name"", ""Parent"", ""MULTIADRESS.ADDRESS""];
    }
}"));
    }

    [TestMethod]
    public async Task TestMultipleXMLAttributesforSingleProperty()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
namespace UnitTests.TestBasic;

[ImplementTallyRequestableObject]
[TDLCollection(Type = ""Voucher"")]
public partial class Voucher
{
    public string VoucherTypeName { get; set; }
    public string VoucherNumber { get; set; }

    [XmlElement(ElementName = ""ALLILEDGERENTRIES.LIST"", Type = typeof(LedgerEntry))]
    [XmlElement(ElementName = ""LEDGERENTRIES.LIST"", Type = typeof(ELedgerEntry))]
    public List<LedgerEntry> LedgerEntries { get; set; }
}
[TDLCollection(CollectionName = ""AllLedgerEntries"", ExplodeCondition = ""$$NUMITEMS:AllLedgerEntries>0"")]
public class LedgerEntry
{  
    public string LedgerName { get; set; }
}
[TDLCollection(CollectionName = ""LedgerEntries"", ExplodeCondition = ""$$NUMITEMS:LedgerEntries>0"")]
public class ELedgerEntry : LedgerEntry
{
    public string NewProp { get; set; }
}
";
        await VerifyTDLReportV2.VerifyGeneratorAsync(src,
           ("UnitTests.TestBasic.Voucher.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Voucher
*/
partial class Voucher : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string VoucherTypeName_PPEC_FieldName = ""VoucherTypeName_PPEC"";
    const string VoucherNumber_VW4Y_FieldName = ""VoucherNumber_VW4Y"";
    const string LedgerName_YBOB_FieldName = ""LedgerName_YBOB"";
    const string NewProp_LUEW_FieldName = ""NewProp_LUEW"";
    const string LedgerEntries_OO83_PartName = ""LedgerEntries_OO83"";
    const string LedgerEntries_SLOU_PartName = ""LedgerEntries_SLOU"";
    const string ReportName = ""Voucher_IEQM"";
    const string CollectionName = ""VouchersCollection_IEQM"";
    const string XMLTag = ""VOUCHER"";
    const int SimpleFieldsCount = 4;
    const int ComplexFieldsCount = 2;
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
        return reqEnvelope;
    }

    public static global::System.Xml.Serialization.XmlAttributeOverrides GetXMLAttributeOverides()
    {
        var xmlAttributeOverrides = new global::System.Xml.Serialization.XmlAttributeOverrides();
        var XmlAttributes = new global::System.Xml.Serialization.XmlAttributes();
        XmlAttributes.XmlElements.Add(new(XMLTag));
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::UnitTests.TestBasic.Voucher>.TypeInfo, ""Objects"", XmlAttributes);
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
        parts[0] = new(LedgerEntries_OO83_PartName, ""AllLedgerEntries"");
        parts[1] = new(LedgerEntries_SLOU_PartName, ""LedgerEntries"");
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [VoucherTypeName_PPEC_FieldName,VoucherNumber_VW4Y_FieldName], XMLTag)
        {
            Explode = [$""{LedgerEntries_OO83_PartName}:{string.Format(""$$NUMITEMS:AllLedgerEntries>0"", ""LedgerEntries"")}"", $""{LedgerEntries_SLOU_PartName}:{string.Format(""$$NUMITEMS:AllLedgerEntries>0"", ""LedgerEntries"")}""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(LedgerEntries_OO83_PartName, [LedgerName_YBOB_FieldName], ""ALLILEDGERENTRIES.LIST"");
        _lines[1] = new(LedgerEntries_SLOU_PartName, [LedgerName_YBOB_FieldName,NewProp_LUEW_FieldName], ""LEDGERENTRIES.LIST"");
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(VoucherTypeName_PPEC_FieldName, ""VOUCHERTYPENAME"", ""$VoucherTypeName"");
        _fields[1] = new(VoucherNumber_VW4Y_FieldName, ""VOUCHERNUMBER"", ""$VoucherNumber"");
        _fields[2] = new(LedgerName_YBOB_FieldName, ""LEDGERNAME"", ""$LedgerName"");
        _fields[3] = new(NewProp_LUEW_FieldName, ""NEWPROP"", ""$NewProp"");
        return _fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, ""Voucher"", nativeFields: [..GetFetchList()]);
        return collections;
    }

    internal static string[] GetFetchList()
    {
        return[""VoucherTypeName"", ""VoucherNumber"", ""AllLedgerEntries.LedgerName"", ""LedgerEntries.LedgerName,LedgerEntries.NewProp""];
    }
}"));
    }

    [TestMethod]
    public async Task TestMultipleXMLAttributesforPropertyandRepeated()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
namespace UnitTests.TestBasic;

[ImplementTallyRequestableObject]
[TDLCollection(Type = ""Voucher"")]
public partial class Voucher
{
    public string VoucherTypeName { get; set; }
    public string VoucherNumber { get; set; }

    

    [XmlElement(ElementName = ""ALLILEDGERENTRIES.LIST"", Type = typeof(LedgerEntry))]
    [XmlElement(ElementName = ""LEDGERENTRIES.LIST"", Type = typeof(ELedgerEntry))]
    public List<LedgerEntry> LedgerEntries { get; set; }

    [TDLCollection(CollectionName = ""INVENTORYALLOCATIONS"", ExplodeCondition = ""$$NUMITEMS:INVENTORYALLOCATIONS>0"")]
    public List<InventoryEntry> InventoryEntry { get; set; }
}
[TDLCollection(CollectionName = ""AllLedgerEntries"", ExplodeCondition = ""$$NUMITEMS:AllLedgerEntries>0"")]
public class LedgerEntry
{  
    public string LedgerName { get; set; }
}
[TDLCollection(CollectionName = ""AllLedgerEntries"", ExplodeCondition = ""$$NUMITEMS:AllLedgerEntries>0"")]
public class InventoryEntry
{  
    public string StockItemName { get; set; }

    [TDLCollection(CollectionName = ""ACCOUNTINGALLOCATIONS"", ExplodeCondition = ""$$NUMITEMS:ACCOUNTINGALLOCATIONS>0"")]
    public List<ELedgerEntry> AccountingEntries { get; set; }
}
[TDLCollection(CollectionName = ""LedgerEntries"", ExplodeCondition = ""$$NUMITEMS:LedgerEntries>0"")]
public class ELedgerEntry : LedgerEntry
{
    public string NewProp { get; set; }
}
";
        await VerifyTDLReportV2.VerifyGeneratorAsync(src,
           ("UnitTests.TestBasic.Voucher.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Voucher
*/
partial class Voucher : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string VoucherTypeName_PPEC_FieldName = ""VoucherTypeName_PPEC"";
    const string VoucherNumber_VW4Y_FieldName = ""VoucherNumber_VW4Y"";
    const string LedgerName_YBOB_FieldName = ""LedgerName_YBOB"";
    const string NewProp_LUEW_FieldName = ""NewProp_LUEW"";
    const string StockItemName_DU48_FieldName = ""StockItemName_DU48"";
    const string LedgerEntries_OO83_PartName = ""LedgerEntries_OO83"";
    const string LedgerEntries_SLOU_PartName = ""LedgerEntries_SLOU"";
    const string InventoryEntry_OVLF_PartName = ""InventoryEntry_OVLF"";
    const string AccountingEntries_7ZOG_PartName = ""AccountingEntries_7ZOG"";
    const string ReportName = ""Voucher_IEQM"";
    const string CollectionName = ""VouchersCollection_IEQM"";
    const string XMLTag = ""VOUCHER"";
    const int SimpleFieldsCount = 5;
    const int ComplexFieldsCount = 4;
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
        return reqEnvelope;
    }

    public static global::System.Xml.Serialization.XmlAttributeOverrides GetXMLAttributeOverides()
    {
        var xmlAttributeOverrides = new global::System.Xml.Serialization.XmlAttributeOverrides();
        var XmlAttributes = new global::System.Xml.Serialization.XmlAttributes();
        XmlAttributes.XmlElements.Add(new(XMLTag));
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::UnitTests.TestBasic.Voucher>.TypeInfo, ""Objects"", XmlAttributes);
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
        parts[0] = new(LedgerEntries_OO83_PartName, ""AllLedgerEntries"");
        parts[1] = new(LedgerEntries_SLOU_PartName, ""LedgerEntries"");
        parts[2] = new(InventoryEntry_OVLF_PartName, ""INVENTORYALLOCATIONS"");
        parts[3] = new(AccountingEntries_7ZOG_PartName, ""ACCOUNTINGALLOCATIONS"");
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [VoucherTypeName_PPEC_FieldName,VoucherNumber_VW4Y_FieldName], XMLTag)
        {
            Explode = [$""{LedgerEntries_OO83_PartName}:{string.Format(""$$NUMITEMS:AllLedgerEntries>0"", ""LedgerEntries"")}"", $""{LedgerEntries_SLOU_PartName}:{string.Format(""$$NUMITEMS:AllLedgerEntries>0"", ""LedgerEntries"")}"", $""{InventoryEntry_OVLF_PartName}:{string.Format(""$$NUMITEMS:INVENTORYALLOCATIONS>0"", ""InventoryEntry"")}""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(LedgerEntries_OO83_PartName, [LedgerName_YBOB_FieldName], ""ALLILEDGERENTRIES.LIST"");
        _lines[1] = new(LedgerEntries_SLOU_PartName, [LedgerName_YBOB_FieldName,NewProp_LUEW_FieldName], ""LEDGERENTRIES.LIST"");
        _lines[2] = new(InventoryEntry_OVLF_PartName, [StockItemName_DU48_FieldName], ""InventoryEntry"")
        {
            Explode = [$""{AccountingEntries_7ZOG_PartName}:{string.Format(""$$NUMITEMS:ACCOUNTINGALLOCATIONS>0"", ""AccountingEntries"")}""]
        };
        _lines[3] = new(AccountingEntries_7ZOG_PartName, [LedgerName_YBOB_FieldName,NewProp_LUEW_FieldName], ""AccountingEntries"");
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(VoucherTypeName_PPEC_FieldName, ""VOUCHERTYPENAME"", ""$VoucherTypeName"");
        _fields[1] = new(VoucherNumber_VW4Y_FieldName, ""VOUCHERNUMBER"", ""$VoucherNumber"");
        _fields[2] = new(LedgerName_YBOB_FieldName, ""LEDGERNAME"", ""$LedgerName"");
        _fields[3] = new(NewProp_LUEW_FieldName, ""NEWPROP"", ""$NewProp"");
        _fields[4] = new(StockItemName_DU48_FieldName, ""STOCKITEMNAME"", ""$StockItemName"");
        return _fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, ""Voucher"", nativeFields: [..GetFetchList()]);
        return collections;
    }

    internal static string[] GetFetchList()
    {
        return[""VoucherTypeName"", ""VoucherNumber"", ""AllLedgerEntries.LedgerName"", ""LedgerEntries.LedgerName,LedgerEntries.NewProp"", ""INVENTORYALLOCATIONS.StockItemName"", ""INVENTORYALLOCATIONS.ACCOUNTINGALLOCATIONS.LedgerName,INVENTORYALLOCATIONS.ACCOUNTINGALLOCATIONS.NewProp""];
    }
}"));
    }

}
