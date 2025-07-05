using System.Xml.Serialization;
using TallyConnector.Abstractions.Attributes;

namespace IntegrationTests.MetaTests.BasicTests;
public class ComplexNestedPropertyTests
{
    [Test]
    public void TestNestedLevels()
    {
        var meta = RootObject.Meta;
        Assert.That(meta.Fields, Is.Not.Null);
        Assert.That(meta.AllFetchList, Is.Not.Null);
    }
}
[GenerateMeta]
public partial class LanguageNameList
{
    public LanguageNameList()
    {
        Names = [];
    }

    [XmlArray(ElementName = "NAME.LIST")]
    [XmlArrayItem(ElementName = "NAME")]
    [TDLCollection(CollectionName = "Name")]
    public List<string> Names { get; set; }

    [XmlElement(ElementName = "LANGUAGEID")]
    public int LanguageId { get; set; }
}
public partial class BaseAliasedMasterObject

{
    [TDLField(Set = "$_FirstAlias")]
    public string? Alias { get; set; }

    [XmlElement(ElementName = "LANGUAGENAME.LIST")]
    [TDLCollection(CollectionName = "LanguageName")]
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