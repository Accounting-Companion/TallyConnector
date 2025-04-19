using System.Reflection;

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
using TallyConnector.Core.Models.Temp;
using TallyConnector.Services;
using TallyConnector.Core.Models;
using static TallyConnector.Core.Constants;
using TallyConnector.Core.Models.Common;
using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
namespace TallyConnector.Services.Temp
{
[GenerateHelperMethod<TLedger>(GenerationMode = GenerationMode.GetMultiple)]
[ImplementTallyService(nameof(_baseHandler))]
public partial class TallyPrimeService : TallyCommonService
{
    public TallyPrimeService()
    {
    }

    public TallyPrimeService(IBaseTallyService baseTallyService) : base(baseTallyService)
    {
    }

    public TallyPrimeService(ILogger logger, IBaseTallyService baseTallyService) : base(logger, baseTallyService)
    {
    }
}

}


namespace TallyConnector.Core.Models.Temp
{


[TDLCollection(Type = ""Ledger"")]
[XmlRoot(""LEDGER"")]
[XmlType(AnonymousType = true)]
public class TLedger : BaseMasterObject
{
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
   [XmlElement(ElementName = ""GSTREGISTRATIONTYPE"")]
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
        new TallyConnector.Core.Models.Request.Field() { IsModify = TallyConnector.Core.Models.Common.YesNo.No }
        ;
        await VerifyTDLReportSG.VerifyGeneratorAsync(src,
            [
                ("TallyConnector.Core.Models.Temp.TLedger.TallyPrimeService_TST.TDLReport.g.cs",
                @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Services.Temp;
/*
 * Generated based on TallyConnector.Core.Models.Temp.TLedger
 */
partial class TallyPrimeService
{
    internal const string TLedgerAddressesReportName = ""TC_TLedgerAddressesList"";
    const string TLedgerTMultiAddressCollectionName = ""LEDMULTIADDRESSLIST"";
    internal const string TLedgerGSTRegistrationDetailsReportName = ""TC_TLedgerGSTRegistrationDetailsList"";
    const string TLedgerTLedgerGSTRegistrationDetailsCollectionName = ""LEDGSTREGDETAILS"";
    internal const string TLedgerReportName = ""TC_TLedgerList"";
    const string TLedgerCollectionName = ""TC_TLedgerCollection"";
    const string TLedgerCollectionNamePaginated = ""TC_TLedgerCollection_Paginated"";
    public async global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<global::TallyConnector.Core.Models.Temp.TLedger>> GetTLedgersAsync(global::TallyConnector.Core.Models.Request.RequestOptions reqOptions, global::System.Threading.CancellationToken token = default)
    {
        var reqType = ""Getting TLedgers"";
        var reqEnvelope = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerRequestEnevelope();
        if (reqOptions != null)
        {
            reqEnvelope.PopulateOptions(reqOptions);
        }

        await _baseHandler.PopulateDefaultOptions(reqEnvelope, token);
        var reqXml = reqEnvelope.GetXML();
        var resp = await _baseHandler.SendRequestAsync(reqXml, reqType, token);
        var respEnv = global::TallyConnector.Services.XMLToObject.GetObjfromXml<global::TallyConnector.Services.Temp.Models.TallyPrimeServiceReportResponseEnvelopeForTLedger>(resp.Response!, GetTLedgerXMLAttributeOverides(), _logger);
        return respEnv.Objects;
    }

    public async global::System.Threading.Tasks.Task<global::TallyConnector.Core.Models.Common.Pagination.PaginatedResponse<global::TallyConnector.Core.Models.Temp.TLedger>> GetTLedgersAsync(global::TallyConnector.Core.Models.Request.PaginatedRequestOptions? reqOptions = null, global::System.Threading.CancellationToken token = default)
    {
        var reqType = ""Getting TLedgers"";
        var reqEnvelope = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerRequestEnevelope();
        reqEnvelope.PopulateOptions(reqOptions);
        await _baseHandler.PopulateDefaultOptions(reqEnvelope, token);
        var reqXml = reqEnvelope.GetXML();
        var resp = await _baseHandler.SendRequestAsync(reqXml, reqType, token);
        var respEnv = global::TallyConnector.Services.XMLToObject.GetObjfromXml<global::TallyConnector.Services.Temp.Models.TallyPrimeServiceReportResponseEnvelopeForTLedger>(resp.Response!, GetTLedgerXMLAttributeOverides(), _logger);
        return new(respEnv.TotalCount ?? 0, reqOptions?.RecordsPerPage ?? 1000, respEnv.Objects, reqOptions?.PageNum ?? 1);
    }

    public global::System.Xml.Serialization.XmlAttributeOverrides GetTLedgerXMLAttributeOverides()
    {
        var xmlAttributeOverrides = new global::System.Xml.Serialization.XmlAttributeOverrides();
        return xmlAttributeOverrides;
    }

    internal static global::TallyConnector.Core.Models.Request.RequestEnvelope GetTLedgerRequestEnevelope()
    {
        var reqEnvelope = new global::TallyConnector.Core.Models.Request.RequestEnvelope(global::TallyConnector.Core.Models.Request.HType.Data, TLedgerReportName);
        var tdlMsg = reqEnvelope.Body.Desc.TDL.TDLMessage;
        tdlMsg.Report = [new(TLedgerReportName)];
        tdlMsg.Form = [new(TLedgerReportName)];
        tdlMsg.Part = [..global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerTDLParts()];
        tdlMsg.Line = [..global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerTDLLines()];
        tdlMsg.Field = [..global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerTDLFields()];
        tdlMsg.Collection = [..global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerTDLCollections()];
        tdlMsg.Functions = [..global::TallyConnector.Services.BaseTallyService.GetDefaultTDLFunctions(), ..global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTMultiAddressChildTGSTRegistrationTypeTDLFunctions()];
        tdlMsg.NameSet = [..global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTMultiAddressChildTGSTRegistrationTypeTDLNameSets()];
        tdlMsg.Object = [];
        return reqEnvelope;
    }

    internal static global::TallyConnector.Core.Models.Request.Part GetTLedgerMainTDLPart()
    {
        return new(TLedgerReportName, TLedgerCollectionName);
    }

    internal static global::TallyConnector.Core.Models.Request.Part[] GetTLedgerTDLParts()
    {
        var parts = new global::TallyConnector.Core.Models.Request.Part[3];
        parts[0] = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerMainTDLPart();
        var baseMasterObjectParts = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerBaseBaseMasterObjectTDLParts();
        parts.AddToArray(baseMasterObjectParts, 1);
        var addressesParts = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTMultiAddressTDLParts(TLedgerAddressesReportName, TLedgerTMultiAddressCollectionName);
        parts.AddToArray(addressesParts, 1);
        var gSTRegistrationDetailsParts = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTLedgerGSTRegistrationDetailsTDLParts(TLedgerGSTRegistrationDetailsReportName, TLedgerTLedgerGSTRegistrationDetailsCollectionName);
        parts.AddToArray(gSTRegistrationDetailsParts, 2);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Request.Line[] GetTLedgerTDLLines()
    {
        var lines = new global::TallyConnector.Core.Models.Request.Line[8];
        lines[0] = new(TLedgerReportName, [], ""LEDGER"")
        {
            Explode = [$""{TLedgerAddressesReportName}:{string.Format(""$$NUMITEMS:LEDMULTIADDRESSLIST>0"", ""$LEDMULTIADDRESSLIST.LIST"")}"", $""{TLedgerGSTRegistrationDetailsReportName}:{string.Format(""$$NUMITEMS:LEDGSTREGDETAILS>0"", ""$LEDGSTREGDETAILS.LIST"")}""],
            Use = TLedgerBaseBaseMasterObjectReportName
        };
        var baseMasterObjectLines = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerBaseBaseMasterObjectTDLLines();
        lines.AddToArray(baseMasterObjectLines, 1);
        lines[4] = new(TLedgerAddressesReportName, [], ""LEDMULTIADDRESSLIST.LIST"")
        {
            Use = TLedgerChildTMultiAddressReportName,
            Local = []
        };
        var addressesLines = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTMultiAddressTDLLines(""LEDMULTIADDRESSLIST.LIST"");
        lines.AddToArray(addressesLines, 5);
        lines[6] = new(TLedgerGSTRegistrationDetailsReportName, [], ""LEDGSTREGDETAILS.LIST"")
        {
            Use = TLedgerChildTLedgerGSTRegistrationDetailsReportName,
            Local = []
        };
        var gSTRegistrationDetailsLines = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTLedgerGSTRegistrationDetailsTDLLines(""LEDGSTREGDETAILS.LIST"");
        lines.AddToArray(gSTRegistrationDetailsLines, 7);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Request.Field[] GetTLedgerTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Request.Field[10];
        var baseMasterObjectFields = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerBaseBaseMasterObjectTDLFields();
        fields.AddToArray(baseMasterObjectFields, 0);
        var addressesFields = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTMultiAddressTDLFields();
        fields.AddToArray(addressesFields, 8);
        var gSTRegistrationDetailsFields = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTLedgerGSTRegistrationDetailsTDLFields();
        fields.AddToArray(gSTRegistrationDetailsFields, 9);
        return fields;
    }

    internal static global::TallyConnector.Core.Models.Request.Collection[] GetTLedgerTDLCollections()
    {
        var collections = new global::TallyConnector.Core.Models.Request.Collection[1];
        collections[0] = new(TLedgerCollectionName, ""Ledger"", nativeFields: [..GetTLedgerFetchList()]);
        return collections;
    }

    internal static string[] GetTLedgerFetchList()
    {
        return[..GetTLedgerBaseBaseMasterObjectFetchList(), ..GetTLedgerChildTMultiAddressFetchList(TLedgerTMultiAddressCollectionName), ..GetTLedgerChildTLedgerGSTRegistrationDetailsFetchList(TLedgerTLedgerGSTRegistrationDetailsCollectionName)];
    }
}"),
                ("TallyConnector.Core.Models.BaseMasterObject.TallyPrimeService_TST.TDLReport.g.cs",
                @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Services.Temp;
/*
 * Generated based on TallyConnector.Core.Models.BaseMasterObject
 */
partial class TallyPrimeService
{
    internal const string TLedgerBaseBaseMasterObjectNameTDLFieldName = ""TC_BaseMasterObject_Name"";
    internal const string TLedgerBaseBaseMasterObjectReportName = ""TC_TLedgerBaseBaseMasterObjectList"";
    const string TLedgerBaseBaseMasterObjectCollectionName = ""BaseMasterObject"";
    const string TLedgerBaseBaseMasterObjectCollectionNamePaginated = ""BaseMasterObject"";
    internal static global::TallyConnector.Core.Models.Request.Part GetTLedgerBaseBaseMasterObjectMainTDLPart(string partName = TLedgerBaseBaseMasterObjectReportName, string? collectionName = TLedgerBaseBaseMasterObjectCollectionName, string? xmlTag = null)
    {
        return new(partName, collectionName, partName)
        {
            XMLTag = xmlTag
        };
    }

    internal static global::TallyConnector.Core.Models.Request.Part[] GetTLedgerBaseBaseMasterObjectTDLParts(string partName = TLedgerBaseBaseMasterObjectReportName, string? collectionName = TLedgerBaseBaseMasterObjectCollectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Request.Part[0];
        var tallyObjectParts = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerBaseBaseMasterObjectBaseTallyObjectTDLParts();
        parts.AddToArray(tallyObjectParts, 0);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Request.Line[] GetTLedgerBaseBaseMasterObjectTDLLines(string xmlTag = ""BASEMASTEROBJECT"")
    {
        var lines = new global::TallyConnector.Core.Models.Request.Line[3];
        lines[0] = new(TLedgerBaseBaseMasterObjectReportName, [TLedgerBaseBaseMasterObjectNameTDLFieldName], xmlTag)
        {
            Use = TLedgerBaseBaseMasterObjectBaseTallyObjectReportName
        };
        var tallyObjectLines = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerBaseBaseMasterObjectBaseTallyObjectTDLLines();
        lines.AddToArray(tallyObjectLines, 1);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Request.Field[] GetTLedgerBaseBaseMasterObjectTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Request.Field[8];
        var tallyObjectFields = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerBaseBaseMasterObjectBaseTallyObjectTDLFields();
        fields.AddToArray(tallyObjectFields, 0);
        fields[7] = new(TLedgerBaseBaseMasterObjectNameTDLFieldName, ""NAME"", ""$NAME"");
        return fields;
    }

    internal static string[] GetTLedgerBaseBaseMasterObjectFetchList()
    {
        return[..GetTLedgerBaseBaseMasterObjectBaseTallyObjectFetchList(), ""NAME""];
    }
}"),
                ("TallyConnector.Core.Models.Base.TallyObject.TallyPrimeService_TST.TDLReport.g.cs",
                @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Services.Temp;
/*
 * Generated based on TallyConnector.Core.Models.Base.TallyObject
 */
partial class TallyPrimeService
{
    internal const string TLedgerBaseBaseMasterObjectBaseTallyObjectMasterIdTDLFieldName = ""TC_TallyObject_MasterId"";
    internal const string TLedgerBaseBaseMasterObjectBaseTallyObjectAlterIdTDLFieldName = ""TC_TallyObject_AlterId"";
    internal const string TLedgerBaseBaseMasterObjectBaseTallyObjectEnteredByTDLFieldName = ""TC_TallyObject_EnteredBy"";
    internal const string TLedgerBaseBaseMasterObjectBaseTallyObjectAlteredByTDLFieldName = ""TC_TallyObject_AlteredBy"";
    internal const string TLedgerBaseBaseMasterObjectBaseTallyObjectReportName = ""TC_TLedgerBaseBaseMasterObjectBaseTallyObjectList"";
    const string TLedgerBaseBaseMasterObjectBaseTallyObjectCollectionName = ""TallyObject"";
    const string TLedgerBaseBaseMasterObjectBaseTallyObjectCollectionNamePaginated = ""TallyObject"";
    internal static global::TallyConnector.Core.Models.Request.Part GetTLedgerBaseBaseMasterObjectBaseTallyObjectMainTDLPart(string partName = TLedgerBaseBaseMasterObjectBaseTallyObjectReportName, string? collectionName = TLedgerBaseBaseMasterObjectBaseTallyObjectCollectionName, string? xmlTag = null)
    {
        return new(partName, collectionName, partName)
        {
            XMLTag = xmlTag
        };
    }

    internal static global::TallyConnector.Core.Models.Request.Part[] GetTLedgerBaseBaseMasterObjectBaseTallyObjectTDLParts(string partName = TLedgerBaseBaseMasterObjectBaseTallyObjectReportName, string? collectionName = TLedgerBaseBaseMasterObjectBaseTallyObjectCollectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Request.Part[0];
        var baseTallyObjectParts = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectTDLParts();
        parts.AddToArray(baseTallyObjectParts, 0);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Request.Line[] GetTLedgerBaseBaseMasterObjectBaseTallyObjectTDLLines(string xmlTag = ""TALLYOBJECT"")
    {
        var lines = new global::TallyConnector.Core.Models.Request.Line[2];
        lines[0] = new(TLedgerBaseBaseMasterObjectBaseTallyObjectReportName, [TLedgerBaseBaseMasterObjectBaseTallyObjectMasterIdTDLFieldName,TLedgerBaseBaseMasterObjectBaseTallyObjectAlterIdTDLFieldName,TLedgerBaseBaseMasterObjectBaseTallyObjectEnteredByTDLFieldName,TLedgerBaseBaseMasterObjectBaseTallyObjectAlteredByTDLFieldName], xmlTag)
        {
            Use = TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectReportName
        };
        var baseTallyObjectLines = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectTDLLines();
        lines.AddToArray(baseTallyObjectLines, 1);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Request.Field[] GetTLedgerBaseBaseMasterObjectBaseTallyObjectTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Request.Field[7];
        var baseTallyObjectFields = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectTDLFields();
        fields.AddToArray(baseTallyObjectFields, 0);
        fields[3] = new(TLedgerBaseBaseMasterObjectBaseTallyObjectMasterIdTDLFieldName, ""MASTERID"", ""$MASTERID"");
        fields[4] = new(TLedgerBaseBaseMasterObjectBaseTallyObjectAlterIdTDLFieldName, ""ALTERID"", ""$ALTERID"");
        fields[5] = new(TLedgerBaseBaseMasterObjectBaseTallyObjectEnteredByTDLFieldName, ""ENTEREDBY"", ""$ENTEREDBY"")
        {
            Invisible = ""$$ISEmpty:$$value""
        };
        fields[6] = new(TLedgerBaseBaseMasterObjectBaseTallyObjectAlteredByTDLFieldName, ""ALTEREDBY"", ""$ALTEREDBY"")
        {
            Invisible = ""$$ISEmpty:$$value""
        };
        return fields;
    }

    internal static string[] GetTLedgerBaseBaseMasterObjectBaseTallyObjectFetchList()
    {
        return[..GetTLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectFetchList(), ""ALTERID, ENTEREDBY, ALTEREDBY""];
    }
}"),
                ("TallyConnector.Core.Models.Base.BaseTallyObject.TallyPrimeService_TST.TDLReport.g.cs",
                @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Services.Temp;
/*
 * Generated based on TallyConnector.Core.Models.Base.BaseTallyObject
 */
partial class TallyPrimeService
{
    internal const string TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectGUIDTDLFieldName = ""TC_BaseTallyObject_GUID"";
    internal const string TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectRemoteIdTDLFieldName = ""TC_BaseTallyObject_RemoteId"";
    internal const string TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectReportName = ""TC_TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectList"";
    const string TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectCollectionName = ""BaseTallyObject"";
    const string TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectCollectionNamePaginated = ""BaseTallyObject"";
    internal static global::TallyConnector.Core.Models.Request.Part GetTLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectMainTDLPart(string partName = TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectReportName, string? collectionName = TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectCollectionName, string? xmlTag = null)
    {
        return new(partName, collectionName, partName)
        {
            XMLTag = xmlTag
        };
    }

    internal static global::TallyConnector.Core.Models.Request.Part[] GetTLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectTDLParts(string partName = TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectReportName, string? collectionName = TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectCollectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Request.Part[0];
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Request.Line[] GetTLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectTDLLines(string xmlTag = ""BASETALLYOBJECT"")
    {
        var lines = new global::TallyConnector.Core.Models.Request.Line[1];
        lines[0] = new(TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectReportName, [TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectGUIDTDLFieldName,TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectRemoteIdTDLFieldName], xmlTag);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Request.Field[] GetTLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Request.Field[3];
        fields[0] = new(""Default"")
        {
            IsModify = TallyConnector.Core.Models.Common.YesNo.Yes
        };
        fields[1] = new(TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectGUIDTDLFieldName, ""GUID"", ""$GUID"");
        fields[2] = new(TLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectRemoteIdTDLFieldName, ""REMOTEALTGUID"", ""$REMOTEALTGUID"");
        return fields;
    }

    internal static string[] GetTLedgerBaseBaseMasterObjectBaseTallyObjectBaseBaseTallyObjectFetchList()
    {
        return[""REMOTEALTGUID""];
    }
}"),
                ("TallyConnector.Core.Models.Temp.TMultiAddress.TallyPrimeService_TST.TDLReport.g.cs",
                @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Services.Temp;
/*
 * Generated based on TallyConnector.Core.Models.Temp.TMultiAddress
 */
partial class TallyPrimeService
{
    internal const string TLedgerChildTMultiAddressGSTDealerTypeTDLFieldName = ""TC_TMultiAddress_GSTDealerType"";
    internal const string TLedgerChildTMultiAddressReportName = ""TC_TLedgerChildTMultiAddressList"";
    const string TLedgerChildTMultiAddressCollectionName = ""TMultiAddress"";
    const string TLedgerChildTMultiAddressCollectionNamePaginated = ""TMultiAddress"";
    internal static global::TallyConnector.Core.Models.Request.Part GetTLedgerChildTMultiAddressMainTDLPart(string partName = TLedgerChildTMultiAddressReportName, string? collectionName = TLedgerChildTMultiAddressCollectionName, string? xmlTag = null)
    {
        return new(partName, collectionName, partName)
        {
            XMLTag = xmlTag
        };
    }

    internal static global::TallyConnector.Core.Models.Request.Part[] GetTLedgerChildTMultiAddressTDLParts(string partName = TLedgerChildTMultiAddressReportName, string? collectionName = TLedgerChildTMultiAddressCollectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Request.Part[1];
        parts[0] = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTMultiAddressMainTDLPart(partName, collectionName);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Request.Line[] GetTLedgerChildTMultiAddressTDLLines(string xmlTag = ""TMULTIADDRESS"")
    {
        var lines = new global::TallyConnector.Core.Models.Request.Line[1];
        lines[0] = new(TLedgerChildTMultiAddressReportName, [TLedgerChildTMultiAddressGSTDealerTypeTDLFieldName], xmlTag);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Request.Field[] GetTLedgerChildTMultiAddressTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Request.Field[1];
        fields[0] = new(TLedgerChildTMultiAddressGSTDealerTypeTDLFieldName, ""GSTREGISTRATIONTYPE"", ""$$TC_GetTGSTRegistrationType:$GSTREGISTRATIONTYPE"")
        {
            Invisible = ""$$ISEmpty:$$value""
        };
        return fields;
    }

    internal static string[] GetTLedgerChildTMultiAddressFetchList(string prefix)
    {
        return[$""{prefix}.GSTREGISTRATIONTYPE""];
    }
}"),
                ("TallyConnector.Core.Models.Temp.TGSTRegistrationType.TallyPrimeService_TST.TDLReport.g.cs",
                @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Services.Temp;
/*
 * Generated based on TallyConnector.Core.Models.Temp.TGSTRegistrationType
 */
partial class TallyPrimeService
{
    internal static global::TallyConnector.Core.Models.Request.NameSet[] GetTLedgerChildTMultiAddressChildTGSTRegistrationTypeTDLNameSets()
    {
        var nameSets = new global::TallyConnector.Core.Models.Request.NameSet[1];
        nameSets[0] = new(""TC_TGSTRegistrationTypeEnum"")
        {
            List = [""Unknown:\""Unknown\"""", ""Composition:\""Composition\"""", ""Consumer:\""Consumer\"""", ""Unregistered/Consumer:\""Consumer\"""", ""Regular:\""Regular\""""]
        };
        return nameSets;
    }

    internal static global::TallyConnector.Core.Models.Request.TDLFunction[] GetTLedgerChildTMultiAddressChildTGSTRegistrationTypeTDLFunctions()
    {
        var functions = new global::TallyConnector.Core.Models.Request.TDLFunction[1];
        functions[0] = new(""TC_GetTGSTRegistrationType"")
        {
            Parameters = [""val : String : \""\""""],
            Actions = [""001 :Return : $$NameGetValue:##Val:TC_TGSTRegistrationTypeEnum""]
        };
        return functions;
    }

    internal static string? GetTallyString(global::TallyConnector.Core.Models.Temp.TGSTRegistrationType? obj, global::TallyConnector.Core.Models.Common.LicenseInfo LicenseInfo)
    {
        var version = LicenseInfo.TallyShortVersion.ToString();
        return obj switch
        {
            null => null,
            global::TallyConnector.Core.Models.Temp.TGSTRegistrationType.None => string.Empty,
            global::TallyConnector.Core.Models.Temp.TGSTRegistrationType.Unknown => ""Unknown"",
            global::TallyConnector.Core.Models.Temp.TGSTRegistrationType.Composition => ""Composition"",
            global::TallyConnector.Core.Models.Temp.TGSTRegistrationType.Consumer => version switch
            {
                ""6.6.3"" or ""1.1.1"" or ""1.1.2"" or ""1.1.3"" or ""1.1.4"" or ""2.0"" or ""2.0.1"" or ""2.1"" => ""Consumer"",
                _ => ""Unregistered/Consumer""
            },
            global::TallyConnector.Core.Models.Temp.TGSTRegistrationType.Regular => ""Regular"",
            _ => string.Empty
        };
    }
}"),
                ("TallyConnector.Core.Models.Temp.TLedgerGSTRegistrationDetails.TallyPrimeService_TST.TDLReport.g.cs",
                @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Services.Temp;
/*
 * Generated based on TallyConnector.Core.Models.Temp.TLedgerGSTRegistrationDetails
 */
partial class TallyPrimeService
{
    internal const string TLedgerChildTLedgerGSTRegistrationDetailsGSTRegistrationTypeTDLFieldName = ""TC_TLedgerGSTRegistrationDetails_GSTRegistrationType"";
    internal const string TLedgerChildTLedgerGSTRegistrationDetailsReportName = ""TC_TLedgerChildTLedgerGSTRegistrationDetailsList"";
    const string TLedgerChildTLedgerGSTRegistrationDetailsCollectionName = ""TLedgerGSTRegistrationDetails"";
    const string TLedgerChildTLedgerGSTRegistrationDetailsCollectionNamePaginated = ""TLedgerGSTRegistrationDetails"";
    internal static global::TallyConnector.Core.Models.Request.Part GetTLedgerChildTLedgerGSTRegistrationDetailsMainTDLPart(string partName = TLedgerChildTLedgerGSTRegistrationDetailsReportName, string? collectionName = TLedgerChildTLedgerGSTRegistrationDetailsCollectionName, string? xmlTag = null)
    {
        return new(partName, collectionName, partName)
        {
            XMLTag = xmlTag
        };
    }

    internal static global::TallyConnector.Core.Models.Request.Part[] GetTLedgerChildTLedgerGSTRegistrationDetailsTDLParts(string partName = TLedgerChildTLedgerGSTRegistrationDetailsReportName, string? collectionName = TLedgerChildTLedgerGSTRegistrationDetailsCollectionName, string? xmlTag = null)
    {
        var parts = new global::TallyConnector.Core.Models.Request.Part[1];
        parts[0] = global::TallyConnector.Services.Temp.TallyPrimeService.GetTLedgerChildTLedgerGSTRegistrationDetailsMainTDLPart(partName, collectionName);
        return parts;
    }

    internal static global::TallyConnector.Core.Models.Request.Line[] GetTLedgerChildTLedgerGSTRegistrationDetailsTDLLines(string xmlTag = ""TLEDGERGSTREGISTRATIONDETAILS"")
    {
        var lines = new global::TallyConnector.Core.Models.Request.Line[1];
        lines[0] = new(TLedgerChildTLedgerGSTRegistrationDetailsReportName, [TLedgerChildTLedgerGSTRegistrationDetailsGSTRegistrationTypeTDLFieldName], xmlTag);
        return lines;
    }

    internal static global::TallyConnector.Core.Models.Request.Field[] GetTLedgerChildTLedgerGSTRegistrationDetailsTDLFields()
    {
        var fields = new global::TallyConnector.Core.Models.Request.Field[1];
        fields[0] = new(TLedgerChildTLedgerGSTRegistrationDetailsGSTRegistrationTypeTDLFieldName, ""GSTREGISTRATIONTYPE"", ""$$TC_GetTGSTRegistrationType:$GSTREGISTRATIONTYPE"")
        {
            Invisible = ""$$ISEmpty:$$value""
        };
        return fields;
    }

    internal static string[] GetTLedgerChildTLedgerGSTRegistrationDetailsFetchList(string prefix)
    {
        return[$""{prefix}.GSTREGISTRATIONTYPE""];
    }
}"),
                ("TallyConnector.Services.Temp.TallyPrimeService.ReportResponseEnvelope.g.cs",
                @"using TallyConnector.Core.Extensions;

#nullable enable
namespace TallyConnector.Services.Temp.Models;
[global::System.Xml.Serialization.XmlRootAttribute(""ENVELOPE"")]
public class TallyPrimeServiceReportResponseEnvelopeForTLedger
{
    [System.Xml.Serialization.XmlElementAttribute(ElementName = ""LEDGER"")]
    public global::System.Collections.Generic.List<global::TallyConnector.Core.Models.Temp.TLedger> Objects { get; set; } = [];

    [System.Xml.Serialization.XmlElementAttribute(ElementName = ""TC_TOTALCOUNT"")]
    public int? TotalCount { get; set; }
}"),
        ]);
    }


}
