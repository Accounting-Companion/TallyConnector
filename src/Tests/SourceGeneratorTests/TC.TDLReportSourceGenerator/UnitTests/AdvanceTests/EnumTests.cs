namespace UnitTests.AdvanceTests;
[TestClass]
public class EnumTests
{
    /// <summary>
    /// This test case to Nameset is not repeated even Enum is repeated twice in same model
    /// Ex: if Ledger.Address.GSTDealerType (GSTRegistrationType-ENUM) and 
    /// Ledger.GSTRegistrationDetails.GSTRegistrationType (GSTRegistrationType-ENUM)
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEnumRepeatedTwiceInSameModel_GetTDLReport()
    {

        var src = @"
        using TallyConnector.Core.Attributes.SourceGenerator;
        using TallyConnector.Core.Attributes;
        using System.Xml;
        using System.Xml.Serialization;
        using System.Collections.Generic;
        using static TallyConnector.Core.Constants;
        
        namespace TallyConnector.Core.Models.Temp
        {

        [TDLCollection(Type = ""Ledger"")]
        [XmlRoot(""LEDGER"")]
        [XmlType(AnonymousType = true)]
        [ImplementTallyRequestableObject]
        public partial class TLedger
        {       
            public string Name {get;set;}

            [XmlElement(ElementName = ""LEDMULTIADDRESSLIST.LIST"")]
            [TDLCollection(CollectionName = ""LEDMULTIADDRESSLIST"",ExplodeCondition = ""$$NUMITEMS:LEDMULTIADDRESSLIST>0"")]

            public List<TMultiAddress> Addresses { get; set; }

            [XmlElement(ElementName = ""LEDGSTREGDETAILS.LIST"")]
            [TDLCollection(CollectionName = ""LEDGSTREGDETAILS"", ExplodeCondition = ""$$NUMITEMS:LEDGSTREGDETAILS>0"")]
            public List<TLedgerGSTRegistrationDetails> GSTRegistrationDetails { get; set; }
        }


        public class TMultiAddress
        {
           [XmlElement(ElementName = ""GSTREGISTRATIONTYPE"")]
            public TGSTRegistrationType GSTDealerType { get; set; }
        }
        public class TLedgerGSTRegistrationDetails
        {
           [XmlElement(ElementName = ""REGISTRATIONTYPE"")]
            public TGSTRegistrationType GSTRegistrationType { get; set; }
        }
        public enum TGSTRegistrationType
        {

            [EnumXMLChoice(Choice = """")]
            None = 0,
            [EnumXMLChoice(Choice = ""Unknown"")]
            Unknown = 1,
            [EnumXMLChoice(Choice = ""Composition"")]
            Composition = 2,

            [EnumXMLChoice(Choice = ""Consumer"", Versions = [""6.6.3"", ""1.1.1"", ""1.1.2"", ""1.1.3"", ""1.1.4"", RetiredVersions.TallyPrime.V2, RetiredVersions.TallyPrime.V2_0_1, RetiredVersions.TallyPrime.V2_1])]
            [EnumXMLChoice(Choice = ""Unregistered/Consumer"")]
            Consumer = 3,

            [EnumXMLChoice(Choice = ""Regular"")]
            Regular = 4
        }
        }

                    ";
        await VerifyTDLReport.VerifyGeneratorAsync(src, [
            ("TallyConnector.Core.Models.Temp.TLedger.cs",@"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Core.Models.Temp;
/*
* Generated based on TallyConnector.Core.Models.Temp.TLedger
*/
partial class TLedger : global::TallyConnector.Core.Models.Interfaces.ITallyRequestableObject
{
    const string Name_9QSD_FieldName = ""Name_9QSD"";
    const string GSTDealerType_QH8A_FieldName = ""GSTDealerType_QH8A"";
    const string GSTRegistrationType_JQFK_FieldName = ""GSTRegistrationType_JQFK"";
    const string Addresses_MUJR_PartName = ""Addresses_MUJR"";
    const string GSTRegistrationDetails_DVBQ_PartName = ""GSTRegistrationDetails_DVBQ"";
    const string ReportName = ""TLedger_R1ZB"";
    const string CollectionName = ""TLedgersCollection_R1ZB"";
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
        tdlMsg.NameSet = [..GetTDLNameSets()];
        return reqEnvelope;
    }

    public static global::System.Xml.Serialization.XmlAttributeOverrides GetXMLAttributeOverides()
    {
        var xmlAttributeOverrides = new global::System.Xml.Serialization.XmlAttributeOverrides();
        var XmlAttributes = new global::System.Xml.Serialization.XmlAttributes();
        XmlAttributes.XmlElements.Add(new(XMLTag));
        xmlAttributeOverrides.Add(TallyConnector.Core.Models.Response.ReportResponseEnvelope<global::TallyConnector.Core.Models.Temp.TLedger>.TypeInfo, ""Objects"", XmlAttributes);
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
        parts[0] = new(Addresses_MUJR_PartName, ""LEDMULTIADDRESSLIST"");
        parts[1] = new(GSTRegistrationDetails_DVBQ_PartName, ""LEDGSTREGDETAILS"");
        return parts;
    }

    public static global::TallyConnector.Core.Models.Request.Line GetMainTDLLine()
    {
        return new(ReportName, [Name_9QSD_FieldName], XMLTag)
        {
            Explode = [$""{Addresses_MUJR_PartName}:{string.Format(""$$NUMITEMS:LEDMULTIADDRESSLIST>0"", ""$LEDMULTIADDRESSLIST.LIST"")}"", $""{GSTRegistrationDetails_DVBQ_PartName}:{string.Format(""$$NUMITEMS:LEDGSTREGDETAILS>0"", ""$LEDGSTREGDETAILS.LIST"")}""]
        };
    }

    public static global::TallyConnector.Core.Models.Request.Line[] GetTDLLines()
    {
        var _lines = new global::TallyConnector.Core.Models.Request.Line[ComplexFieldsCount];
        _lines[0] = new(Addresses_MUJR_PartName, [GSTDealerType_QH8A_FieldName], ""LEDMULTIADDRESSLIST.LIST"");
        _lines[1] = new(GSTRegistrationDetails_DVBQ_PartName, [GSTRegistrationType_JQFK_FieldName], ""LEDGSTREGDETAILS.LIST"");
        return _lines;
    }

    public static global::TallyConnector.Core.Models.Request.Field[] GetTDLFields()
    {
        var _fields = new global::TallyConnector.Core.Models.Request.Field[SimpleFieldsCount];
        _fields[0] = new(Name_9QSD_FieldName, ""NAME"", ""$Name"");
        _fields[1] = new(GSTDealerType_QH8A_FieldName, ""GSTREGISTRATIONTYPE"", ""$$NameGetValue:$GSTREGISTRATIONTYPE:TGSTRegistrationType_XDKO"")
        {
            Invisible = ""$$ISEmpty:$$value""
        };
        _fields[2] = new(GSTRegistrationType_JQFK_FieldName, ""REGISTRATIONTYPE"", ""$$NameGetValue:$REGISTRATIONTYPE:TGSTRegistrationType_XDKO"")
        {
            Invisible = ""$$ISEmpty:$$value""
        };
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
        return[""Name"", ""LEDMULTIADDRESSLIST.GSTREGISTRATIONTYPE"", ""LEDGSTREGDETAILS.REGISTRATIONTYPE""];
    }

    public static global::TallyConnector.Core.Models.Request.NameSet[] GetTDLNameSets()
    {
        var namesets = new global::TallyConnector.Core.Models.Request.NameSet[1];
        namesets[0] = new(""TGSTRegistrationType_XDKO"")
        {
            List = [""Unknown:\""Unknown\"""", ""Composition:\""Composition\"""", ""Consumer:\""Consumer\"""", ""Unregistered/Consumer:\""Consumer\"""", ""Regular:\""Regular\""""]
        };
        return namesets;
    }
}")]);

    }


}
