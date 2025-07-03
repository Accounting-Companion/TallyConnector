namespace UnitTests;
[TestClass]
public class ModelMetaGeneratorTests
{
    [TestMethod]
    public async Task VerifyBasicClassMetaWithNoInheritance()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using TallyConnector.Abstractions.Attributes;
namespace UnitTests.TestBasic;

[TallyConnector.Abstractions.Attributes.GenerateMeta]
[TDLCollection(Type = ""Ledger"")]
public partial class Ledger
{
    public string Name { get; set; }
    public string Parent { get; set; }
}
";
        await VerifyModelMetaGenerator.VerifyGeneratorAsync(src,
            ("Ledger_UnitTests.TestBasic.g.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic.Meta;
/*
* Generated based on UnitTests.TestBasic.Ledger
*/
public class LedgerMeta : global::TallyConnector.Abstractions.Models.MetaObject
{
    LedgerMeta Instance => new();

    private LedgerMeta(string pathPrefix = """") : base(pathPrefix)
    {
    }

    global::TallyConnector.Abstractions.Models.PropertyMetaData Name => new(""NAME_SQUH"", ""NAME"");

    global::TallyConnector.Abstractions.Models.PropertyMetaData Parent => new(""PARENT_MUIG"", ""PARENT"");
}"));
    }
    [TestMethod]
    public async Task VerifyBasicClassMetaWithInheritance()
    {
        var src = @"
using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using TallyConnector.Abstractions.Attributes;
namespace UnitTests.TestBasic;

[TDLCollection(Type = ""Ledger"")]
[GenerateMeta]
public partial class BasicMetaModelBase
{
    public string BName { get; set; }
    public string BParent { get; set; }

    public OldClass Base { get; set; }
}
[GenerateMeta]
[TDLCollection(Type = ""Ledger"")]
partial class BasicMetaModelInheritanceInheritanceandOveridden : BasicMetaModelBase
{

    [XmlElement(ElementName = ""OVERIDDENNAME"")]
    public new string Name { get; set; }

    [XmlElement(ElementName = ""ISBILLWISEON"")]
    public bool IsBillWise { get; set; }

    public new NewClass Base { get; set; }

}
public partial class OldClass
{
    public string OldName { get; set; }
    public string OldParent { get; set; }
}
public partial class NewClass
{
    public string NewName { get; set; }
    public string NewParent { get; set; }
}
";
        await VerifyModelMetaGenerator.VerifyGeneratorAsync(src,
            ("Ledger_UnitTests.TestBasic.g.cs", @"using TallyConnector.Core.Extensions;

#nullable enable
namespace UnitTests.TestBasic.Meta;
/*
* Generated based on UnitTests.TestBasic.Ledger
*/
public class LedgerMeta : global::TallyConnector.Abstractions.Models.MetaObject
{
    LedgerMeta Instance => new();

    private LedgerMeta(string pathPrefix = """") : base(pathPrefix)
    {
    }

    global::TallyConnector.Abstractions.Models.PropertyMetaData Name => new(""NAME_SQUH"", ""NAME"");

    global::TallyConnector.Abstractions.Models.PropertyMetaData Parent => new(""PARENT_MUIG"", ""PARENT"");
}"));
    }
}
