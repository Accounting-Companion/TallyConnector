namespace UnitTests.AdvanceTests.Inheritance;
[TestClass]
public class InhertanceOveriddenTests
{
    [TestMethod]
    public async Task TestSameBaseClassUsedTwice()
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
public partial class Root 
{

    [TDLCollection(CollectionName=""Col1"")]
    public BaseUsage1 RootProp1 {get;set;}


    public BaseUsage2 RootProp2 { get; set; }

    public string RootProp3 { get; set; }
}
public class BaseUsage1:CommonBase
{
}
 [TDLCollection(CollectionName=""Col2"")]
public class BaseUsage2:CommonBase
{
}
public class CommonBase
{ 
    public  string BaseProperty1 { get; set; }
}

";
        await VerifyTDLReport.VerifyGeneratorAsync(src,
           ("UnitTests.TestBasic.Root.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Root
*/
partial class Root : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string RootProp3_J0QI_FieldName = ""RootProp3_J0QI"";
    const string BaseProperty1_UHE3_FieldName = ""BaseProperty1_UHE3"";
    const string RootProp1_BVGT_PartName = ""RootProp1_BVGT"";
    const string RootProp2_7LFJ_PartName = ""RootProp2_7LFJ"";
    const string ReportName = ""Root_KFZG"";
    const string CollectionName = ""RootsCollection_KFZG"";
    const string XMLTag = ""ROOT"";
    const int SimpleFieldsCount = 2;
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
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::UnitTests.TestBasic.Root>.TypeInfo, ""Objects"", XmlAttributes);
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
        parts[0] = new(RootProp1_BVGT_PartName, ""Col1"");
        parts[1] = new(RootProp2_7LFJ_PartName, ""Col2"");
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [RootProp3_J0QI_FieldName], XMLTag)
        {
            Explode = [$""{RootProp1_BVGT_PartName}:YES"", $""{RootProp2_7LFJ_PartName}:YES""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(RootProp1_BVGT_PartName, [BaseProperty1_UHE3_FieldName], ""RootProp1"");
        _lines[1] = new(RootProp2_7LFJ_PartName, [BaseProperty1_UHE3_FieldName], ""RootProp2"");
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(RootProp3_J0QI_FieldName, ""ROOTPROP3"", ""$RootProp3"");
        _fields[1] = new(BaseProperty1_UHE3_FieldName, ""BASEPROPERTY1"", ""$BaseProperty1"");
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
        return[""RootProp3"", ""Col1.BaseProperty1"", ""Col2.BaseProperty1""];
    }
}"));
    }
    [TestMethod]
    public async Task TestSameBaseClassUsedTwiceandOverriddenOnce()
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
public partial class Root :BaseRoot
{
    [TDLCollection(CollectionName=""Col1"")]
    public new BaseUsage2 RootProp1 {get;set;}


    public string RootProp3 { get; set; }
}
public partial class BaseRoot 
{

    [TDLCollection(CollectionName=""Col1"")]
    public BaseUsage1 RootProp1 {get;set;}

}
public class BaseUsage1:CommonBase
{
public new int BaseProperty1 { get; set; }
public  int BaseUsageProperty2 { get; set; }
}
 [TDLCollection(CollectionName=""Col2"")]
public class BaseUsage2:CommonBase
{

}
public class CommonBase
{ 
    public  string BaseProperty1 { get; set; }
}

";
        await VerifyTDLReport.VerifyGeneratorAsync(src,
           ("UnitTests.TestBasic.Root.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Root
*/
partial class Root : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string RootProp3_J0QI_FieldName = ""RootProp3_J0QI"";
    const string BaseProperty1_UHE3_FieldName = ""BaseProperty1_UHE3"";
    const string RootProp1_BVGT_PartName = ""RootProp1_BVGT"";
    const string RootProp2_7LFJ_PartName = ""RootProp2_7LFJ"";
    const string ReportName = ""Root_KFZG"";
    const string CollectionName = ""RootsCollection_KFZG"";
    const string XMLTag = ""ROOT"";
    const int SimpleFieldsCount = 2;
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
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::UnitTests.TestBasic.Root>.TypeInfo, ""Objects"", XmlAttributes);
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
        parts[0] = new(RootProp1_BVGT_PartName, ""Col1"");
        parts[1] = new(RootProp2_7LFJ_PartName, ""Col2"");
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [RootProp3_J0QI_FieldName], XMLTag)
        {
            Explode = [$""{RootProp1_BVGT_PartName}:YES"", $""{RootProp2_7LFJ_PartName}:YES""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(RootProp1_BVGT_PartName, [BaseProperty1_UHE3_FieldName], ""RootProp1"");
        _lines[1] = new(RootProp2_7LFJ_PartName, [BaseProperty1_UHE3_FieldName], ""RootProp2"");
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(RootProp3_J0QI_FieldName, ""ROOTPROP3"", ""$RootProp3"");
        _fields[1] = new(BaseProperty1_UHE3_FieldName, ""BASEPROPERTY1"", ""$BaseProperty1"");
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
        return[""RootProp3"", ""Col1.BaseProperty1"", ""Col2.BaseProperty1""];
    }
}"));
    }

}
