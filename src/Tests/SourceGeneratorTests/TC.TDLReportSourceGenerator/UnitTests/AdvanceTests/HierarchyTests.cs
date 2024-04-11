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
[Test(c=""NameGetValue:##Val:TC_AdAllocTypeEnum"")]
    public string Prop2 { get; set; } = ""##fdghjk"";
}
public class TestClass : TestBaseClass, IBaseObject
{
    public new int Prop1toBeOveridden {  get; set; }
}
[GenerateHelperMethod<TestClass>]
public class CustomTallyService : BaseTallyService
{

}";
        var resp = @"public class TestBaseClass
{
    public string Prop1toBeOveridden { get; set; }
[Test(c=""##cfghnj"")]
    public string Prop2 { get; set; } = ""##fdghjk"";

 internal static global::TallyConnector.Core.Models.TDLFunction[] GetAdAllocTypeTDLFunctions()
    {
        var functions = new global::TallyConnector.Core.Models.TDLFunction[1];
        functions[0] = new(""TC_GetAdAllocType"")
        {
            Parameters = [""val : String : \""\""""],
            Actions = [""001 :Return : NameGetValue:##Val:TC_AdAllocTypeEnum""]
        };
        return functions;
    }
}";
        await VerifyTDLReportSG.VerifyGeneratorAsync(src, [("", resp), ("", resp), ("", resp), ("", resp), ("", resp), ("", resp), ("", resp)]);
    }
}

