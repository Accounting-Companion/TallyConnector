using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
using TallyConnector.Core.Models;

namespace IntegrationTests;

[TestClass]
public class BasicIntegrationTests
{
    [TestMethod]
    public void BasicFieldsTest()
    {
        var fields = TallyServiceGroup.GetGroupTDLFields();
        Assert.IsNotNull(fields);
        Assert.AreEqual(3, fields.Length);
    }
    [TestMethod]
    public void BasicEnvTest()
    {
        TallyConnector.Core.Models.RequestEnvelope requestEnvelope = TallyServiceGroup.GetGroupRequestEnevelope();
        //requestEnvelope.Body.Desc.TDL.TDLMessage.Report = [new()];
        //requestEnvelope.Body.Desc.TDL.TDLMessage.Field = [.. Group.GetTDLFields()];
        Assert.IsNotNull(requestEnvelope);
        
        Assert.AreEqual(TallyConnector.Core.Models.HType.Data, requestEnvelope.Header!.Type);
        string v = requestEnvelope.GetXML();
        string expected = "<ENVELOPE Action=\"\"><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>DATA</TYPE><ID>TC_GroupList</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><REPORT ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupList\"><FORMS>TC_GroupList</FORMS></REPORT><FORM ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupList\"><TOPPARTS>TC_GroupList</TOPPARTS></FORM><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupList\"><TOPLINES>TC_GroupList</TOPLINES><REPEAT>TC_GroupList : TC_GroupCollection</REPEAT><SCROLLED>Vertical</SCROLLED></PART><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupLanguageNameListList\"><TOPLINES>TC_LanguageNameLisList</TOPLINES><REPEAT>TC_LanguageNameLisList : LanguageName</REPEAT><SCROLLED>Vertical</SCROLLED></PART><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LanguageNameLisNameListList\"><TOPLINES>TC_NamesLList</TOPLINES><SCROLLED>Vertical</SCROLLED></PART><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_NamesLNAMESList\"><TOPLINES>TC_NamesLNAMESList</TOPLINES><REPEAT>TC_NamesLNAMESList : Name</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupList\"><FIELDS>TC_Group_Parent</FIELDS><XMLTAG>GROUP</XMLTAG><EXPLODE>TC_GroupLanguageNameListList:Yes</EXPLODE></LINE><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LanguageNameLisList\"><FIELDS>TC_LanguageNameLis_LanguageAlias</FIELDS><XMLTAG>LANGUAGENAME.LIST</XMLTAG><EXPLODE>TC_LanguageNameLisNameListList:Yes</EXPLODE></LINE><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_NamesLList\"><FIELDS>SimpleField</FIELDS><XMLTAG>NAME.LIST</XMLTAG><EXPLODE>TC_NamesLNAMESList:Yes</EXPLODE></LINE><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_NamesLNAMESList\"><FIELDS>TC_NamesL_NAMES</FIELDS></LINE><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_Group_Parent\"><SET>$PARENT</SET><XMLTAG>PARENT</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LanguageNameLis_LanguageAlias\"><SET>$LANGUAGEALIAS</SET><XMLTAG>LANGUAGEALIAS</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_NamesL_NAMES\"><SET>$NAME</SET><XMLTAG>NAME</XMLTAG></FIELD><COLLECTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupCollection\"><TYPE>GROUP</TYPE><NATIVEMETHOD>*</NATIVEMETHOD><NATIVEMETHOD>LanguageName</NATIVEMETHOD></COLLECTION></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
        Assert.AreEqual(expected, v);
    }
    [TestMethod]
    public void ComplexModelTest()
    {
        TallyConnector.Core.Models.RequestEnvelope requestEnvelope = TallyServiceVoucher.GetVoucherRequestEnevelope();
        var c = TallyServiceVoucher.VoucherReportName;
        Assert.IsNotNull(requestEnvelope);
        Assert.AreEqual(TallyConnector.Core.Models.HType.Data, requestEnvelope.Header!.Type);
        string v = requestEnvelope.GetXML();
        string expected = "<ENVELOPE Action=\"\"><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>DATA</TYPE><ID>TC_VoucherList</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><REPORT ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_VoucherList\"><FORMS>TC_VoucherList</FORMS></REPORT><FORM ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_VoucherList\"><TOPPARTS>TC_VoucherList</TOPPARTS></FORM><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_VoucherList\"><TOPLINES>TC_VoucherList</TOPLINES><REPEAT>TC_VoucherList : TC_VoucherCollection</REPEAT><SCROLLED>Vertical</SCROLLED></PART><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_VoucherLedgerEntriesList\"><TOPLINES>TC_LedgerEntryList</TOPLINES><REPEAT>TC_LedgerEntryList : LedgerEntries</REPEAT><SCROLLED>Vertical</SCROLLED></PART><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_VoucherInventoryEntriesList\"><TOPLINES>TC_InventoryEntryList</TOPLINES><REPEAT>TC_InventoryEntryList : ALLINVENTORYENTRIES</REPEAT><SCROLLED>Vertical</SCROLLED></PART><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_InventoryEntryLedgerEntriesList\"><TOPLINES>TC_LedgerEntryList</TOPLINES><REPEAT>TC_LedgerEntryList : ACCOUNTINGALLOCATIONS</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_VoucherList\"><FIELDS>TC_Voucher_VoucherNumber</FIELDS><FIELDS>TC_Voucher_PersistedView</FIELDS><XMLTAG>VOUCHER</XMLTAG><EXPLODE>TC_VoucherLedgerEntriesList:Yes</EXPLODE><EXPLODE>TC_VoucherInventoryEntriesList:Yes</EXPLODE></LINE><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LedgerEntryList\"><FIELDS>TC_LedgerEntry_LedgerName</FIELDS><XMLTAG>LEDGERENTRY</XMLTAG></LINE><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_InventoryEntryList\"><FIELDS>TC_InventoryEntry_StockItemName</FIELDS><XMLTAG>INVENTORYENTRY</XMLTAG><EXPLODE>TC_InventoryEntryLedgerEntriesList:Yes</EXPLODE></LINE><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_Voucher_VoucherNumber\"><SET>$VOUCHERNUMBER</SET><XMLTAG>VOUCHERNUMBER</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_Voucher_PersistedView\"><SET>$PERSISTEDVIEW</SET><XMLTAG>PERSISTEDVIEW</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LedgerEntry_LedgerName\"><SET>$LEDGERNAME</SET><XMLTAG>LEDGERNAME</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_InventoryEntry_StockItemName\"><SET>$STOCKITEMNAME</SET><XMLTAG>STOCKITEMNAME</XMLTAG></FIELD><COLLECTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_VoucherCollection\"><TYPE>VOUCHER</TYPE><NATIVEMETHOD>*</NATIVEMETHOD><NATIVEMETHOD>LedgerEntries</NATIVEMETHOD><NATIVEMETHOD>ALLINVENTORYENTRIES</NATIVEMETHOD></COLLECTION></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
        Assert.AreEqual(expected, v);
    }
    [TestMethod]
    public void TestSimpleList()
    {
        var requestEnvelope = TallyService.GetLGroupRequestEnevelope();
        var xml = requestEnvelope.GetXML();
        string expected = "<ENVELOPE Action=\"\"><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>DATA</TYPE><ID>TC_LGroupList</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><REPORT ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupList\"><FORMS>TC_LGroupList</FORMS></REPORT><FORM ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupList\"><TOPPARTS>TC_LGroupList</TOPPARTS></FORM><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupList\"><TOPLINES>TC_LGroupList</TOPLINES><REPEAT>TC_LGroupList : TC_LGroupCollection</REPEAT><SCROLLED>Vertical</SCROLLED></PART><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupAddresesList\"><TOPLINES>TC_LGroupAddresesList</TOPLINES><REPEAT>TC_LGroupAddresesList : Address</REPEAT><SCROLLED>Vertical</SCROLLED><XMLTAG>ADDRESS.LIST</XMLTAG></PART><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupList\"><FIELDS>TC_LGroup_Parent</FIELDS><FIELDS>TC_LGroup_Name</FIELDS><XMLTAG>LGROUP</XMLTAG><EXPLODE>TC_LGroupAddresesList:Yes</EXPLODE></LINE><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupAddresesList\"><FIELDS>TC_LGroup_Addreses</FIELDS></LINE><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroup_Parent\"><SET>$PARENT</SET><XMLTAG>PARENT</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroup_Name\"><SET>$NAME</SET><XMLTAG>NAME</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroup_Addreses\"><SET>$Address</SET><XMLTAG>Address</XMLTAG></FIELD><COLLECTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupCollection\"><TYPE>LGROUP</TYPE><NATIVEMETHOD>*</NATIVEMETHOD><NATIVEMETHOD>Address</NATIVEMETHOD></COLLECTION></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
        Assert.AreEqual(expected, xml);
    }
    [TestMethod]
    public void TestComplexList()
    {
        var requestEnvelope = TallyService.GetLedgerRequestEnevelope();
        var xml = requestEnvelope.GetXML();
        string expected = "<ENVELOPE Action=\"\"><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>DATA</TYPE><ID>TC_LGroupList</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><REPORT ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupList\"><FORMS>TC_LGroupList</FORMS></REPORT><FORM ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupList\"><TOPPARTS>TC_LGroupList</TOPPARTS></FORM><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupList\"><TOPLINES>TC_LGroupList</TOPLINES><REPEAT>TC_LGroupList : TC_LGroupCollection</REPEAT><SCROLLED>Vertical</SCROLLED></PART><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupAddresesList\"><TOPLINES>TC_LGroupAddresesList</TOPLINES><REPEAT>TC_LGroupAddresesList : Address</REPEAT><SCROLLED>Vertical</SCROLLED><XMLTAG>ADDRESS.LIST</XMLTAG></PART><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupList\"><FIELDS>TC_LGroup_Parent</FIELDS><FIELDS>TC_LGroup_Name</FIELDS><XMLTAG>LGROUP</XMLTAG><EXPLODE>TC_LGroupAddresesList:Yes</EXPLODE></LINE><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupAddresesList\"><FIELDS>TC_LGroup_Addreses</FIELDS></LINE><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroup_Parent\"><SET>$PARENT</SET><XMLTAG>PARENT</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroup_Name\"><SET>$NAME</SET><XMLTAG>NAME</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroup_Addreses\"><SET>$Address</SET><XMLTAG>Address</XMLTAG></FIELD><COLLECTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LGroupCollection\"><TYPE>LGROUP</TYPE><NATIVEMETHOD>*</NATIVEMETHOD><NATIVEMETHOD>Address</NATIVEMETHOD></COLLECTION></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
        Assert.AreEqual(expected, xml);
    }
}
[GenerateHelperMethod<LGroup>]
[GenerateHelperMethod<Ledger>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public class Ledger : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }

    //[XmlElement(ElementName = "LEDGERCLOSINGVALUES.LIST")]
    //[XmlArray(ElementName = "LEDGERCLOSINGVALUES.LIST")]
    [XmlArrayItem(ElementName = "LEDGERCLOSINGVALUES.LIST")]
    [TDLCollection(CollectionName = "LEDGERCLOSINGVALUES")]
    public List<IntegrationTests.Models.ClosingBalances>? ClosingBalances { get; set; }
}
public  class LGroup : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }

    [XmlArray(ElementName = "ADDRESS.LIST")]
    [XmlArrayItem(ElementName = "Address")]
    [TDLCollection(CollectionName ="Address")]
    public List<string>? Addreses { get; set; }
}
[GenerateHelperMethod<Group,Group>]
public partial class TallyServiceGroup : TallyConnector.Services.BaseTallyService
{
}
public  class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }

    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
    public List<LanguageNameLis>? LanguageNameList { get; set; }
}
[XmlRoot(ElementName = "LANGUAGENAME.LIST")]
[TDLCollection(CollectionName = "LanguageName")]
public partial class LanguageNameLis
{
    public LanguageNameLis()
    {
        NameList = new();

    }
    [XmlIgnore]
    public string? LanguageAlias
    {
        get { return NameList.NAMES?.Count > 1 ? string.Join("..\n", NameList.NAMES.GetRange(1, NameList.NAMES.Count - 1)) : null; }
        set
        {
            if (NameList.NAMES?.Count > 1)
            {
                NameList.NAMES.RemoveRange(1, NameList.NAMES.Count - 1);
                NameList.NAMES.InsertRange(1, value?.Split("..\n".ToCharArray()).ToList()!);
            }
            else if (NameList.NAMES?.Count == 1)
            {
                NameList.NAMES.InsertRange(1, value?.Split("..\n".ToCharArray()).ToList()!);
            }
        }
    }

