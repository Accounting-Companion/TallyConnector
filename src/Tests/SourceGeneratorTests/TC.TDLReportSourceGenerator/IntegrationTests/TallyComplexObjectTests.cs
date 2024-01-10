using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IntegrationTests;
[TestClass]
public class TallyComplexObjectTests
{
    [TestMethod]
    public void TestAmount()
    {
        var envelope = ComplexObjectTallyService.GetLedgerRequestEnevelope();
        var xml = envelope.GetXML();
        var expected = "<ENVELOPE Action=\"\"><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>DATA</TYPE><ID>TC_LedgerList</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><REPORT ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LedgerList\"><FORMS>TC_LedgerList</FORMS></REPORT><FORM ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LedgerList\"><TOPPARTS>TC_LedgerList</TOPPARTS></FORM><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LedgerList\"><TOPLINES>TC_LedgerList</TOPLINES><REPEAT>TC_LedgerList : TC_LedgerCollection</REPEAT><SCROLLED>Vertical</SCROLLED></PART><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LedgerClosingBalanceList\"><TOPLINES>TC_AmountList</TOPLINES><SCROLLED>Vertical</SCROLLED></PART><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LedgerList\"><FIELDS>TC_Ledger_Name</FIELDS><XMLTAG>LEDGER</XMLTAG><EXPLODE>TC_LedgerClosingBalanceList:Yes</EXPLODE></LINE><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_AmountList\"><FIELDS>TC_Amount_BaseAmount</FIELDS><FIELDS>TC_Amount_ForexAmount</FIELDS><FIELDS>TC_Amount_ExchangeRate</FIELDS><FIELDS>TC_Amount_IsDebit</FIELDS><XMLTAG>ClosingBalance</XMLTAG></LINE><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_Ledger_Name\"><SET>$NAME</SET><XMLTAG>NAME</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_Amount_BaseAmount\"><SET>$ClosingBalance</SET><XMLTAG>BASEAMOUNT</XMLTAG><TYPE>Amount : Base</TYPE><FORMAT>Symbol, No Zero</FORMAT></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_Amount_ForexAmount\"><SET>$ClosingBalance</SET><XMLTAG>FOREXAMOUNT</XMLTAG><TYPE>Amount : Forex</TYPE><FORMAT>Symbol, No Zero</FORMAT></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_Amount_ExchangeRate\"><SET>$ClosingBalance</SET><XMLTAG>EXCHANGERATE</XMLTAG><TYPE>Amount : Rate</TYPE><FORMAT>Symbol, No Zero</FORMAT></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_Amount_IsDebit\"><SET>$$TC_GetBooleanFromLogicField:$$IsDebit:$ClosingBalance</SET><XMLTAG>ISDEBIT</XMLTAG></FIELD><COLLECTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_LedgerCollection\"><TYPE>LEDGER</TYPE><NATIVEMETHOD>*</NATIVEMETHOD></COLLECTION><FUNCTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GetBooleanFromLogicField\"><Parameter>val : Logical : None</Parameter><Returns>String</Returns><Action>000 :   If  : $$ISEmpty:##val</Action><Action>001 :Return : ##val</Action><Action>002 : Else    :</Action><Action>003 : If  :  ##val </Action><Action>004 :Return :\"true\"</Action><Action>005 : Else    :</Action><Action>006 :Return : \"false\"</Action><Action>007 : End If</Action><Action>008 : End If</Action></FUNCTION></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
        Assert.AreEqual(expected, xml);
    }
}
[TallyConnector.Core.Attributes.GenerateHelperMethod<Ledger>]
public partial class ComplexObjectTallyService : TallyConnector.Services.BaseTallyService
{
}
file class Ledger : TallyConnector.Core.Models.ITallyBaseObject
{

    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "ClosingBalance")]
    public TallyConnector.Core.Models.TallyComplexObjects.Amount? ClosingBalance { get; set; }

}