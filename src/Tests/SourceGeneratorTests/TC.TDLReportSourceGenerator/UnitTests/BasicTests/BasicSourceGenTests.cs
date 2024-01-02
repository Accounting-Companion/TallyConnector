using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.BasicTests;
[TestClass]
public class BasicSourceGenTests
{
    [TestMethod]
    public async Task TestBasicModel()
    {
        var src = @"
namespace TestNameSpace;
public partial class Group : TallyConnector.Core.Models.ITallyBaseObject
{
    [XmlElement(ElementName = ""PARENT"")]
    public string? Parent { get; set; }
    public virtual void RemoveNullChilds()
    {

    }
}";
        var resp = "";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src,("",resp));
    }
}
