using TallyConnector.MigrationTests.Infrastructure;
using TallyConnector.MigrationTests.Models;

namespace TallyConnector.MigrationTests.Tests;

/// <summary>
/// Tests for XML attribute serialization comparison
/// </summary>
[TestFixture]
public class AttributeMappingTests
{
    [Test]
    public void XmlAttributes_SerializeIdentically()
    {
        // Arrange
        var testObj = new AttributeTestObject
        {
            Id = 123,
            Type = "TestType",
            Value = "TestValue"
        };

        // Act
        string currentXml = testObj.GetXML(indent: true);
        var generatedXml = testObj.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }

    [Test]
    public void XmlAttributes_WithNullStringAttribute_SerializeIdentically()
    {
        // Arrange
        var testObj = new AttributeTestObject
        {
            Id = 456,
            Type = null,
            Value = "SomeValue"
        };

        // Act
        string currentXml = testObj.GetXML(indent: true);
        var generatedXml = testObj.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }

    [Test]
    public void MixedAttributesAndElements_SerializeIdentically()
    {
        // Arrange
        var testObj = new AttributeTestObject
        {
            Id = 789,
            Type = "Mixed",
            Value = "Element Value"
        };

        // Act
        string currentXml = testObj.GetXML(indent: true);
        var generatedXml = testObj.WriteToXml();
        string sourceGeneratorXml = generatedXml.ToString();

        // Assert
        bool areEqual = SerializationComparer.AreXmlEquivalent(currentXml, sourceGeneratorXml, out string? difference);
        Assert.That(areEqual, Is.True, $"XML outputs differ:\n{difference}");
    }
}
