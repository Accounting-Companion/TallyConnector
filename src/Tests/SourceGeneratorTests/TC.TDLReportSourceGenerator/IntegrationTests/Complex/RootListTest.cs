using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using TallyConnector.Core.Attributes;
using TallyConnector.Services;

namespace IntegrationTests.Complex
{
    [TestClass]
    public class RootListTest
    {
        [TestMethod]
        public void TestGetParts()
        {
            var parts = Class.TallyService.GetGroupTDLParts();
            Assert.AreEqual(2, parts.Length);
            var listPart = parts[1];
            Assert.AreEqual(Class.TallyService.GroupAdressesReportName, listPart.Name);
        }
        [TestMethod]
        public void TestGetLines()
        {
            var lines = Class.TallyService.GetGroupTDLLines();
            Assert.AreEqual(2, lines.Length);
            var line1 = lines[0];
            Assert.AreEqual(1, line1.Explode.Count);
            CollectionAssert.AreEqual(new List<string> { $"{Class.TallyService.GroupAdressesReportName}:{"$$NumItems:ADDRESS<1"}" }, line1.Explode);
            Assert.AreEqual(2, line1.Fields!.Count);
            var line2 = lines[1];
            CollectionAssert.AreEqual(new List<string> { Class.TallyService.GroupAdressesTDLFieldName }, line2.Fields!);
        }

    }
}
namespace IntegrationTests.Complex.Class
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

        [XmlArray(ElementName = "ADDRESS.LIST")]
        [XmlArrayItem(ElementName = "ADDRESS")]
        [TDLCollection(CollectionName = "ADDRESS", ExplodeCondition = "$$NumItems:ADDRESS<1")]
        public List<string> Adresses { get; set; } = [];
    }

}
