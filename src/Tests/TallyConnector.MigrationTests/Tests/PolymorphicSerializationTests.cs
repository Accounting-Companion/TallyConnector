using TallyConnector.MigrationTests.Models;

namespace TallyConnector.MigrationTests.Tests;

[TestFixture]
public class PolymorphicSerializationTests
{
    [Test]
    public void PolymorphicList_SerializesIdentically()
    {
        var testObj = new PolymorphicListTestObject
        {
            Items = new List<BaseItem>
            {
                new DerivedItemA { Name = "Item 1", PropertyA = "Value A" },
                new DerivedItemB { Name = "Item 2", PropertyB = "Value B" }
            }
        };

        VerifySerialization(testObj);
    }

    [Test]
    public void PolymorphicList_MixedOrder_SerializesIdentically()
    {
        var testObj = new PolymorphicListTestObject
        {
            Items = new List<BaseItem>
            {
                new DerivedItemB { Name = "Item 1", PropertyB = "Value B" },
                new DerivedItemA { Name = "Item 2", PropertyA = "Value A" },
                new DerivedItemB { Name = "Item 3", PropertyB = "Value B2" }
            }
        };

        VerifySerialization(testObj);
    }

    [Test]
    public void PolymorphicList_Empty_SerializesIdentically()
    {
        var testObj = new PolymorphicListTestObject
        {
            Items = new List<BaseItem>()
        };

        VerifySerialization(testObj);
    }

    [Test]
    public void PolymorphicList_Null_SerializesIdentically()
    {
        var testObj = new PolymorphicListTestObject
        {
            Items = null
        };

        VerifySerialization(testObj);
    }

    [Test]
    public void PolymorphicList_DeserializesIdentically()
    {
        string xml = @"<POLYMORPHICTEST>
  <ITEM_A>
    <NAME>Item 1</NAME>
    <PROP_A>Value A</PROP_A>
  </ITEM_A>
  <ITEM_B>
    <NAME>Item 2</NAME>
    <PROP_B>Value B</PROP_B>
  </ITEM_B>
</POLYMORPHICTEST>";

        // Act - Read using XmlSourceGenerator
        var testObj = new PolymorphicListTestObject();
        var element = System.Xml.Linq.XElement.Parse(xml);
        testObj.ReadFromXml(element);

        // Assert
        Assert.That(testObj.Items, Is.Not.Null);
        Assert.That(testObj.Items.Count, Is.EqualTo(2));
        
        Assert.That(testObj.Items[0], Is.InstanceOf<DerivedItemA>());
        Assert.That(testObj.Items[0].Name, Is.EqualTo("Item 1"));
        Assert.That(((DerivedItemA)testObj.Items[0]).PropertyA, Is.EqualTo("Value A"));

        Assert.That(testObj.Items[1], Is.InstanceOf<DerivedItemB>());
        Assert.That(testObj.Items[1].Name, Is.EqualTo("Item 2"));
        Assert.That(((DerivedItemB)testObj.Items[1]).PropertyB, Is.EqualTo("Value B"));
    }

    private void VerifySerialization<T>(T testObj) where T : TallyConnector.Core.Models.TallyXml, XmlSourceGenerator.Abstractions.IXmlStreamable
    {
        // Act - Get XML using current approach (TallyXml.GetXML)
        string currentXml = testObj.GetXML(indent: true);

        // Act - Get XML using XmlSourceGenerator approach
        var generatedXml = testObj.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = TallyConnector.MigrationTests.Infrastructure.SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }
}
