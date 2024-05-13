using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using TallyConnector.Services;

namespace IntegrationTests.Basic
{
    [TestClass]
    public class BasicTests
    {
        const string _rootTag = "GROUP";
        [TestMethod]
        public void TestConstStrings()
        {
            Assert.AreEqual($"TC_{nameof(TestBasicNS.Group)}_{nameof(TestBasicNS.Group.Name)}", TestBasicNS.TallyService.GroupNameTDLFieldName);
            Assert.AreEqual($"TC_{nameof(TestBasicNS.Group)}_{nameof(TestBasicNS.Group.Parent)}", TestBasicNS.TallyService.GroupParentTDLFieldName);


            Assert.AreEqual($"TC_{nameof(TestBasicNS.Group)}List", TestBasicNS.TallyService.GroupReportName);
        }
        [TestMethod]
        public void TestGetParts()
        {
            var parts = TestBasicNS.TallyService.GetGroupTDLParts();

            Assert.AreEqual(1, parts.Length);
            var part1 = parts[0];

            Assert.AreEqual(TestBasicNS.TallyService.GroupReportName, part1.Name);
            Assert.AreEqual($"{TestBasicNS.TallyService.GroupReportName} : TC_GroupCollection", part1.Repeat);

        }
        [TestMethod]
        public void TestGetLines()
        {
            var lines = TestBasicNS.TallyService.GetGroupTDLLines();

            Assert.AreEqual(1, lines.Length);
            var line1 = lines[0];

            Assert.AreEqual(TestBasicNS.TallyService.GroupReportName, line1.Name);
            Assert.AreEqual(_rootTag, line1.XMLTag);

            Assert.AreEqual(2, line1.Fields!.Count);

            Assert.AreEqual(TestBasicNS.TallyService.GroupParentTDLFieldName, line1.Fields![0]);
            Assert.AreEqual(TestBasicNS.TallyService.GroupNameTDLFieldName, line1.Fields![1]);

        }
        [TestMethod]
        public void TestGetFields()
        {
            var fields = TestBasicNS.TallyService.GetGroupTDLFields();
            Assert.AreEqual(2, fields.Length);
            var field1 = fields[0];
            Assert.AreEqual(TestBasicNS.TallyService.GroupParentTDLFieldName, field1.Name);
            Assert.AreEqual("PARENT", field1.XMLTag);
            Assert.AreEqual("$PARENT", field1.Set);

            var field2 = fields[1];
            Assert.AreEqual(TestBasicNS.TallyService.GroupNameTDLFieldName, field2.Name);
            Assert.AreEqual("NAME", field2.XMLTag);
            Assert.AreEqual("$NAME", field2.Set);
        }

        [TestMethod]
        public void TestGetCollections()
        {
            var collections = TestBasicNS.TallyService.GetGroupTDLCollections();
            Assert.AreEqual(1, collections.Length);

            var collection1 = collections[0];
            Assert.AreEqual("TC_GroupCollection", collection1.Name);

            Assert.AreEqual(_rootTag, collection1.Type);
            CollectionAssert.AreEqual(new List<string>() { "PARENT, NAME" }, collection1.NativeFields);
        }

        [TestMethod]
        public void TestGetFetchList()
        {
            var fetchlist = TestBasicNS.TallyService.GetGroupFetchList();
            CollectionAssert.AreEqual(new List<string>() { "PARENT, NAME" }, fetchlist);
        }

