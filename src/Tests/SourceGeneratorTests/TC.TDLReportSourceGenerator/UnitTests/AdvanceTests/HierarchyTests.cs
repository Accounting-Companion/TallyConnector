namespace UnitTests.AdvanceTests;
[TestClass]
public class HierarchyTests
{
    [TestMethod]
    public async Task TestOverridenSimpleProperty()
    {
        var src = @"
using TallyConnector.Core.Attributes;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.Masters;
using TallyConnector.Services;

namespace Test.NameSpace;
public class TestBaseClass
{
    public string Prop1toBeOveridden { get; set; }
    public string Prop2 { get; set; }
}
public class TestClass : TestBaseClass, IBaseObject
{
    public new int Prop1toBeOveridden {  get; set; }
}
[GenerateHelperMethod<TestClass>]
public class CustomTallyService : BaseTallyService
{

}";
        var resp = @"";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src, [("", resp)]);
    }
}

