using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.BasicTests;
[TestClass]
public class ComplexObjectListTests
{
    [TestMethod]
    public async Task TestClosingBalances()
    {
        var src = @"
#nullable enable
using System.Xml.Serialization;
using TallyConnector.Core.Models;
namespace TestNameSpace;
[TallyConnector.Core.Attributes.GenerateHelperMethod<Group>]
public partial class TallyService : TallyConnector.Services.BaseTallyService
{
}
public class Group :TallyConnector.Core.Models.ITallyBaseObject
{

    [XmlElement(ElementName = ""NAME"")]
    public string? Name { get; set; }

    [XmlElement(ElementName = ""LANGUAGENAME.LIST"")]
    [TDLCollection(CollectionName = ""LanguageName"")]
    public List<LanguageNameList> LanguageNameList { get; set; }

}
";

    }
}
