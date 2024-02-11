using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Interfaces.Masters;
using TallyConnector.Core.Models.Masters;

namespace IntegrationTests;

[TestClass]
public class BasicIntegrationTests
{
    [TestMethod]
    public async Task TestEnum()
    {
        var tallyServiceTestEnum = new TallyServiceTestEnum();
        List<GroupEnum> groupEnums = await tallyServiceTestEnum.GetGroupsAsync(new());
        var Ledgers = await tallyServiceTestEnum.GetLedgersAsync(new());
        var requestEnvelope = TallyServiceTestEnum.GetGroupRequestEnevelope();
        var xml = requestEnvelope.GetXML();
        string expected = "<ENVELOPE Action=\"\"><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>DATA</TYPE><ID>TC_GroupEnumList</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><REPORT ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupEnumList\"><FORMS>TC_GroupEnumList</FORMS></REPORT><FORM ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupEnumList\"><TOPPARTS>TC_GroupEnumList</TOPPARTS></FORM><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupEnumList\"><TOPLINES>TC_GroupEnumList</TOPLINES><REPEAT>TC_GroupEnumList : TC_GroupEnumCollection</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupEnumList\"><FIELDS>TC_GroupEnum_Name</FIELDS><FIELDS>TC_GroupEnum_GSTTaxType</FIELDS><XMLTAG>GROUPENUM</XMLTAG></LINE><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupEnum_Name\"><SET>$NAME</SET><XMLTAG>NAME</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupEnum_GSTTaxType\"><SET>$$TC_GetGSTTaxType:$GSTDUTYHEAD</SET><XMLTAG>GSTDUTYHEAD</XMLTAG></FIELD><COLLECTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupEnumCollection\"><TYPE>GROUPENUM</TYPE><NATIVEMETHOD>*</NATIVEMETHOD></COLLECTION><NAMESET ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GSTTaxTypeEnum\"><LIST>Central Tax:\"CentralTax\"</LIST><LIST>CGST:\"CGST\"</LIST><LIST>Cess:\"Cess\"</LIST><LIST>Integrated Tax:\"IntegratedTax\"</LIST><LIST>IGST:\"IGST\"</LIST><LIST>UT Tax:\"UTTax\"</LIST><LIST>State Tax:\"StateTax\"</LIST><LIST>SGST/UTGST:\"SGST\"</LIST></NAMESET><FUNCTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GetBooleanFromLogicField\"><Parameter>val : Logical : None</Parameter><Returns>String</Returns><Action>000 :   If  : $$ISEmpty:##val</Action><Action>001 :Return : ##val</Action><Action>002 : Else    :</Action><Action>003 : If  :  ##val </Action><Action>004 :Return :\"true\"</Action><Action>005 : Else    :</Action><Action>006 :Return : \"false\"</Action><Action>007 : End If</Action><Action>008 : End If</Action></FUNCTION><FUNCTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GetGSTTaxType\"><Parameter>val : String : \"\"</Parameter><Action>001 :Return : $$NameGetValue:##Val:TC_GSTTaxTypeEnum</Action></FUNCTION></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
        Assert.AreEqual(expected, xml);
    }
}
[TallyConnector.Core.Attributes.GenerateHelperMethod<GroupEnum>(MethodNameSuffix ="Group")]
[TallyConnector.Core.Attributes.GenerateHelperMethod<Ledger>]
public partial class TallyServiceTestEnum : TallyConnector.Services.BaseTallyService
{
}
[TDLCollection(Type = "GROUP")]
public  class GroupEnum : IBaseMasterObject
{
    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "GSTDUTYHEAD")]
    public GSTTaxType? GSTTaxType { get; set; }

}
public class Ledger : BaseLedger
{
}
