using IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IntegrationTests;
[TestClass]
public partial class HirechyTests
{
    [TestMethod]
    public void TestBaseClass()
    {
        var requestEnvelope = TallyServiceHierarchy.GetGroupHRequestEnevelope();
        string v = requestEnvelope.GetXML();
        var expected = "<ENVELOPE Action=\"\"><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>DATA</TYPE><ID>TC_GroupHList</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><REPORT ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupHList\"><FORMS>TC_GroupHList</FORMS></REPORT><FORM ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupHList\"><TOPPARTS>TC_GroupHList</TOPPARTS></FORM><PART ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupHList\"><TOPLINES>TC_GroupHList</TOPLINES><REPEAT>TC_GroupHList : TC_GroupHCollection</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupHList\"><USE>TC_BaseGroupList</USE><FIELDS>TC_GroupH_Name</FIELDS><XMLTAG>GROUPH</XMLTAG></LINE><LINE ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_BaseGroupList\"><FIELDS>TC_BaseGroup_Parent</FIELDS><XMLTAG>BASEGROUP</XMLTAG></LINE><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_BaseGroup_Parent\"><SET>$PARENT</SET><XMLTAG>PARENT</XMLTAG></FIELD><FIELD ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupH_Name\"><SET>$NAME</SET><XMLTAG>NAME</XMLTAG></FIELD><COLLECTION ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\" NAME=\"TC_GroupHCollection\"><TYPE>GROUPH</TYPE><NATIVEMETHOD>*</NATIVEMETHOD></COLLECTION></TDLMESSAGE></TDL></DESC></BODY></ENVELOPE>";
        Assert.AreEqual(expected, v);
    }
    [TestMethod]
    public void TestGroupClass()
    {
       var v = TallyServiceHierarchy.GetLedgerRequestEnevelope().GetXML();
    }
}
[TallyConnector.Core.Attributes.GenerateHelperMethod<GroupH>]
[TallyConnector.Core.Attributes.GenerateHelperMethod<Ledger>]
public partial class TallyServiceHierarchy : TallyServiceGroup
{
}
public class GroupH : BaseGroup, TallyConnector.Core.Models.ITallyBaseObject
{

    [XmlElement(ElementName = "NAME")]
    public string? Name { get; set; }

}

public class BaseGroup
{
    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }
}
