namespace UnitTests.RequestableObjectTests;
[TestClass]
public class NestedPropertyTests
{
    [TestMethod]
    public async Task TestNestedPoperties()
    {
        var src = @"

using TallyConnector.Core.Attributes.SourceGenerator;
using TallyConnector.Core.Attributes;
using TallyConnector.Abstractions.Attributes;
using System.Collections.Generic;
namespace UnitTests.TestBasic;

[GenerateMeta]
public partial class LanguageNameList
{
    public LanguageNameList()
    {
        Names = [];
    }

    [XmlArray(ElementName = ""NAME.LIST"")]
    [XmlArrayItem(ElementName = ""NAME"")]
    [TDLCollection(CollectionName = ""Name"")]
    public List<string> Names { get; set; }

    [XmlElement(ElementName = ""LANGUAGEID"")]
    public int LanguageId { get; set; }
}
public partial class BaseAliasedMasterObject

{
    [TDLField(Set = ""$_FirstAlias"")]
    public string? Alias { get; set; }

    [XmlElement(ElementName = ""LANGUAGENAME.LIST"")]
    [TDLCollection(CollectionName = ""LanguageName"")]
    public List<LanguageNameList> LanguageNameList { get; set; } = [];
}
    [GenerateMeta]
[GenerateITallyRequestableObect]
public partial class RootObject : BaseAliasedMasterObject
{
    public List<Level1> Level1 { get; set; } = [];
}
public partial class Level1
{
    public string Level1Prop1 { get; set; }
    public string Level1Prop2 { get; set; }
    public List<Level2> Level2 { get; set; } = [];
}
public partial class Level2
{
    public string Level2Prop1 { get; set; }
    public string Level2Prop2 { get; set; }
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
