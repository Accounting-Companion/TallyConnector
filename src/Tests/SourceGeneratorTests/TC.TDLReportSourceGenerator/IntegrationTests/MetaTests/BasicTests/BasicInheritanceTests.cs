using System.Xml.Serialization;
using TallyConnector.Abstractions.Attributes;

namespace IntegrationTests.MetaTests.BasicTests;
internal class BasicInheritanceTests
{
    [Test]
    public void TestBasicMeta()
    {
        var c = BasicMetaModel.Meta;
        Assert.Multiple(() =>
        {
            Assert.That(c.TDLReportName, Is.EqualTo("BasicMetaModel_D5EZ").IgnoreCase);
            Assert.That(c.TDLDefaultCollectionName, Is.EqualTo("BasicMetaModelColl_D5EZ").IgnoreCase);
        });
        Assert.Multiple(() =>
        {
            Assert.That(c.Name.Set, Is.EqualTo("$NAME").IgnoreCase);
            Assert.That(c.Name.XMLTag, Is.EqualTo("NAME").IgnoreCase);
            Assert.That(c.Name.TDLType, Is.Null);
            Assert.That(c.Name.FetchText, Is.EqualTo("NAME").IgnoreCase);
            Assert.That(c.Name.Invisible, Is.Null);
            Assert.That(c.Name.Format, Is.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(c.Parent.Set, Is.EqualTo("$PARENT").IgnoreCase);
            Assert.That(c.Parent.XMLTag, Is.EqualTo("PARENT").IgnoreCase);
            Assert.That(c.Parent.TDLType, Is.Null);
            Assert.That(c.Parent.FetchText, Is.EqualTo("PARENT").IgnoreCase);
            Assert.That(c.Parent.Invisible, Is.Null);
            Assert.That(c.Parent.Format, Is.Null);
        });

    }
    [Test]
    public void TestInheritedMeta()
    {
        var c = BasicMetaModelInheritance.Meta;
        Assert.Multiple(() =>
        {
            Assert.That(c.Name.Set, Is.EqualTo("$NAME").IgnoreCase);
            Assert.That(c.Name.XMLTag, Is.EqualTo("NAME").IgnoreCase);
            Assert.That(c.Name.TDLType, Is.Null);
            Assert.That(c.Name.FetchText, Is.EqualTo("NAME").IgnoreCase);
            Assert.That(c.Name.Invisible, Is.Null);
            Assert.That(c.Name.Format, Is.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(c.Parent.Set, Is.EqualTo("$PARENT").IgnoreCase);
            Assert.That(c.Parent.XMLTag, Is.EqualTo("PARENT").IgnoreCase);
            Assert.That(c.Parent.TDLType, Is.Null);
            Assert.That(c.Parent.FetchText, Is.EqualTo("PARENT").IgnoreCase);
            Assert.That(c.Parent.Invisible, Is.Null);
            Assert.That(c.Parent.Format, Is.Null);
        });

        Assert.Multiple(() =>
        {
            Assert.That(c.IsBillWise.Set, Is.EqualTo("$ISBILLWISE").IgnoreCase);
            Assert.That(c.IsBillWise.XMLTag, Is.EqualTo("ISBILLWISE").IgnoreCase);
            Assert.That(c.IsBillWise.TDLType, Is.Null);
            Assert.That(c.IsBillWise.FetchText, Is.EqualTo("ISBILLWISE").IgnoreCase);
            Assert.That(c.IsBillWise.Invisible, Is.Null);
            Assert.That(c.IsBillWise.Format, Is.Null);
        });
    }
    [Test]
    public void TestInheritedMetaOveridden()
    {
        new BasicMetaModelMet()
        {
            TDLReportName = "",
        };
        var c = BasicMetaModelInheritanceInheritanceandOveridden.Meta;
        Assert.Multiple(() =>
        {
            Assert.That(c.Name.Set, Is.EqualTo("$NAME").IgnoreCase);
            Assert.That(c.Name.XMLTag, Is.EqualTo("NAME").IgnoreCase);
            Assert.That(c.Name.TDLType, Is.Null);
            Assert.That(c.Name.FetchText, Is.EqualTo("NAME").IgnoreCase);
            Assert.That(c.Name.Invisible, Is.Null);
            Assert.That(c.Name.Format, Is.Null);
        });

    }
}

[TDLCollection(Type = "Ledger")]
[GenerateMeta]
[GenerateITallyRequestableObect]
public partial class BasicMetaModel
{
    public string Name { get; set; }
    public string Parent { get; set; }
}
[TDLCollection(Type = "Ledger")]
[GenerateMeta]
public partial class BasicMetaModelBase
{
    public string Name { get; set; }
    public string Parent { get; set; }

    public OldClass Base { get; set; }
}

[TDLCollection(Type = "Ledger")]
[GenerateMeta]
partial class BasicMetaModelInheritance : BasicMetaModelBase
{
    [XmlElement(ElementName = "ISBILLWISEON")]
    public bool IsBillWise { get; set; }
}
[GenerateMeta]
[TDLCollection(Type = "Ledger")]
[GenerateITallyRequestableObect]
partial class BasicMetaModelInheritanceInheritanceandOveridden : BasicMetaModelBase
{

    [XmlElement(ElementName = "OVERIDDENNAME")]
    public new string Name { get; set; }

    [XmlElement(ElementName = "ISBILLWISEON")]
    public bool IsBillWise { get; set; }

   public new NewClass Base { get; set; }

   public  OldClass NewBase { get; set; }

}

public partial class NewClass
{
    public string NewName { get; set; }
    public string NewParent { get; set; }
}

public partial class OldClass
{
    public string OldName { get; set; }
    public string OldParent { get; set; }
}

public class BasicMetaModelMet : TallyConnector.Abstractions.Models.MetaObject
{
    public static BasicMetaModelMet Instance => new();

    public BasicMetaModelMet(string pathPrefix = "") : base(pathPrefix)
    {
    }

    public new  string TDLReportName = "BasicMetaModel_D5EZ";
    public TallyConnector.Abstractions.Models.PropertyMetaData Name => new("NAME_AQT4", "NAME");

    public List<TallyConnector.Abstractions.Models.PropertyMetaData> Fields => [Name];

}