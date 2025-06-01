namespace UnitTests.AdvanceTests;
[TestClass]
public class NestedComplexPropertyTests
{
    [TestMethod]
    public async Task TestNestedComplexProperties()
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

    [TDLCollection(CollectionName = ""DummyCollection"")]
    public Level1 Level1 { get; set; }
}
public class Level1
{

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
    const string Level1_F2RA_PartName = ""Level1_F2RA"";
    const string MultiAddress_DWRA_PartName = ""MultiAddress_DWRA"";
    const string AddressLines_VE9R_PartName = ""AddressLines_VE9R"";
    const string ReportName = ""Ledger_BUL5"";
    const string CollectionName = ""LedgersCollection_BUL5"";
    const string XMLTag = ""LEDGER"";
    const int SimpleFieldsCount = 3;
    const int ComplexFieldsCount = 3;
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
        parts[0] = new(Level1_F2RA_PartName, ""DummyCollection"");
        parts[1] = new(MultiAddress_DWRA_PartName, ""MULTIADRESS"");
        parts[2] = new(AddressLines_VE9R_PartName, ""ADDRESS"")
        {
            XMLTag = ""ADDRESS.LIST""
        };
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [Name_39DV_FieldName,Parent_DR40_FieldName], XMLTag)
        {
            Explode = [$""{Level1_F2RA_PartName}:YES""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(Level1_F2RA_PartName, [""SimpleField""], ""Level1"")
        {
            Explode = [$""{MultiAddress_DWRA_PartName}:YES""]
        };
        _lines[1] = new(MultiAddress_DWRA_PartName, [""SimpleField""], ""MultiAddress"")
        {
            Explode = [$""{AddressLines_VE9R_PartName}:YES""]
        };
        _lines[2] = new(AddressLines_VE9R_PartName, [AddressLines_VE9R_FieldName]);
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

    public static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, ""Ledger"", nativeFields: [..GetFetchList()]);
        return collections;
    }

    public static string[] GetFetchList()
    {
        return[""Name"", ""Parent"", ""DummyCollection.MULTIADRESS.ADDRESS""];
    }
}"));
    }

    public async Task TestcomplexPropertiesRepeated()
    {

    }
}