        [TestMethod]
        public void TestReqXML()
        {
            var reqXml = TestBasicNS.TallyService.GetGroupRequestEnevelope().GetXML();
            Assert.AreEqual("""<ENVELOPE Action="None"><HEADER><VERSION>1</VERSION><TALLYREQUEST>EXPORT</TALLYREQUEST><TYPE>DATA</TYPE><ID>TC_GroupList</ID></HEADER><BODY><DESC><STATICVARIABLES><SVEXPORTFORMAT>$$SysName:XML</SVEXPORTFORMAT></STATICVARIABLES><TDL><TDLMESSAGE><REPORT NAME="TC_GroupList" ISMODIFY="No" ISFIXED="No" ISINITIALIZE="No" ISOPTION="No" ISINTERNAL="No"><FORMS>TC_GroupList</FORMS></REPORT><FORM NAME="TC_GroupList" ISMODIFY="No" ISFIXED="No" ISINITIALIZE="No" ISOPTION="No" ISINTERNAL="No"><TOPPARTS>TC_GroupList</TOPPARTS></FORM><PART NAME="TC_GroupList" ISMODIFY="No" ISFIXED="No" ISINITIALIZE="No" ISOPTION="No" ISINTERNAL="No"><TOPLINES>TC_GroupList</TOPLINES><REPEAT>TC_GroupList : TC_GroupCollection</REPEAT><SCROLLED>Vertical</SCROLLED></PART><LINE NAME="TC_GroupList" ISMODIFY="No" ISFIXED="No" ISINITIALIZE="No" ISOPTION="No" ISINTERNAL="No"><FIELDS>TC_Group_Parent</FIELDS><FIELDS>TC_Group_Name</FIELDS><XMLTAG>GROUP</XMLTAG></LINE><FIELD NAME="TC_Group_Parent" ISMODIFY="No" ISFIXED="No" ISINITIALIZE="No" ISOPTION="No" ISINTERNAL="No"><SET>$PARENT</SET><XMLTAG>PARENT</XMLTAG></FIELD><FIELD NAME="TC_Group_Name" ISMODIFY="No" ISFIXED="No" ISINITIALIZE="No" ISOPTION="No" ISINTERNAL="No"><SET>$NAME</SET><XMLTAG>NAME</XMLTAG></FIELD><COLLECTION NAME="TC_GroupCollection" ISMODIFY="No" ISFIXED="No" ISINITIALIZE="No" ISOPTION="No" ISINTERNAL="No"><TYPE>GROUP</TYPE><NATIVEMETHOD>PARENT, NAME</NATIVEMETHOD></COLLECTION><FUNCTION NAME="TC_GetBooleanFromLogicField" ISMODIFY="No" ISFIXED="No" ISINITIALIZE="No" ISOPTION="No" ISINTERNAL="No"><Parameter>val : Logical : None</Parameter><Returns>String</Returns><Action>000 :   If  : $$ISEmpty:##val</Action><Action>001 :Return : ##val</Action><Action>002 : Else    :</Action><Action>003 : If  :  ##val </Action><Action>004 :Return :"true"</Action><Action>005 : Else    :</Action><Action>006 :Return : "false"</Action><Action>007 : End If</Action><Action>008 : End If</Action></FUNCTION><FUNCTION NAME="TC_TransformDateToXSD" ISMODIFY="No" ISFIXED="No" ISINITIALIZE="No" ISOPTION="No" ISINTERNAL="No"><Parameter>ParamInputDate   : Date</Parameter><VARIABLES>ParamSeparator        : String : "-"</VARIABLES><VARIABLES>TempVarYear           : String</VARIABLES><VARIABLES>TempVarMonth          : String</VARIABLES><VARIABLES>TempVarDate           : String</VARIABLES><Returns>String</Returns><Action>01  : If        : NOT $$IsEmpty:##ParamInputDate</Action><Action>02  :   Set     : TempVarYear       : $$Zerofill:($$YearofDate:##ParamInputDate):4</Action><Action>03  :   Set     : TempVarMonth      : $$Zerofill:($$MonthofDate:##ParamInputDate):2</Action><Action>04  :   Set     : TempVarDate       : $$Zerofill:($$DayofDate:##ParamInputDate):2</Action><Action>05  :   Return  : $$String:##TempVarYear + $$String:##ParamSeparator + $$String:##TempVarMonth + $$String:##ParamSeparator + $$String:##TempVarDate</Action><Action>06  : End If</Action><Action>07  : Return    : ""</Action></FUNCTION></TDLMESSAGE></TDL></DESC><DATA /></BODY></ENVELOPE>""", reqXml);
        }
    }
}

namespace TestBasicNS
{
    [TallyConnector.Core.Attributes.GenerateHelperMethod<Group>(GenerationMode = TallyConnector.Core.Models.GenerationMode.GetMultiple)]
    public partial class TallyService : BaseTallyService
    {
    }
    [XmlRoot(ElementName = "GROUP")]
    public partial class Group : TallyConnector.Core.Models.IBaseObject
    {
        [XmlElement(ElementName = "PARENT")]
        public string? Parent { get; set; }

        [XmlElement(ElementName = "NAME")]
        public string? Name { get; set; }

    }
}
