using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.BasicTests;
[TestClass]
public class TestsWithComplexProperties
{
    public TestsWithComplexProperties()
    {
       
    }

    [TestMethod]
    public async Task VerifyBasicClasswithComplexPropertyWithnonPartialWithNoInheritance()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System;
namespace UnitTests.TestBasic;

[ImplementTallyRequestableObject]
[TDLCollection(Type = ""Ledger"")]
public partial class Ledger
{
    public string Name { get; set; }
    public string Parent { get; set; }

   [XmlElement(ElementName = ""LEDGSTREGDETAILS.LIST"")]
    [TDLCollection(CollectionName = ""LEDGSTREGDETAILS"", ExplodeCondition = ""$$NUMITEMS:LEDGSTREGDETAILS>0"")]
    public List<LedgGSTRegDetail> GSTRegistrationDetails { get; set; }
}

public class LedgGSTRegDetail
{
    [XmlElement(""APPLICABLEFROM"")]
    public DateTime ApplicableFrom { get; set; }

    [XmlElement(""STATE"")]
    public string State { get; set; }

    [XmlElement(""PLACEOFSUPPLY"")]
    public string PlaceOfSupply { get; set; }
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
    const string ApplicableFrom_JWGQ_FieldName = ""ApplicableFrom_JWGQ"";
    const string State_JGLV_FieldName = ""State_JGLV"";
    const string PlaceOfSupply_SKQX_FieldName = ""PlaceOfSupply_SKQX"";
    const string GSTRegistrationDetails_UPZF_PartName = ""GSTRegistrationDetails_UPZF"";
    const string ReportName = ""Ledger_BUL5"";
    const string CollectionName = ""LedgersCollection_BUL5"";
    const string XMLTag = ""LEDGER"";
    const int SimpleFieldsCount = 5;
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
        parts[0] = new(GSTRegistrationDetails_UPZF_PartName, ""LEDGSTREGDETAILS"");
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [Name_39DV_FieldName,Parent_DR40_FieldName], XMLTag)
        {
            Explode = [$""{GSTRegistrationDetails_UPZF_PartName}:{string.Format(""$$NUMITEMS:LEDGSTREGDETAILS>0"", ""GSTRegistrationDetails"")}""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(GSTRegistrationDetails_UPZF_PartName, [ApplicableFrom_JWGQ_FieldName,State_JGLV_FieldName,PlaceOfSupply_SKQX_FieldName], ""LEDGSTREGDETAILS.LIST"");
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_39DV_FieldName, ""NAME"", ""$Name"");
        _fields[1] = new(Parent_DR40_FieldName, ""PARENT"", ""$Parent"");
        _fields[2] = new(ApplicableFrom_JWGQ_FieldName, ""APPLICABLEFROM"", ""$APPLICABLEFROM"");
        _fields[3] = new(State_JGLV_FieldName, ""STATE"", ""$STATE"");
        _fields[4] = new(PlaceOfSupply_SKQX_FieldName, ""PLACEOFSUPPLY"", ""$PLACEOFSUPPLY"");
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
        return[""Name"", ""Parent"", ""LEDGSTREGDETAILS.APPLICABLEFROM,LEDGSTREGDETAILS.STATE,LEDGSTREGDETAILS.PLACEOFSUPPLY""];
    }
}"));
    }

    


}