    [XmlElement(ElementName = "NAME.LIST")]
    public NamesL NameList { get; set; }

    //[XmlElement(ElementName = "LANGUAGEID")]
    //public LANGUAGEID LANGUAGEID { get; set; }
}
[XmlRoot(ElementName = "NAME.LIST")]

public partial class NamesL
{
    public NamesL()
    {
        NAMES = new();
    }

    [XmlElement(ElementName = "NAME")]
    [TDLCollection(CollectionName = "Name")]
    public List<string>? NAMES { get; set; }
    //public List<int>? Tests { get; set; }

    //[XmlAttribute(AttributeName = "TYPE")]
    //public string TYPE { get; set; }

    //[XmlText]
    //public string Text { get; set; }
}
[GenerateHelperMethod<Voucher>]
public partial class TallyServiceVoucher : TallyConnector.Services.BaseTallyService
{
}

public partial class Voucher : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = "VOUCHERNUMBER")]
    public string? VoucherNumber { get; set; }
    [XmlElement(ElementName = "PERSISTEDVIEW")]
    public string? PersistedView { get; set; }

    [TDLCollection(CollectionName = "LedgerEntries")]
    public List<LedgerEntry> LedgerEntries { get; set; }

    [TDLCollection(CollectionName = "ALLINVENTORYENTRIES")]
    public List<InventoryEntry> InventoryEntries { get; set; }


}
public partial class LedgerEntry
{
    [XmlElement(ElementName = "LEDGERNAME")]
    public string? LedgerName { get; set; }
}
public partial class InventoryEntry
{
    [XmlElement(ElementName = "STOCKITEMNAME")]
    public string? StockItemName { get; set; }

    [TDLCollection(CollectionName = "ACCOUNTINGALLOCATIONS")]
    public List<LedgerEntry> LedgerEntries { get; set; }
}