using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace IntegrationTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
    }
}

public partial class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = "PARENT")]
    public string? Parent { get; set; }
    public virtual void RemoveNullChilds()
    {

    }
}