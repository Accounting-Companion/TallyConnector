using TallyConnector.MigrationTests.Models;

namespace TallyConnector.MigrationTests.Tests;

[TestFixture]
public class AttributeOverridesTests
{
    [Test]
    public void XmlSerializer_WithOverrides_AddsPolymorphism()
    {
        // Arrange
        var testObj = new PlainListObject
        {
            Items = new List<BaseItem>
            {
                new DerivedItemA { Name = "Item 1", PropertyA = "Value A" },
                new DerivedItemB { Name = "Item 2", PropertyB = "Value B" }
            }
        };

        var overrides = new XmlAttributeOverrides();
        var attrs = new XmlAttributes();
        attrs.XmlElements.Add(new XmlElementAttribute("ITEM_A", typeof(DerivedItemA)));
        attrs.XmlElements.Add(new XmlElementAttribute("ITEM_B", typeof(DerivedItemB)));
        overrides.Add(typeof(PlainListObject), "Items", attrs);

        // Act
        var serializer = new XmlSerializer(typeof(PlainListObject), overrides);
        using var writer = new StringWriter();
        serializer.Serialize(writer, testObj);
        string xml = writer.ToString();

        // Assert
        Assert.That(xml, Does.Contain("<ITEM_A>"));
        Assert.That(xml, Does.Contain("<ITEM_B>"));
    }

    [Test]
    public void XmlSerializer_RootOverride()
    {
        var testObj = new SimpleTestObject { Name = "Root Test" };
        var overrides = new XmlAttributeOverrides();
        var attrs = new XmlAttributes { XmlRoot = new XmlRootAttribute("NEW_ROOT") };
        overrides.Add(typeof(SimpleTestObject), attrs);

        var serializer = new XmlSerializer(typeof(SimpleTestObject), overrides);
        using var writer = new StringWriter();
        serializer.Serialize(writer, testObj);
        
        Assert.That(writer.ToString(), Does.Contain("<NEW_ROOT"));
    }

    [Test]
    public void XmlSourceGenerator_RootOverride()
    {
        var testObj = new SimpleTestObject { Name = "Root Test" };
        // XmlSerializationOptions does not appear to have a RootOverride property based on previous file view
        // But let's check if it can be done via PropertyOverrides with empty property name? No, key is (Type, string).
        
        // Checking if we can achieve this.
        var options = new XmlSourceGenerator.Abstractions.XmlSerializationOptions();
        // options.RootOverrides? No such property found in view.
        
        // If feature doesn't exist, this test demonstrates the gap.
        var element = testObj.WriteToXml(options);
        
        // Expectation: It fails to change root
        Assert.That(element.Name.LocalName, Is.Not.EqualTo("NEW_ROOT"));
    }

    [Test]
    public void XmlSerializer_IgnoreOverride()
    {
        var testObj = new SimpleTestObject { Name = "Ignore Test", Id = 99 };
        var overrides = new XmlAttributeOverrides();
        var attrs = new XmlAttributes();
        attrs.XmlIgnore = true;
        overrides.Add(typeof(SimpleTestObject), "Id", attrs);

        var serializer = new XmlSerializer(typeof(SimpleTestObject), overrides);
        using var writer = new StringWriter();
        serializer.Serialize(writer, testObj);
        
        Assert.That(writer.ToString(), Does.Not.Contain("<ID>99</ID>"));
    }

    [Test]
    public void XmlSourceGenerator_IgnoreOverride()
    {
        var testObj = new SimpleTestObject { Name = "Ignore Test", Id = 99 };
        var options = new XmlSourceGenerator.Abstractions.XmlSerializationOptions();
        // XmlSerializationOptions has PropertyOverrides which maps to string (element name).
        // It does not have an "Ignore" option per property.
        
        var element = testObj.WriteToXml(options);
        
        // Expectation: ID is still present
        Assert.That(element.ToString(), Does.Contain("<ID>99</ID>"));
    }

    [Test]
    public void XmlSourceGenerator_PolymorphicOverride()
    {
        // XmlSerializationOptions PropertyOverrides is Dictionary<(Type, string), string>
        // It maps a property to a SINGLE element name.
        // It cannot map a property to MULTIPLE element names based on the type of the value (polymorphism).
        
        var testObj = new PlainListObject
        {
            Items = new List<BaseItem>
            {
                new DerivedItemA { Name = "Item A" }
            }
        };

        var options = new XmlSourceGenerator.Abstractions.XmlSerializationOptions();
        // We can try to rename "Items", but we can't specify "Use ITEM_A for DerivedItemA".
        options.PropertyOverrides.Add((typeof(PlainListObject), "Items"), "NEW_ITEMS_NAME");

        var element = testObj.WriteToXml(options);
        string xml = element.ToString();

        // It might rename the container or the item depending on implementation, 
        // but it won't handle the type-based mapping "DerivedItemA -> ITEM_A".
        Assert.That(xml, Does.Not.Contain("<ITEM_A>"));
    }
}
