namespace UnitTests.BasicTests;

[TestClass]
public class BasicTestsWithSimpleProperties
{

    [TestMethod]
    public async Task VerifyBasicClassWithNoInheritance()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
namespace UnitTests.TestBasic;

[ImplementTallyRequestableObject]
[TDLCollection(Type = ""Ledger"")]
public partial class Ledger
{
    public string Name { get; set; }
    public string Parent { get; set; }
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
    const string ReportName = ""Ledger_BUL5"";
    const string CollectionName = ""LedgersCollection_BUL5"";
    const string XMLTag = ""LEDGER"";
    const int SimpleFieldsCount = 2;
    const int ComplexFieldsCount = 0;
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
        return[];
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [Name_39DV_FieldName,Parent_DR40_FieldName], XMLTag);
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        return[];
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_39DV_FieldName, ""NAME"", ""$Name"");
        _fields[1] = new(Parent_DR40_FieldName, ""PARENT"", ""$Parent"");
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
        return[""Name"", ""Parent""];
    }
}"));
    }
    [TestMethod]
    public async Task VerifyBasicClassWithInheritance()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using System.Xml;
using System.Xml.Serialization;
namespace UnitTests.TestBasic;


public partial class LedgerBase
{
  [XmlElement(ElementName = ""NAME"")]
    public string Name { get; set; }
  [XmlElement(ElementName = ""PARENT"")]
    public string Parent { get; set; }
}

[ImplementTallyRequestableObject]
public partial class Ledger : LedgerBase
{
    [XmlElement(ElementName = ""ISBILLWISEON"")]
    public bool IsBillWise { get; set; }
}
";
        await VerifyTDLReportV2.VerifyGeneratorAsync(src,
            [
            ("UnitTests.TestBasic.Ledger.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Ledger
*/
partial class Ledger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string Name_VO4U_FieldName = ""Name_VO4U"";
    const string Parent_V6LJ_FieldName = ""Parent_V6LJ"";
    const string IsBillWise_PTST_FieldName = ""IsBillWise_PTST"";
    const string ReportName = ""Ledger_BUL5"";
    const string CollectionName = ""LedgersCollection_BUL5"";
    const string XMLTag = ""LEDGER"";
    const int SimpleFieldsCount = 3;
    const int ComplexFieldsCount = 0;
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
        return[];
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [Name_VO4U_FieldName,Parent_V6LJ_FieldName,IsBillWise_PTST_FieldName], XMLTag);
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        return[];
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_VO4U_FieldName, ""NAME"", ""$NAME"");
        _fields[1] = new(Parent_V6LJ_FieldName, ""PARENT"", ""$PARENT"");
        _fields[2] = new(IsBillWise_PTST_FieldName, ""ISBILLWISEON"", ""$ISBILLWISEON"");
        return _fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, ""LEDGER"", nativeFields: [..GetFetchList()]);
        return collections;
    }

    internal static string[] GetFetchList()
    {
        return[""NAME"", ""PARENT"", ""ISBILLWISEON""];
    }
}")]);
    }


    [TestMethod]
    public async Task VerifyBasicPropertiesOveridden()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using System.Xml;
using System.Xml.Serialization;
namespace UnitTests.TestBasic;


public partial class LedgerBase
{
    [XmlElement(ElementName = ""NAME"")]
    public string Name { get; set; }
  [XmlElement(ElementName = ""PARENT"")]
    public string Parent { get; set; }
}

[ImplementTallyRequestableObject]
public partial class Ledger : LedgerBase
{


[XmlElement(ElementName = ""OVERIDDENNAME"")]
    public new string Name { get; set; }

    [XmlElement(ElementName = ""ISBILLWISEON"")]
    public bool IsBillWise { get; set; }
}
";
        await VerifyTDLReportV2.VerifyGeneratorAsync(src,
            [
            ("UnitTests.TestBasic.Ledger.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic;
/*
* Generated based on UnitTests.TestBasic.Ledger
*/
partial class Ledger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string Parent_V6LJ_FieldName = ""Parent_V6LJ"";
    const string Name_39DV_FieldName = ""Name_39DV"";
    const string IsBillWise_PTST_FieldName = ""IsBillWise_PTST"";
    const string ReportName = ""Ledger_BUL5"";
    const string CollectionName = ""LedgersCollection_BUL5"";
    const string XMLTag = ""LEDGER"";
    const int SimpleFieldsCount = 3;
    const int ComplexFieldsCount = 0;
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
        xmlAttributeOverrides.Add(typeof(global::UnitTests.TestBasic.LedgerBase), ""Name"", new global::System.Xml.Serialization.XmlAttributes() { XmlIgnore = true });
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
        return[];
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [Parent_V6LJ_FieldName,Name_39DV_FieldName,IsBillWise_PTST_FieldName], XMLTag);
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        return[];
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Parent_V6LJ_FieldName, ""PARENT"", ""$PARENT"");
        _fields[1] = new(Name_39DV_FieldName, ""OVERIDDENNAME"", ""$OVERIDDENNAME"");
        _fields[2] = new(IsBillWise_PTST_FieldName, ""ISBILLWISEON"", ""$ISBILLWISEON"");
        return _fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(CollectionName, ""LEDGER"", nativeFields: [..GetFetchList()]);
        return collections;
    }

    internal static string[] GetFetchList()
    {
        return[""PARENT"", ""OVERIDDENNAME"", ""ISBILLWISEON""];
    }
}")]);
    }
}

